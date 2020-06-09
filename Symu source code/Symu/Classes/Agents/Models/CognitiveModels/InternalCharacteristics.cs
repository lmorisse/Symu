#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
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
        #region Learning

        /// <summary>
        ///     This parameter specify whether agents of this class can learn new knowledge.
        ///     If set to true, agent will use the Learning Model
        /// </summary>
        public bool CanLearn { get; set; }

        #endregion

        public void CopyTo(InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            #region Learning

            internalCharacteristics.CanLearn = CanLearn;

            #endregion

            #region Risk aversion

            internalCharacteristics.RiskAversionThreshold = RiskAversionThreshold;

            #endregion

            #region Forgetting

            internalCharacteristics.CanForget = CanForget;
            internalCharacteristics.ForgettingMean = ForgettingMean;
            internalCharacteristics.ForgettingStandardDeviation = ForgettingStandardDeviation;
            internalCharacteristics.PartialForgetting = PartialForgetting;
            internalCharacteristics.PartialForgettingRate = PartialForgettingRate;
            internalCharacteristics.ForgettingSelectingMode = ForgettingSelectingMode;
            internalCharacteristics.MinimumRemainingKnowledge = MinimumRemainingKnowledge;
            internalCharacteristics.TimeToLive = TimeToLive;

            #endregion

            #region Influence

            internalCharacteristics.CanInfluenceOrBeInfluence = CanInfluenceOrBeInfluence;
            internalCharacteristics.InfluenceabilityRateMax = InfluenceabilityRateMax;
            internalCharacteristics.InfluenceabilityRateMin = InfluenceabilityRateMin;
            internalCharacteristics.InfluentialnessRateMax = InfluentialnessRateMax;
            internalCharacteristics.InfluentialnessRateMin = InfluentialnessRateMin;

            #endregion
        }

        #region Forgetting

        /// <summary>
        ///     This parameter specify whether agents of this class can forget knowledge
        ///     If set to true, agent will use the Forgetting Model
        /// </summary>
        public bool CanForget { get; set; }

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
        /// <remarks>the default is set to be > Murphies.IncompleteKnowledge.KnowledgeThreshHoldForReacting</remarks>
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
        ///     This parameter specify whether agents of this class can influence others or be influence by others
        ///     If set to true, agent will use the Influence Model
        /// </summary>
        public bool CanInfluenceOrBeInfluence { get; set; }

        private float _influentialnessRateMin;

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how inﬂuential an agent of this class will be.
        ///     Inﬂuentialness rates close to zero will mean that the agent will not have much sway over other agents,
        ///     while values closer to one will mean that the agent has a signiﬁcant amount of inﬂuence.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluentialnessRateMin
        {
            get => _influentialnessRateMin;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InfluentialnessRateMin should be between 0 and 1");
                }

                _influentialnessRateMin = value;
            }
        }

        private float _influentialnessRateMax;

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how inﬂuential an agent of this class will be.
        ///     Inﬂuentialness rates close to zero will mean that the agent will not have much sway over other agents,
        ///     while values closer to one will mean that the agent has a signiﬁcant amount of inﬂuence.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluentialnessRateMax
        {
            get => _influentialnessRateMax;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InfluentialnessRateMax should be between 0 and 1");
                }

                _influentialnessRateMax = value;
            }
        }

        private float _influenceabilityRateMin;

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how susceptible an agent of this class will be
        ///     to the inﬂuentialness of another agent.
        ///     Inﬂuenceability values close to zero will mean that an agent is extremely independent and unlikely to change its
        ///     beliefs,
        ///     while values close to one indicate that an agent is very impressionable.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluenceabilityRateMin
        {
            get => _influenceabilityRateMin;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InfluenceabilityRateMin should be between 0 and 1");
                }

                _influenceabilityRateMin = value;
            }
        }

        private float _influenceabilityRateMax;

        /// <summary>
        ///     This parameter, speciﬁed as a range with a min and a max, specify how susceptible an agent of this class will be
        ///     to the inﬂuentialness of another agent.
        ///     Inﬂuenceability values close to zero will mean that an agent is extremely independent and unlikely to change its
        ///     beliefs,
        ///     while values close to one indicate that an agent is very impressionable.
        /// </summary>
        /// <remarks>range [0;1]</remarks>
        public float InfluenceabilityRateMax
        {
            get => _influenceabilityRateMax;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("InfluenceabilityRateMax should be between 0 and 1");
                }

                _influenceabilityRateMax = value;
            }
        }

        #endregion

        #region Attention

        #endregion

        #region Risk aversion

        private float _riskAversionThreshold;

        /// <summary>
        ///     The risk aversion parameter affects whether or not an agent can make a particular decision.
        ///     Agents can accumulate beliefs during the simulation and in the absence of risk aversion will act upon these
        ///     beliefs.
        ///     The risk aversion parameter, however, acts as a catch-all for factors not included in the model which prevent an
        ///     agent from acting on a particular belief.
        ///     Agents who are risk averse will still be able to communicate their knowledge and beliefs like any other agent, but
        ///     will never be able to make the corresponding decision.
        ///     RiskAversionThreshold == 0, full risk aversion
        ///     RiskAversionThreshold == 1, no risk aversion
        ///     RiskAversionThreshold should be > Environment.Organization.Murphies.IncompleteBelief.ThresholdForReacting
        ///     Range [0;1]
        /// </summary>
        public float RiskAversionThreshold
        {
            get => _riskAversionThreshold;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("RiskAversionThreshold should be between 0 and 1");
                }

                _riskAversionThreshold = value;
            }
        }

        #endregion

        #region Socio demographics

        #endregion
    }
}