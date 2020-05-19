#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Classes.Task;
using SymuEngine.Environment;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Influences;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;
using static SymuTools.Constants;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will manage its knowledge
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The KnowledgeModels initialize the real value of the agent's knowledge parameters 
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class KnowledgeModel
    {
        public bool On { get; set; }
        private readonly AgentId _agentId;
        private readonly NetworkKnowledges _networkKnowledges;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly MessageContent _messageContent;
        private readonly InternalCharacteristics _internalCharacteristics;

        /// <summary>
        ///     Get the Agent Expertise
        /// </summary>
        public AgentExpertise Expertise =>
            _knowledgeAndBeliefs.HasKnowledge ? _networkKnowledges.GetAgentExpertise(_agentId) : new AgentExpertise();

        /// <summary>
        /// Initialize Knowledge model :
        /// update NetworkKnowledges
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public KnowledgeModel(AgentId agentId, ModelEntity entity, CognitiveArchitecture cognitiveArchitecture, Network network)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }
            On = entity.IsAgentOn();
            _agentId = agentId;
            _networkKnowledges = network.NetworkKnowledges;
            _knowledgeAndBeliefs= cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
            _internalCharacteristics = cognitiveArchitecture.InternalCharacteristics;

        }

        /// <summary>
        ///     Check Knowledge required by a task against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="taskBitIndexes">KnowledgeBits indexes of the task that must be checked against worker Knowledge</param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <param name="step"></param>
        public void CheckKnowledge(ushort knowledgeId, TaskKnowledgeBits taskBitIndexes, ref bool mandatoryCheck,
            ref bool requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex, ushort step)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            // model is off
            if (!On)
            {
                return;
            }

            // agent may don't have the knowledge at all
            var workerKnowledge = Expertise?.GetKnowledge(knowledgeId);
            if (workerKnowledge == null)
            {
                return;
            }

            mandatoryCheck = workerKnowledge.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex,
                _internalCharacteristics.KnowledgeThreshHoldForReacting, step);
            requiredCheck = workerKnowledge.Check(taskBitIndexes.GetRequired(), out requiredIndex,
                _internalCharacteristics.KnowledgeThreshHoldForReacting, step);
        }

        /// <summary>
        ///     Check Knowledge required against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit">KnowledgeBit index of the task that must be checked against worker Knowledge</param>
        /// <param name="step"></param>
        /// <returns>True if the knowledgeBit is known enough</returns>
        public bool CheckKnowledge(ushort knowledgeId, byte knowledgeBit, ushort step)
        {
            if (!On)
            {
                return false;
            }
            // workerKnowledge may don't have the knowledge at all
            var workerKnowledge = Expertise?.GetKnowledge(knowledgeId);
            return workerKnowledge != null &&
                   workerKnowledge.KnowsEnough(knowledgeBit, _internalCharacteristics.KnowledgeThreshHoldForReacting, step);
        }

        /// <summary>
        ///     Initialize the expertise of the agent based on the knowledge network
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public void InitializeExpertise(ushort step)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge) 
            {
                return;
            }

            if (!_networkKnowledges.Exists(_agentId))
            // An agent may be able to have knowledge but with no expertise for the moment
            {
                _networkKnowledges.AddAgentId(_agentId);
            }

            _networkKnowledges.InitializeExpertise(_agentId, !_knowledgeAndBeliefs.HasInitialKnowledge, step);
        }

        /// <summary>
        ///     Add an agentId's expertise to the network
        /// </summary>
        /// <param name="expertise"></param>
        public void AddExpertise(AgentExpertise expertise)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            _networkKnowledges.Add(_agentId, expertise);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="internalCharacteristics"></param>
        public void AddKnowledge(ushort knowledgeId, KnowledgeLevel level,
            InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            AddKnowledge(knowledgeId, level, internalCharacteristics.MinimumRemainingKnowledge,
                internalCharacteristics.TimeToLive);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public void AddKnowledge(ushort knowledgeId, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            _networkKnowledges.Add(_agentId, knowledgeId, level, minimumKnowledge, timeToLive);
        }


        /// <summary>
        ///     The agent have received a message that ask for knowledge in return
        ///     stochastic Filtering agentKnowledge based on MinimumBitsOfKnowledgeToSend/MaximumBitsOfKnowledgeToSend
        ///     Work with non binary KnowledgeBits
        /// </summary>
        /// <returns>null if he don't have the knowledge or the right</returns>
        /// <returns>a knowledgeBits if he has the knowledge or the right</returns>
        /// <returns>With binary KnowledgeBits it will return a float of 0</returns>
        /// <example>KnowledgeBits[0,1,0.6] and MinimumKnowledgeToSend = 0.8 => KnowledgeBits[0,1,0]</example>
        public Bits FilterKnowledgeToSend(ushort knowledgeId, byte knowledgeBit, CommunicationTemplate medium, ushort step, out byte[] knowledgeIndexToSend)
        {
            knowledgeIndexToSend = null;
            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }
            // If can't send knowledge or no knowledge asked
            if (!On || !_messageContent.CanSendKnowledge || knowledgeId == 0 ||
                Expertise == null)
            {
                return null;
            }

            if (!Expertise.KnowsEnough(knowledgeId, knowledgeBit,
                _messageContent.MinimumKnowledgeToSendPerBit, step))
            {
                return null;
            }

            var agentKnowledge = Expertise.GetKnowledge(knowledgeId);
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }
            // Filter the Knowledge to send, via the good communication medium
            //var bitsToSend = GetFilteredKnowledgeToSend(agentKnowledge, knowledgeBit, medium,
            //    out knowledgeIndexToSend);

            knowledgeIndexToSend = null;
            // Filtering via the good channel
            var minBits = Math.Max(_messageContent.MinimumNumberOfBitsOfKnowledgeToSend,
                medium.Cognitive.MessageContent.MinimumNumberOfBitsOfKnowledgeToSend);
            var maxBits = Math.Min(_messageContent.MaximumNumberOfBitsOfKnowledgeToSend,
                medium.Cognitive.MessageContent.MaximumNumberOfBitsOfKnowledgeToSend);
            var minKnowledge = Math.Max(_messageContent.MinimumKnowledgeToSendPerBit,
                medium.Cognitive.MessageContent.MinimumKnowledgeToSendPerBit);
            // Random knowledgeBits to send
            var lengthToSend = DiscreteUniform.SampleToByte(Math.Min(minBits, maxBits), Math.Max(minBits, maxBits));
            if (lengthToSend == 0)
            {
                return null;
            }

            knowledgeIndexToSend = DiscreteUniform.SamplesToByte(lengthToSend, agentKnowledge.Length - 1);
            // Force the first index of the knowledgeIndex To send to be the knowledgeBit asked by an agent
            knowledgeIndexToSend[0] = knowledgeBit;
            var knowledgeBitsToSend = Bits.Initialize(agentKnowledge.Length, 0F);
            for (byte i = 0; i < knowledgeIndexToSend.Length; i++)
            {
                var index = knowledgeIndexToSend[i];
                if (agentKnowledge.GetKnowledgeBit(index) >= minKnowledge)
                {
                    knowledgeBitsToSend[index] = agentKnowledge.GetKnowledgeBit(index);
                }
            }

            var bitsToSend = new Bits(knowledgeBitsToSend, 0);
            // Check Length of message
            // We don't find always what we were looking for
            return Math.Abs(bitsToSend.GetSum()) < Tolerance ? null : bitsToSend;
        }

        public AgentKnowledge GetKnowledge(ushort knowledgeId)
        {
            return Expertise.GetKnowledge(knowledgeId);
        }
    }
}