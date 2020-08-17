#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace Symu.Messaging.Templates
{
    /// <summary>
    ///     Clone all the CognitiveArchitecture parameters for the CommunicationChannels
    ///     base class for all the communication channels
    /// </summary>
    public class CommunicationTemplate //: CognitiveArchitectureTemplate
    {
        private byte _maximumNumberOfBitsOfKnowledgeToSend = 1;
        private float _maxRateLearnable = 1;

        private float _minimumBeliefToSendPerBit = 0.35F;
        private float _minimumKnowledgeToSendPerBit = 0.35F;

        private byte _minimumNumberOfBitsOfKnowledgeToSend = 1;

        private short _timeToLive = -1;

        /// <summary>
        ///     Maximum rate learnable the message can be
        ///     Range [0;1]
        /// </summary>
        /// <example>a phone call can be less learnable than an email because you can forget easier</example>
        /// <example>If 0, nothing is learnable from the message</example>
        /// <example>If 1, everything is learnable from the message</example>
        public float MaxRateLearnable
        {
            get => _maxRateLearnable;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MaxRateLearnable should be between 0 and 1");
                }

                _maxRateLearnable = value;
            }
        }

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

        public bool CanReceiveBeliefs { get; set; }

        /// <summary>
        ///     To send beliefs, an agent must have enough beliefs per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent BeliefId[index] = 0.6 => he can answer to the question</example>
        /// <remarks>Default value = Min value of the level Foundational</remarks>
        public float MinimumBeliefToSendPerBit
        {
            get => _minimumBeliefToSendPerBit;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinimumBeliefToSendPerBit should be between 0 and 1");
                }

                _minimumBeliefToSendPerBit = value;
            }
        }

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfBeliefToSend { get; set; } = 1;

        /// <summary>
        ///     To send Knowledge, an agent must have enough knowledge per KnowledgeBits
        ///     [0 - 1]
        /// </summary>
        /// <remarks>Default value = Min value of the level foundational</remarks>
        /// <example>if KnowledgeThreshHoldForAnswer = 0.5 and agent KnowledgeId[index] = 0.6 => he can answer to the question</example>
        public float MinimumKnowledgeToSendPerBit
        {
            get => _minimumKnowledgeToSendPerBit;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MinimumKnowledgeToSendPerBit should be between 0 and 1");
                }

                _minimumKnowledgeToSendPerBit = value;
            }
        }

        /// <summary>
        ///     The minimum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MinimumNumberOfBitsOfKnowledgeToSend
        {
            get => _minimumNumberOfBitsOfKnowledgeToSend;
            set
            {
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("MinimumNumberOfBitsOfKnowledgeToSend should be <= " +
                                                          Bits.MaxBits);
                }

                _minimumNumberOfBitsOfKnowledgeToSend = value;
            }
        }

        /// <summary>
        ///     The maximum number of non zero Bits of Knowledge to send back during an interaction (message)
        /// </summary>
        public byte MaximumNumberOfBitsOfKnowledgeToSend
        {
            get => _maximumNumberOfBitsOfKnowledgeToSend;
            set
            {
                if (value > Bits.MaxBits)
                {
                    throw new ArgumentOutOfRangeException("MaximumNumberOfBitsOfKnowledgeToSend should be <= " +
                                                          Bits.MaxBits);
                }

                _maximumNumberOfBitsOfKnowledgeToSend = value;
            }
        }

        /// <summary>
        ///     Cost : time spent to send an message
        ///     Range [0;1]
        /// </summary>
        /// <example>time spent to write an email</example>
        public GenericLevel CostToSendLevel { get; set; } = GenericLevel.Medium;

        /// <summary>
        ///     Cost : time spent to read an message
        ///     Range [0;1]
        /// </summary>
        /// <example>time spent to read an email</example>
        public GenericLevel CostToReceiveLevel { get; set; } = GenericLevel.Medium;

        public float CostToSend(byte random)
        {
            return Cost(CostToSendLevel, random);
        }

        public float CostToReceive(byte random)
        {
            return Cost(CostToReceiveLevel, random);
        }

        public static float Cost(GenericLevel level, byte random)
        {
            float cost;
            switch (level)
            {
                case GenericLevel.None:
                    return 0;
                case GenericLevel.VeryLow:
                    cost = 0.05F;
                    break;
                case GenericLevel.Low:
                    cost = 0.1F;
                    break;
                case GenericLevel.Medium:
                    cost = 0.15F;
                    break;
                case GenericLevel.High:
                    cost = 0.20F;
                    break;
                case GenericLevel.VeryHigh:
                    cost = 0.25F;
                    break;
                case GenericLevel.Complete:
                    cost = 1;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }

            cost = Normal.Sample(cost, 0.05F * random);
            return cost < 0 ? 0 : cost;
        }

        public void CopyTo(CommunicationTemplate medium)
        {
            if (medium == null)
            {
                throw new ArgumentNullException(nameof(medium));
            }

            medium.CostToSendLevel = CostToSendLevel;
            medium.CostToReceiveLevel = CostToReceiveLevel;
            medium.MaxRateLearnable = MaxRateLearnable;
            medium.TimeToLive = TimeToLive;
            medium.MinimumBeliefToSendPerBit = MinimumBeliefToSendPerBit;
            medium.MinimumNumberOfBitsOfBeliefToSend = MinimumNumberOfBitsOfBeliefToSend;
            medium.MaximumNumberOfBitsOfBeliefToSend = MaximumNumberOfBitsOfBeliefToSend;
            medium.MaximumNumberOfBitsOfKnowledgeToSend = MaximumNumberOfBitsOfKnowledgeToSend;
            medium.MinimumNumberOfBitsOfKnowledgeToSend = MinimumNumberOfBitsOfKnowledgeToSend;
            medium.MinimumKnowledgeToSendPerBit = MinimumKnowledgeToSendPerBit;
        }
    }
}