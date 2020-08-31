#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA;
using Symu.DNA.Knowledges;
using Symu.Messaging.Templates;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using static Symu.Common.Constants;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will manage its knowledge
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The KnowledgeModels initialize the real value of the agent's knowledge parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class KnowledgeModel
    {
        private readonly IAgentId _agentId;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly MessageContent _messageContent;
        private readonly KnowledgeNetwork _knowledgeNetwork;

        /// <summary>
        ///     Initialize Knowledge model :
        ///     update NetworkKnowledges
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        public KnowledgeModel(IAgentId agentId, ModelEntity entity, CognitiveArchitecture cognitiveArchitecture,
            MetaNetwork network)
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
            _knowledgeNetwork = network.Knowledge;
            _knowledgeAndBeliefs = cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
            if (_knowledgeAndBeliefs.HasKnowledge)
            {
                if (_knowledgeNetwork.Exists(_agentId))
                {
                    Expertise = _knowledgeNetwork.GetAgentExpertise(_agentId);
                }
                else
                {
                    Expertise = new AgentExpertise();
                    _knowledgeNetwork.Add(_agentId, Expertise);
                }
            }
            else
            {
                Expertise = new AgentExpertise();
            }
        }

        public bool On { get; set; }

        /// <summary>
        ///     Get the Agent Expertise
        /// </summary>
        public AgentExpertise Expertise { get; }

        #region Knowledge Analyze

        /// <summary>
        ///     Get the sum of all the knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgeSum()
        {
            return Expertise.GetAgentKnowledges<AgentKnowledge>().Sum(l => l.GetKnowledgeSum());
        }

        /// <summary>
        ///     Get the maximum potential knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgePotential()
        {
            return Expertise.GetAgentKnowledges<AgentKnowledge>().Sum(l => l.GetKnowledgePotential());
        }
        /// <summary>
        ///     Percentage of all Knowledge of the agent for all knowledge during the simulation
        /// </summary>
        public float PercentageKnowledge
        {
            get
            {
                float percentage = 0;
                var sum = GetKnowledgeSum();
                var potential = GetKnowledgePotential();
                if (potential > Tolerance)
                {
                    percentage = 100 * sum / potential;
                }

                return percentage;
            }
        }

        /// <summary>
        ///     Average of all the knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        public float Obsolescence(float step)
        {
            return Expertise.List.Any() ? Expertise.GetAgentKnowledges<AgentKnowledge>().Average(t => t.KnowledgeBits.Obsolescence(step)) : 0;
        }
        #endregion

        /// <summary>
        ///     Initialize the expertise of the agent based on the knowledge network
        /// </summary>
        /// <param name="step"></param>
        public void InitializeExpertise(ushort step)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            _knowledgeNetwork.AddAgentId(_agentId, Expertise);
            //_knowledgeNetwork.InitializeExpertise(_agentId, !_knowledgeAndBeliefs.HasInitialKnowledge, step);

            foreach (var agentKnowledge in _knowledgeNetwork.GetAgentExpertise(_agentId).List)
            {
                InitializeAgentKnowledge((AgentKnowledge)agentKnowledge, !_knowledgeAndBeliefs.HasInitialKnowledge, step);
            }
        }



        /// <summary>
        ///     Initialize AgentExpertise with a stochastic process based on the agent knowledge level
        /// </summary>
        /// <param name="agentKnowledge">AgentKnowledge to initialize</param>
        /// <param name="neutral">if !HasInitialKnowledge, then a neutral (KnowledgeLevel.NoKnowledge) initialization is done</param>
        /// <param name="step"></param>
        public void InitializeAgentKnowledge(AgentKnowledge agentKnowledge, bool neutral, ushort step)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var knowledge = GetKnowledge(agentKnowledge.KnowledgeId);
            if (knowledge == null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var level = neutral ? KnowledgeLevel.NoKnowledge : agentKnowledge.KnowledgeLevel;
            agentKnowledge.InitializeKnowledge(knowledge.Length, _knowledgeNetwork.Model, level, step);
        }

        /// <summary>
        ///     Initialize the knowledge of the agent based on the knowledge network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="step"></param>
        public void InitializeKnowledge(IId knowledgeId, ushort step)
        {
            InitializeAgentKnowledge(GetAgentKnowledge(knowledgeId),
                !_knowledgeAndBeliefs.HasInitialKnowledge, step);
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

            _knowledgeNetwork.Add(_agentId, expertise);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="internalCharacteristics"></param>
        public void AddKnowledge(IId knowledgeId, KnowledgeLevel level,
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
        public void AddKnowledge(IId knowledgeId, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            var agentKnowledge = new AgentKnowledge(knowledgeId, level, minimumKnowledge, timeToLive);
            _knowledgeNetwork.Add(_agentId, agentKnowledge);
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
        public Bits FilterKnowledgeToSend(IId knowledgeId, byte knowledgeBit, CommunicationTemplate medium,
            ushort step, out byte[] knowledgeIndexToSend)
        {
            knowledgeIndexToSend = null;
            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            // If can't send knowledge or no knowledge asked
            if (!On || !_messageContent.CanSendKnowledge || //knowledgeId == 0 ||
                Expertise == null)
            {
                return null;
            }

            if (!KnowsEnough(knowledgeId, knowledgeBit,
                _messageContent.MinimumKnowledgeToSendPerBit, step))
            {
                return null;
            }

            var agentKnowledge = GetAgentKnowledge(knowledgeId);
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }
            // Filter the Knowledge to send, via the good communication medium

            knowledgeIndexToSend = null;
            // Filtering via the good channel
            var minBits = Math.Max(_messageContent.MinimumNumberOfBitsOfKnowledgeToSend,
                medium.MinimumNumberOfBitsOfKnowledgeToSend);
            var maxBits = Math.Min(_messageContent.MaximumNumberOfBitsOfKnowledgeToSend,
                medium.MaximumNumberOfBitsOfKnowledgeToSend);
            var minKnowledge = Math.Max(_messageContent.MinimumKnowledgeToSendPerBit,
                medium.MinimumKnowledgeToSendPerBit);
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

        public AgentKnowledge GetAgentKnowledge(IId knowledgeId)
        {
            return Expertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId);
        }

        public Knowledge GetKnowledge(IId knowledgeId)
        {
            return _knowledgeNetwork.GetKnowledge<Knowledge>(knowledgeId);
        }

        #region KnowsEnough

        /// <summary>
        ///     Check if agentKnowLedgeBits include or not taskKnowledgeIndexes
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="taskKnowledgeIndexes">indexes of the KnowledgeBits that must be checked</param>
        /// <param name="index"></param>
        /// <param name="knowledgeThreshHoldForDoing"></param>
        /// <param name="step"></param>
        /// <returns>0 if agentKnowLedgeBits include taskKnowledge</returns>
        /// <returns>index if agentKnowLedgeBits include taskKnowledge</returns>
        public bool Check(AgentKnowledge agentKnowledge, byte[] taskKnowledgeIndexes, out byte index, float knowledgeThreshHoldForDoing, ushort step)
        {
            if (taskKnowledgeIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskKnowledgeIndexes));
            }

            for (byte i = 0; i < taskKnowledgeIndexes.Length; i++)
            {
                if (KnowsEnough(agentKnowledge, taskKnowledgeIndexes[i], knowledgeThreshHoldForDoing, step))
                {
                    continue;
                }

                index = taskKnowledgeIndexes[i];
                return false;
            }

            index = 0;
            return true;
        }

        /// <summary>
        ///     Check that agent has the knowledgeId[knowledgeBit] == 1
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit"></param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if the agent has the knowledge</returns>
        public bool KnowsEnough(IId knowledgeId, byte knowledgeBit, float knowledgeThreshHoldForAnswer, ushort step)
        {
            if (!Expertise.Contains(knowledgeId))
            {
                return false;
            }

            return KnowsEnough(GetAgentKnowledge(knowledgeId), knowledgeBit, knowledgeThreshHoldForAnswer, step);
        }

        /// <summary>
        ///     Check that agent has the knowledgeBit
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="index">index of the knowledgeBit</param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if agent has the knowledge</returns>
        public static bool KnowsEnough(AgentKnowledge agentKnowledge, byte index, float knowledgeThreshHoldForAnswer, ushort step)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            if (agentKnowledge.Length == 0)
            {
                return false;
            }

            if (index >= agentKnowledge.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return agentKnowledge.KnowledgeBits.GetBit(index, step) >= knowledgeThreshHoldForAnswer;
        }
        #endregion
    }
}