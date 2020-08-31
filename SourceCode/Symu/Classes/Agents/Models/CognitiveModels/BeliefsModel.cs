#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Common;
using Symu.Common.Classes;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.DNA.Beliefs;
using Symu.DNA.Knowledges;
using Symu.Messaging.Templates;
using Symu.Repository.Entity;
using Symu.Repository.Networks;
using static Symu.Common.Constants;

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
        private readonly BeliefNetwork _networkBeliefs;

        /// <summary>
        ///     Initialize Beliefs model :
        ///     update NetworkBeliefs
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="entity"></param>
        /// <param name="cognitiveArchitecture"></param>
        /// <param name="network"></param>
        /// <param name="model"></param>
        public BeliefsModel(IAgentId agentId, ModelEntity entity, CognitiveArchitecture cognitiveArchitecture,
            SymuMetaNetwork network, RandomGenerator model)
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
            _networkBeliefs = network.Beliefs;
            _knowledgeAndBeliefs = cognitiveArchitecture.KnowledgeAndBeliefs;
            _messageContent = cognitiveArchitecture.MessageContent;
            _model = model;
        }

        public bool On { get; set; }

        /// <summary>
        ///     Get the agent Beliefs
        /// </summary>
        public AgentBeliefs Beliefs =>
            On && _knowledgeAndBeliefs.HasBelief ? _networkBeliefs.GetAgentBeliefs(_agentId) : null;

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

            InitializeBeliefs(!_knowledgeAndBeliefs.HasInitialBelief);
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

            AddBeliefs(expertiseAgent, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }
        public void AddBeliefs(AgentExpertise expertise, BeliefLevel beliefLevel)
        {
            if (expertise is null)
            {
                throw new ArgumentNullException(nameof(expertise));
            }

            _networkBeliefs.AddAgentId(_agentId);

            foreach (var agentBelief in expertise.List.Select(agentKnowledge => new AgentBelief(agentKnowledge.KnowledgeId, beliefLevel)))
            {
                _networkBeliefs.AddBelief(_agentId, agentBelief);
            }
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the DefaultBeliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void AddBelief(IId knowledgeId)
        {
            AddBelief(knowledgeId, _knowledgeAndBeliefs.DefaultBeliefLevel);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        ///     using the beliefLevel
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="beliefLevel"></param>
        public void AddBelief(IId knowledgeId, BeliefLevel beliefLevel)
        {
            if (!_knowledgeAndBeliefs.HasBelief || !On)
            {
                return;
            }

            var agentBelief = new AgentBelief(knowledgeId, beliefLevel);
            _networkBeliefs.Add(_agentId, agentBelief);
        }

        public AgentBelief GetAgentBelief(IId beliefId)
        {
            return Beliefs.GetAgentBelief<AgentBelief>(beliefId);
        }

        public Belief GetBelief(IId beliefId)
        {
            return _networkBeliefs.GetBelief<Belief>(beliefId);
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
        public Bits FilterBeliefToSend(IId beliefId, byte beliefBit, CommunicationTemplate medium)
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
            if (!BelievesEnough(beliefId, beliefBit,
                _messageContent.MinimumBeliefToSendPerBit))
            {
                return null;
            }

            var agentBelief = Beliefs.GetAgentBelief<AgentBelief>(beliefId);
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
        public bool BelievesEnough(IId beliefId, byte beliefBit, float beliefThreshHoldForAnswer)
        {
            if (!Beliefs.Contains(beliefId))
            {
                return false;
            }

            var belief = GetAgentBelief(beliefId);
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
            return Beliefs.GetAgentBeliefs<AgentBelief>().Sum(l => l.GetBeliefSum());
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
            return Beliefs.GetAgentBeliefs<AgentBelief>().Sum(l => l.GetBeliefPotential());
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process
        /// </summary>
        /// <param name="neutral"></param>
        public void InitializeBeliefs(bool neutral)
        {
            if (!_networkBeliefs.Exists(_agentId))
            {
                throw new NullReferenceException(nameof(_agentId));
            }

            foreach (var agentBelief in Beliefs.GetAgentBeliefs<AgentBelief>())
            {
                InitializeAgentBelief(agentBelief, neutral);
            }
        }

        /// <summary>
        ///     Initialize AgentBelief with a stochastic process based on the agent belief level
        /// </summary>
        /// <param name="agentBelief">agentBelief to initialize</param>
        /// <param name="neutral">if !HasInitialBelief, then a neutral initialization is done</param>
        public void InitializeAgentBelief(AgentBelief agentBelief, bool neutral)
        {
            if (agentBelief == null)
            {
                throw new ArgumentNullException(nameof(agentBelief));
            }

            var belief = GetBelief(agentBelief.BeliefId);
            if (belief == null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            var level = neutral ? BeliefLevel.NoBelief : agentBelief.BeliefLevel;
            var beliefBits = belief.InitializeBits(_model, level);
            agentBelief.SetBeliefBits(beliefBits);
        }

        /// <summary>
        ///     agent learn beliefId with a weight of influenceability * influentialness
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBits"></param>
        /// <param name="influenceWeight"></param>
        /// <param name="beliefLevel"></param>
        public void Learn(IId beliefId, Bits beliefBits, float influenceWeight,
            BeliefLevel beliefLevel)
        {
            LearnNewBelief(beliefId, beliefLevel);
            _networkBeliefs.GetAgentBelief<AgentBelief>(_agentId, beliefId).Learn(beliefBits, influenceWeight);
        }

        /// <summary>
        ///     Agent don't have still this belief, it's time to learn a new one
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefLevel"></param>
        public void LearnNewBelief(IId beliefId, BeliefLevel beliefLevel)
        {
            if (_networkBeliefs.Exists(_agentId, beliefId))
            {
                return;
            }

            var agentBelief = new AgentBelief(beliefId, beliefLevel);
            _networkBeliefs.Add(_agentId, agentBelief);
            InitializeBeliefs(true);
        }

    }
}