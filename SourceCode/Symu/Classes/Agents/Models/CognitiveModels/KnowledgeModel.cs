#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Messaging.Templates;
using Symu.OrgMod.Edges;
using Symu.OrgMod.GraphNetworks;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
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
        private readonly ActorKnowledgeNetwork _actorKnowledgeNetwork;
        private readonly IAgentId _agentId;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly OneModeNetwork _knowledgeNetwork;
        private readonly MessageContent _messageContent;
        private readonly RandomGenerator _model;

        /// <summary>
        ///     Initialize Knowledge model :
        ///     update NetworkKnowledges
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        /// <param name="model"></param>
        public KnowledgeModel(IAgentId agentId, KnowledgeModelEntity entity,
            CognitiveArchitecture cognitiveArchitecture,
            GraphMetaNetwork network, RandomGenerator model)
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
            _actorKnowledgeNetwork = network.ActorKnowledge;
            _knowledgeAndBeliefs = cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
            _model = model;
        }

        public bool On { get; set; }

        /// <summary>
        ///     Get a readonly list of the actor knowledge
        /// </summary>
        public IReadOnlyList<IEntityKnowledge> Expertise =>
            On ? _actorKnowledgeNetwork.EdgesFilteredBySource(_agentId).ToList() : new List<IEntityKnowledge>();

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

            foreach (var agentKnowledge in _actorKnowledgeNetwork.EdgesFilteredBySource<ActorKnowledge>(_agentId))
            {
                InitializeActorKnowledge(agentKnowledge, !_knowledgeAndBeliefs.HasInitialKnowledge, step);
            }
        }


        /// <summary>
        ///     Initialize AgentExpertise with a stochastic process based on the agent knowledge level
        /// </summary>
        /// <param name="actorKnowledge">AgentKnowledge to initialize</param>
        /// <param name="neutral">if !HasInitialKnowledge, then a neutral (KnowledgeLevel.NoKnowledge) initialization is done</param>
        /// <param name="step"></param>
        public void InitializeActorKnowledge(ActorKnowledge actorKnowledge, bool neutral, ushort step)
        {
            if (actorKnowledge is null)
            {
                throw new ArgumentNullException(nameof(actorKnowledge));
            }

            var knowledge = GetKnowledge(actorKnowledge.Target);
            if (knowledge == null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var level = neutral ? KnowledgeLevel.NoKnowledge : actorKnowledge.KnowledgeLevel;
            actorKnowledge.InitializeKnowledge(knowledge.Length, _model, level, step);
        }

        /// <summary>
        ///     Initialize the knowledge of the agent based on the knowledge network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="step"></param>
        public void InitializeKnowledge(IAgentId knowledgeId, ushort step)
        {
            InitializeActorKnowledge(GetActorKnowledge(knowledgeId),
                !_knowledgeAndBeliefs.HasInitialKnowledge, step);
        }

        /// <summary>
        ///     Add an agentId's expertise to the network
        /// </summary>
        /// <param name="expertise"></param>
        public void AddExpertise(IEnumerable<IEntityKnowledge> expertise)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            _actorKnowledgeNetwork.Add(expertise);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="level"></param>
        /// <param name="internalCharacteristics"></param>
        public void AddKnowledge(IAgentId knowledgeId, KnowledgeLevel level,
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
        public void AddKnowledge(IAgentId knowledgeId, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            ActorKnowledge.CreateInstance(_actorKnowledgeNetwork, _agentId, knowledgeId, level, minimumKnowledge, timeToLive);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public void AddKnowledge(IAgentId knowledgeId, float[] knowledgeBits, float minimumKnowledge, short timeToLive)
        {
            if (!On || !_knowledgeAndBeliefs.HasKnowledge)
            {
                return;
            }

            ActorKnowledge.CreateInstance(_actorKnowledgeNetwork, _agentId, knowledgeId, knowledgeBits, minimumKnowledge, timeToLive);
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
        public Bits FilterKnowledgeToSend(IAgentId knowledgeId, byte knowledgeBit, CommunicationTemplate medium,
            ushort step, out byte[] knowledgeIndexToSend)
        {
            knowledgeIndexToSend = null;
            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            // If can't send knowledge or no knowledge asked
            if (!On || !_messageContent.CanSendKnowledge) //|| Expertise == null)
            {
                return null;
            }

            if (!KnowsEnough(knowledgeId, knowledgeBit,
                _messageContent.MinimumKnowledgeToSendPerBit, step))
            {
                return null;
            }

            var agentKnowledge = GetActorKnowledge(knowledgeId);
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

        public ActorKnowledge GetActorKnowledge(IAgentId knowledgeId)
        {
            return _actorKnowledgeNetwork.Edge<ActorKnowledge>(_agentId, knowledgeId);
        }

        public void SetKnowledge(IAgentId knowledgeId, byte index, float value, ushort step)
        {
            var actorKnowledge = GetActorKnowledge(knowledgeId);
            if (actorKnowledge == null)
            {
                throw new NullReferenceException(nameof(actorKnowledge));
            }

            actorKnowledge.SetKnowledgeBit(index, value, step);
        }

        public void SetKnowledge(IAgentId knowledgeId, float[] knowledgeBits, ushort step)
        {
            var actorKnowledge = GetActorKnowledge(knowledgeId);
            if (actorKnowledge == null)
            {
                throw new NullReferenceException(nameof(actorKnowledge));
            }

            actorKnowledge.SetKnowledgeBits(knowledgeBits, step);
        }

        public Knowledge GetKnowledge(IAgentId knowledgeId)
        {
            return _knowledgeNetwork.GetEntity<Knowledge>(knowledgeId);
        }

        #region Knowledge Analyze

        /// <summary>
        ///     Get the sum of all the knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgeSum()
        {
            return _actorKnowledgeNetwork.EdgesFilteredBySource<ActorKnowledge>(_agentId).Sum(l => l.GetKnowledgeSum());
        }

        /// <summary>
        ///     Get the maximum potential knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgePotential()
        {
            return _actorKnowledgeNetwork.EdgesFilteredBySource<ActorKnowledge>(_agentId)
                .Sum(l => l.GetKnowledgePotential());
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
            if (_actorKnowledgeNetwork.EdgesFilteredBySourceCount(_agentId) > 0)
            {
                return _actorKnowledgeNetwork.EdgesFilteredBySource<ActorKnowledge>(_agentId)
                    .Average(t => t.KnowledgeBits.Obsolescence(step));
            }

            return 0;
        }

        #endregion

        #region KnowsEnough

        /// <summary>
        ///     Check if agentKnowLedgeBits include or not taskKnowledgeIndexes
        /// </summary>
        /// <param name="actorKnowledge"></param>
        /// <param name="taskKnowledgeIndexes">indexes of the KnowledgeBits that must be checked</param>
        /// <param name="index"></param>
        /// <param name="knowledgeThreshHoldForDoing"></param>
        /// <param name="step"></param>
        /// <returns>0 if agentKnowLedgeBits include taskKnowledge</returns>
        /// <returns>index if agentKnowLedgeBits include taskKnowledge</returns>
        public bool Check(ActorKnowledge actorKnowledge, byte[] taskKnowledgeIndexes, out byte index,
            float knowledgeThreshHoldForDoing, ushort step)
        {
            if (taskKnowledgeIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskKnowledgeIndexes));
            }

            for (byte i = 0; i < taskKnowledgeIndexes.Length; i++)
            {
                if (KnowsEnough(actorKnowledge, taskKnowledgeIndexes[i], knowledgeThreshHoldForDoing, step))
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
        public bool KnowsEnough(IAgentId knowledgeId, byte knowledgeBit, float knowledgeThreshHoldForAnswer,
            ushort step)
        {
            return _actorKnowledgeNetwork.Exists(_agentId, knowledgeId) && 
                   KnowsEnough(GetActorKnowledge(knowledgeId), knowledgeBit, knowledgeThreshHoldForAnswer, step);
        }

        /// <summary>
        ///     Check that agent has the knowledgeBit
        /// </summary>
        /// <param name="actorKnowledge"></param>
        /// <param name="index">index of the knowledgeBit</param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if agent has the knowledge</returns>
        public static bool KnowsEnough(ActorKnowledge actorKnowledge, byte index, float knowledgeThreshHoldForAnswer,
            ushort step)
        {
            if (actorKnowledge == null)
            {
                throw new ArgumentNullException(nameof(actorKnowledge));
            }

            if (actorKnowledge.Length == 0)
            {
                return false;
            }

            if (index >= actorKnowledge.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return actorKnowledge.KnowledgeBits.GetBit(index, step) >= knowledgeThreshHoldForAnswer;
        }

        #endregion
    }
}