#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Common;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Knowledges;
using SymuTools.ProbabilityDistributions;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     InternalCharacteristics from Construct Software
    ///     Influentialness, influenceability
    ///     attention
    ///     forgetting
    ///     risk aversion
    ///     socio demographics
    /// </summary>
    /// <remarks>Knowledge & Beliefs from Construct Software</remarks>
    public class InternalCharacteristics
    {
        private readonly AgentId _id;
        private readonly Network _network;

        public InternalCharacteristics(Network network, AgentId id)
        {
            _network = network;
            _id = id;
        }

        public void CopyTo(InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            #region Forgetting

            internalCharacteristics.ForgettingMean = ForgettingMean;
            internalCharacteristics.ForgettingStandardDeviation = ForgettingStandardDeviation;
            internalCharacteristics.PartialForgetting = PartialForgetting;
            internalCharacteristics.PartialForgettingRate = PartialForgettingRate;
            internalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode;
            internalCharacteristics.MinimumRemainingKnowledge = MinimumRemainingKnowledge;

            #endregion

            #region Influence

            internalCharacteristics.InfluenceabilityRateMax = InfluenceabilityRateMax;
            internalCharacteristics.InfluenceabilityRateMin = InfluenceabilityRateMin;
            internalCharacteristics.InfluentialnessRateMax = InfluentialnessRateMax;
            internalCharacteristics.InfluentialnessRateMin = InfluentialnessRateMin;

            #endregion
        }

        #region Forgetting

        private float _forgettingMean;

        /// <summary>
        ///     Define the mean probability an agent will forget a bit of knowledge
        ///     It impacts the KnowledgeBits of the Agent
        ///     range[0;1]
        /// </summary>
        public float ForgettingMean
        {
            get => _forgettingMean;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("ForgettingMean should be between 0 and 1");
                }

                _forgettingMean = value;
            }
        }

        /// <summary>
        ///     Standard deviation around the ForgettingMean and ForgettingMean
        ///     Default 0.1F
        /// </summary>
        public GenericLevel ForgettingStandardDeviation { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     A particular bit of Knowledge score can be decreased if it is not used by the stochastic forgetting process
        ///     if false, the score is binary 1 or 0
        ///     The forgetting process will set the particular bit to 0
        /// </summary>
        public bool PartialForgetting { get; set; } = true;

        private float _partialForgettingRate = 0.05F;

        /// <summary>
        ///     the numerical reduction in knowledge if the bit is to be effected by the stochastic forgetting process
        ///     It impacts the KnowledgeBits of the Agent
        /// </summary>
        /// <example>a knowledgeBit = 0.5 is partially forgotten with a rate of 0.05, it's new value = 0.45</example>
        public float PartialForgettingRate
        {
            get => _partialForgettingRate;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("PartialForgettingRate should be between 0 and 1");
                }

                _partialForgettingRate = value;
            }
        }

        private float _minimumRemainingKnowledge = 0.15F;

        /// <summary>
        ///     When forgetting a knowledge there is a minimum level that is remaining, this is the general culture on the subject
        ///     MinimumRemainingKnowledge set this minimum knowledge that remains
        /// </summary>
        /// <remarks>the default is set to be > Murphies.IncompleteKnowledge.KnowledgeThreshHoldForDoing</remarks>
        /// <example>
        ///     if knowledgeBit = MinimumRemainingKnowledge and the agent is forgetting this knowledge, the knowledgeBit will
        ///     remaining equal
        /// </example>
        public float MinimumRemainingKnowledge
        {
            get => _minimumRemainingKnowledge;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinimumRemainingKnowledge should be between 0 and 1");
                }

                _minimumRemainingKnowledge = value;
            }
        }

        /// <summary>
        ///     Forgetting mode is used to select the knowledges that will be forgotten.
        ///     It can be a random mode or based on the oldest knowledges.LastTouched attributes
        /// </summary>
        public ForgettingSelectingMode ForgettingSelectingMode { get; set; } = ForgettingSelectingMode.Oldest;

        ///// <summary>
        ///// Forgetting is a function of time, which can take different types
        ///// Those types are modelized by a linear function if speed == 0 or inverse functions
        ///// </summary>
        ///// <example>Forgetting speed = 1 => linear function</example>
        ///// <example>Forgetting speed = -0.5 => 1/sqrt(x)</example>
        ///// <example>Forgetting speed = -1 => 1/x</example>
        ///// <example>Forgetting speed = -2 => 1/x2</example>
        //public float ForgettingSpeed { get; set; } = 0;
        private short _timeToLive = -1;

        /// <summary>
        ///     When ForgettingSelectingMode.Oldest is selected, knowledge are forget based on their timeToLive attribute
        ///     -1 for unlimited time to live
        /// </summary>
        public short TimeToLive
        {
            get => _timeToLive;
            set
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException("TimeToLive should be >= -1");
                }

                _timeToLive = value;
            }
        }

        #endregion

        #region Influence

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how inﬂuential an agent of this class will be.
        ///     Inﬂuentialness rates close to zero will mean that the agent will not have much sway over other agents,
        ///     while values closer to one will mean that the agent has a signiﬁcant amount of inﬂuence.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluentialnessRateMin { get; set; }

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how inﬂuential an agent of this class will be.
        ///     Inﬂuentialness rates close to zero will mean that the agent will not have much sway over other agents,
        ///     while values closer to one will mean that the agent has a signiﬁcant amount of inﬂuence.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluentialnessRateMax { get; set; }

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how susceptible an agent of this class will be
        ///     to the inﬂuentialness of another agent.
        ///     Inﬂuenceability values close to zero will mean that an agent is extremely independent and unlikely to change its
        ///     beliefs,
        ///     while values close to one indicate that an agent is very impressionable.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluenceabilityRateMin { get; set; }

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how susceptible an agent of this class will be
        ///     to the inﬂuentialness of another agent.
        ///     Inﬂuenceability values close to zero will mean that an agent is extremely independent and unlikely to change its
        ///     beliefs,
        ///     while values close to one indicate that an agent is very impressionable.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluenceabilityRateMax { get; set; }

        /// <summary>
        ///     Set the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
        ///     InfluentialnessRateMax]
        /// </summary>
        /// <returns></returns>
        public float NextInfluentialness()
        {
            return ContinuousUniform.Sample(InfluentialnessRateMin, InfluentialnessRateMax);
        }

        /// <summary>
        ///     Set the Influentialness for a specific agent with a random value between [InfluentialnessRateMin,
        ///     InfluentialnessRateMax]
        /// </summary>
        /// <returns></returns>
        public float NextInfluenceability()
        {
            return ContinuousUniform.Sample(InfluenceabilityRateMin, InfluenceabilityRateMax);
        }

        /// <summary>
        ///     Learn beliefId from agentId
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="beliefBits">from agentId beliefBits</param>
        /// <param name="agentId"></param>
        public void Learn(ushort beliefId, Bits beliefBits, AgentId agentId)
        {
            if (beliefId == 0 || beliefBits == null)
            {
                return;
            }

            if (beliefId > 0 && beliefBits == null)
            {
                throw new ArgumentNullException(nameof(beliefBits));
            }

            // Learning From agent
            var influentialness = _network.NetworkInfluences.GetInfluentialness(agentId);
            // to Learner
            var influenceability = _network.NetworkInfluences.GetInfluenceability(_id);
            // Learner learn beliefId from agentId with a weight of influenceability * influentialness
            _network.NetworkBeliefs.Learn(_id, beliefId, beliefBits, influenceability * influentialness);
        }

        public void LearnByDoing(ushort beliefId, byte beliefBit)
        {
            if (!_network.NetworkBeliefs.Exists(_id, beliefId))
            {
                _network.NetworkBeliefs.LearnNewBelief(_id, beliefId);
            }

            var agentBelief = _network.NetworkBeliefs.GetAgentBelief(_id, beliefId);
            agentBelief.Learn(_network.NetworkBeliefs.Model, beliefBit);
        }

        #endregion

        #region Attention

        #endregion

        #region Risk aversion

        #endregion

        #region Socio demographics

        #endregion
    }
}