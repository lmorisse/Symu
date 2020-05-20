#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Classes.Task;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.Math.ProbabilityDistributions;
using static SymuTools.Constants;

#endregion

namespace SymuEngine.Classes.Agents.Models.CognitiveModel
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will manage its beliefs
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The BeliefsModel initialize the real value of the agent's beliefs parameters and its real behaviour
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class BeliefsModel
    {
        private readonly AgentId _agentId;
        private readonly InternalCharacteristics _internalCharacteristics;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly MessageContent _messageContent;
        private readonly NetworkBeliefs _networkBeliefs;

        /// <summary>
        ///     Initialize Beliefs model :
        ///     update NetworkBeliefs
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public BeliefsModel(AgentId agentId, ModelEntity entity, CognitiveArchitecture cognitiveArchitecture,
            Network network)
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
            _networkBeliefs = network.NetworkBeliefs;
            _internalCharacteristics = cognitiveArchitecture.InternalCharacteristics;
            _knowledgeAndBeliefs = cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
        }

        public bool On { get; set; }

        /// <summary>
        ///     Get the agent Beliefs
        /// </summary>
        public AgentBeliefs Beliefs =>
            _knowledgeAndBeliefs.HasBelief ? _networkBeliefs.GetAgentBeliefs(_agentId) : null;

        /// <summary>
        ///     Initialize the beliefs of the agent based on the belief network
        /// </summary>
        public void InitializeBeliefs()
        {
            if (!_knowledgeAndBeliefs.HasBelief || !On)
            {
                return;
            }

            if (!_networkBeliefs.Exists(_agentId))
                // An agent may be able to have belief but with no expertise for the moment
            {
                _networkBeliefs.AddAgentId(_agentId);
            }

            _networkBeliefs.InitializeBeliefs(_agentId, !_knowledgeAndBeliefs.HasInitialBelief);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on agentExpertise to the network
        /// </summary>
        /// <param name="expertiseAgent"></param>
        public void AddBeliefs(AgentExpertise expertiseAgent)
        {
            if (!_knowledgeAndBeliefs.HasBelief || !On)
            {
                return;
            }

            _networkBeliefs.Add(_agentId, expertiseAgent, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the DefaultBeliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void AddBelief(ushort knowledgeId)
        {
            if (!_knowledgeAndBeliefs.HasBelief || !On)
            {
                return;
            }

            _networkBeliefs.Add(_agentId, knowledgeId, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the beliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="beliefLevel"></param>
        public void AddBelief(ushort knowledgeId, BeliefLevel beliefLevel)
        {
            if (!_knowledgeAndBeliefs.HasBelief || !On)
            {
                return;
            }

            _networkBeliefs.Add(_agentId, knowledgeId, beliefLevel);
        }

        public AgentBelief GetBelief(ushort beliefId)
        {
            return Beliefs.GetBelief(beliefId);
        }

        /// <summary>
        ///     Check belief required by a task against the worker expertise
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="taskBitIndexes">KnowledgeBit indexes of the task that must be checked against agent's beliefs</param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        public void CheckBelief(ushort beliefId, TaskKnowledgeBits taskBitIndexes, ref float mandatoryCheck,
            ref float requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            // model is off
            // agent may don't have the belief at all
            if (!On || Beliefs is null)
            {
                mandatoryCheck = 0;
                requiredCheck = 0;
                return;
            }

            var agentBelief = Beliefs.GetBelief(beliefId);
            var belief = _networkBeliefs.GetBelief(beliefId);
            if (belief is null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            if (agentBelief == null)
            {
                return;
            }

            mandatoryCheck = agentBelief.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex, belief,
                _internalCharacteristics.BeliefThreshHoldForReacting);
            requiredCheck = agentBelief.Check(taskBitIndexes.GetRequired(), out requiredIndex, belief,
                _internalCharacteristics.BeliefThreshHoldForReacting);
        }

        /// <summary>
        ///     The agent have received a message that ask for belief in return
        ///     stochastic Filtering agentKnowledge based on MinimumBitsOfKnowledgeToSend/MaximumBitsOfKnowledgeToSend
        ///     Work with non binary KnowledgeBits
        /// </summary>
        /// <returns>null if he don't have the belief or the right</returns>
        /// <returns>a beliefBits if he has the belief or the right</returns>
        /// <returns>With binary KnowledgeBits it will return a float of 0</returns>
        /// <example>KnowledgeBits[0,1,0.6] and MinimumKnowledgeToSend = 0.8 => KnowledgeBits[0,1,0]</example>
        public Bits FilterBeliefToSend(ushort beliefId, byte beliefBit, CommunicationTemplate medium)
        {
            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            // If don't have belief, can't send belief or no belief asked
            if (!_knowledgeAndBeliefs.HasBelief || !_messageContent.CanSendBeliefs || !On) // || beliefId == 0)
            {
                return null;
            }

            // intentionally after Cognitive.MessageContent.CanSendBeliefs test
            if (Beliefs == null)
            {
                throw new NullReferenceException(nameof(Beliefs));
            }

            // If Agent don't have the belief, he can't reply
            if (!Beliefs.BelievesEnough(beliefId, beliefBit,
                _messageContent.MinimumBeliefToSendPerBit))
            {
                return null;
            }

            var agentBelief = Beliefs.GetBelief(beliefId);
            if (agentBelief is null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            // Filter the belief to send, via the good communication channel// Filtering via the good channel
            var minBits = Math.Max(_messageContent.MinimumNumberOfBitsOfBeliefToSend,
                medium.Cognitive.MessageContent.MinimumNumberOfBitsOfBeliefToSend);
            var maxBits = Math.Min(_messageContent.MaximumNumberOfBitsOfBeliefToSend,
                medium.Cognitive.MessageContent.MaximumNumberOfBitsOfBeliefToSend);
            var minKnowledge = Math.Max(_messageContent.MinimumBeliefToSendPerBit,
                medium.Cognitive.MessageContent.MinimumBeliefToSendPerBit);
            // Random knowledgeBits to send
            if (minBits > maxBits)
            {
                minBits = maxBits;
            }

            var lengthToSend = DiscreteUniform.SampleToByte(minBits, maxBits);
            if (lengthToSend == 0)
            {
                return null;
            }

            var beliefIndexToSend = DiscreteUniform.SamplesToByte(lengthToSend, agentBelief.Length - 1);
            // Force the first index of the knowledgeIndex To send to be the knowledgeBit asked by an agent
            beliefIndexToSend[0] = beliefBit;
            var beliefBitsToSend = agentBelief.CloneWrittenBeliefBits(_messageContent.MinimumBeliefToSendPerBit);
            // knowledgeBitsToSend full of 0 except for the random indexes knowledgeIndexToSend
            for (byte i = 0; i < beliefBitsToSend.Length; i++)
            {
                if (!beliefIndexToSend.Contains(i) || Math.Abs(beliefBitsToSend.GetBit(i)) < minKnowledge)
                {
                    beliefBitsToSend.SetBit(i, 0);
                }
            }

            // Check Length of message
            // We don't find always what we were looking for
            return Math.Abs(beliefBitsToSend.GetSum()) < Tolerance ? null : beliefBitsToSend;
        }
    }
}