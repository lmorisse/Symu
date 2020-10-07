#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.XPath;
using Symu.Classes.Organization;
using Symu.Common.Classes;
using Symu.Common.Interfaces;

using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA;
using Symu.Messaging.Templates;
using Symu.OrgMod.Edges;
using Symu.OrgMod.GraphNetworks;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks;
using Symu.Repository.Edges;
using Symu.Repository.Entities;
using static Symu.Common.Constants;
using ActorBelief = Symu.Repository.Edges.ActorBelief;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will manage its beliefs
    ///     Entity enable or not this mechanism for all the agents during the simulation
    ///     The BeliefsModel initialize the real value of the agent's beliefs parameters and its real behaviour
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    public class BeliefsModel
    {
        private readonly RandomGenerator _model;
        private readonly IAgentId _agentId;
        private readonly KnowledgeAndBeliefs _knowledgeAndBeliefs;
        private readonly MessageContent _messageContent;
        private readonly ActorBeliefNetwork _actorBeliefNetwork;
        private readonly OneModeNetwork _beliefNetwork;
        public BeliefModelEntity Entity { get; }

        /// <summary>
        ///     Initialize Beliefs model :
        ///     update NetworkBeliefs
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        /// <param name="model"></param>
        public BeliefsModel(IAgentId agentId, BeliefModelEntity entity, CognitiveArchitecture cognitiveArchitecture,
            GraphMetaNetwork network, RandomGenerator model)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            if (cognitiveArchitecture == null)
            {
                throw new ArgumentNullException(nameof(cognitiveArchitecture));
            }

            _agentId = agentId;
            _beliefNetwork = network.Belief;
            _actorBeliefNetwork = network.ActorBelief;
            _knowledgeAndBeliefs = cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
            _model = model;
            Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        }

        public bool On => _knowledgeAndBeliefs.HasBelief && Entity.IsAgentOn();

        /// <summary>
        ///     Get a readonly list of the actor Beliefs
        /// </summary>
        public IReadOnlyList<IActorBelief> Beliefs =>
            On ? _actorBeliefNetwork.EdgesFilteredBySource(_agentId).ToList() : new List<IActorBelief>();

        /// <summary>
        ///     Initialize the beliefs of the agent based on the belief network
        /// </summary>
        public void InitializeBeliefs()
        {
            if (!On)
            {
                return;
            }

            InitializeBeliefs(!_knowledgeAndBeliefs.HasInitialBelief);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on agentExpertise to the network
        /// </summary>
        /// <param name="expertiseActor"></param>
        public void AddBeliefsFromKnowledge(IEnumerable<IEntityKnowledge> expertiseActor)
        {
            if (!On)
            {
                return;
            }

            AddBeliefsFromKnowledge(expertiseActor, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }
        public void AddBeliefsFromKnowledge(IEnumerable<IEntityKnowledge> expertise, BeliefLevel beliefLevel)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            foreach (var knowledgeId in expertise.Select(x => x.Target))
            {
                AddBeliefFromKnowledgeId(knowledgeId, beliefLevel);
            }
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the DefaultBeliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void AddBeliefFromKnowledgeId(IAgentId knowledgeId)
        {
            AddBeliefFromKnowledgeId(knowledgeId, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }
        /// <summary>
        ///     Add an agentId's beliefs 
        ///     using the default beliefLevel
        /// </summary>
        /// <param name="beliefId"></param>
        public void AddBelief(IAgentId beliefId)
        {
            AddBelief(beliefId, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }
        /// <summary>
        ///     Add an agentId's beliefs 
        ///     using the beliefLevel
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefLevel"></param>
        public void AddBelief(IAgentId beliefId, BeliefLevel beliefLevel)
        {
            if (!On)
            {
                return;
            }

            var actorBelief = new ActorBelief(_agentId, beliefId, beliefLevel);
            _actorBeliefNetwork.Add(actorBelief);
        }


        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the beliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="beliefLevel"></param>
        public void AddBeliefFromKnowledgeId(IAgentId knowledgeId, BeliefLevel beliefLevel)
        {
            if (!On)
            {
                return;
            }

            var belief = GetBeliefFromKnowledgeId(knowledgeId);
            if (belief == null)
            {
                throw new NullReferenceException(nameof(belief));
            }
            var actorBelief = new ActorBelief(_agentId, belief.EntityId, beliefLevel);
            _actorBeliefNetwork.Add(actorBelief);
        }

        public ActorBelief GetActorBelief(IAgentId beliefId)
        {
            return _actorBeliefNetwork.Edge<ActorBelief>(_agentId,beliefId);
        }
        public void SetBelief(IAgentId beliefId, byte index, float value)
        {
            GetActorBelief(beliefId).BeliefBits.SetBit(index, value);
        }


        public void SetBelief(IAgentId beliefId, float[] beliefBits)
        {
            GetActorBelief(beliefId).SetBeliefBits(beliefBits);
        }

        public Belief GetBelief(IAgentId beliefId)
        {
            return _beliefNetwork.GetEntity<Belief>(beliefId);
        }

        public Belief GetBeliefFromKnowledgeId(IAgentId knowledgeId)
        {
            return _beliefNetwork.List.OfType<Belief>().FirstOrDefault(x => x.KnowledgeId != null && x.KnowledgeId.Equals(knowledgeId));
        }
        public IAgentId GetBeliefIdFromKnowledgeId(IAgentId knowledgeId)
        {
            var belief = GetBeliefFromKnowledgeId(knowledgeId);
            return belief?.EntityId;
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
        public Bits FilterBeliefToSendFromKnowledgeId(IAgentId knowledgeId, byte beliefBit, CommunicationTemplate medium)
        {
            var beliefId = GetBeliefIdFromKnowledgeId(knowledgeId);

            return FilterBeliefToSend(beliefId, beliefBit, medium);
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
        public Bits FilterBeliefToSend(IAgentId beliefId, byte beliefBit, CommunicationTemplate medium)
        {
            if (medium is null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            // If don't have belief, can't send belief or no belief asked
            if (!_messageContent.CanSendBeliefs || !On)
            {
                return null;
            }

            // intentionally after Cognitive.MessageContent.CanSendBeliefs test
            if (Beliefs == null)
            {
                throw new NullReferenceException(nameof(Beliefs));
            }

            // If Agent don't have the belief, he can't reply
            if (!BelievesEnough(beliefId, beliefBit,
                _messageContent.MinimumBeliefToSendPerBit))
            {
                return null;
            }

            var agentBelief = _actorBeliefNetwork.Edge<ActorBelief>(_agentId, beliefId);
            if (agentBelief is null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            // Filter the belief to send, via the good communication channel// Filtering via the good channel
            var minBits = Math.Max(_messageContent.MinimumNumberOfBitsOfBeliefToSend,
                medium.MinimumNumberOfBitsOfBeliefToSend);
            var maxBits = Math.Min(_messageContent.MaximumNumberOfBitsOfBeliefToSend,
                medium.MaximumNumberOfBitsOfBeliefToSend);
            var minKnowledge = Math.Max(_messageContent.MinimumBeliefToSendPerBit,
                medium.MinimumBeliefToSendPerBit);
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

        /// <summary>
        ///     Check that agent has the BeliefId[knowledgeBit] == 1
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBit"></param>
        /// <param name="beliefThreshHoldForAnswer"></param>
        /// <returns>true if the agent has the knowledge</returns>
        public bool BelievesEnough(IAgentId beliefId, byte beliefBit, float beliefThreshHoldForAnswer)
        {
            if (!_actorBeliefNetwork.Exists(_agentId,beliefId))
            {
                return false;
            }

            var belief = _actorBeliefNetwork.Edge<ActorBelief>(_agentId,beliefId);
            return belief.BelievesEnough(beliefBit, beliefThreshHoldForAnswer);
        }

        /// <summary>
        ///     Get the sum of all the beliefs
        /// </summary>
        /// <returns></returns>
        public float GetBeliefsSum()
        {
            if (! On || Beliefs == null)
            {
                return 0;
            }
            return _actorBeliefNetwork.EdgesFilteredBySource<ActorBelief>(_agentId).Sum(l => l.GetBeliefSum());
        }

        /// <summary>
        ///     Get the maximum potential of all the beliefs
        /// </summary>
        /// <returns></returns>
        public float GetBeliefsPotential()
        {
            if (Beliefs == null)
            {
                return 0;
            }
            return _actorBeliefNetwork.EdgesFilteredBySource<ActorBelief>(_agentId).Sum(l => l.GetBeliefPotential());
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process
        /// </summary>
        /// <param name="neutral"></param>
        public void InitializeBeliefs(bool neutral)
        {
            if (!On)
            {
                return;
            }
                
            foreach (var agentBelief in _actorBeliefNetwork.EdgesFilteredBySource<ActorBelief>(_agentId) )
            {
                InitializeActorBelief(agentBelief, neutral);
            }
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process based on the agent belief level
        /// </summary>
        /// <param name="actorBelief">agentBelief to initialize</param>
        /// <param name="neutral">if !HasInitialBelief, then a neutral initialization is done</param>
        public void InitializeActorBelief(ActorBelief actorBelief, bool neutral)
        {
            if (actorBelief == null)
            {
                throw new ArgumentNullException(nameof(actorBelief));
            }

            var belief = GetBelief(actorBelief.Target);
            if (belief == null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            var level = neutral ? BeliefLevel.NoBelief : actorBelief.BeliefLevel;
            var beliefBits = belief.InitializeBits(_model, level);
            actorBelief.SetBeliefBits(beliefBits);
        }

        /// <summary>
        ///     agent learn beliefId with a weight of influenceability * influentialness
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBits"></param>
        /// <param name="influenceWeight"></param>
        /// <param name="beliefLevel"></param>
        public void Learn(IAgentId beliefId, Bits beliefBits, float influenceWeight,
            BeliefLevel beliefLevel)
        {
            LearnNewBelief(beliefId, beliefLevel);
            _actorBeliefNetwork.Edge<ActorBelief>(_agentId, beliefId).Learn(beliefBits, influenceWeight);
        }

        /// <summary>
        ///     Agent don't have still this belief, it's time to learn a new one
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefLevel"></param>
        public void LearnNewBelief(IAgentId beliefId, BeliefLevel beliefLevel)
        {
            if (_actorBeliefNetwork.Exists(_agentId, beliefId))
            {
                return;
            }

            var actorBelief = new ActorBelief(_agentId, beliefId, beliefLevel);
            _actorBeliefNetwork.Add(actorBelief);
            InitializeBeliefs(true);
        }
    }
}