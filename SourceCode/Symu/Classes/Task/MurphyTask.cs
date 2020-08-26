#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Murphies;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Entity;
using static Symu.Common.Constants;

#endregion

namespace Symu.Classes.Task
{
    /// <summary>
    ///     Model task's murphies
    /// </summary>
    public class MurphyTask : Murphy
    {
        public const float NoRequiredBits = 0;
        public const float FullRequiredBits = 2;

        private float _mandatoryRatio = 0.2F;

        private float _requiredRatio = 0.2F;

        /// <summary>
        ///     Mandatory bit ratio is a function of the task complexity
        ///     The more complex, the more bits are mandatory
        ///     Complexity has a normalized range of [0; 1]
        ///     Mandatory ratio denormalized complexity
        /// </summary>
        /// <example>
        ///     KnowledgeBits = 100 with a MandatoryRatio of 0.2 and a complexity of 1 => number of mandatory bits = 1* 0.2
        ///     *100 = 20 bits
        /// </example>
        public float MandatoryRatio
        {
            get => _mandatoryRatio;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("MandatoryRatio should be between 0 and 1");
                }

                _mandatoryRatio = value;
            }
        }

        /// <summary>
        ///     required bit ratio is a function of the task complexity
        ///     The more complex, the more bits are required
        ///     Complexity has a normalized range of [0; 1]
        ///     Required ratio denormalized complexity
        /// </summary>
        /// <example>
        ///     KnowledgeBits = 100 with a RequiredRatio of 0.2 and a complexity of 1 => number of required bits = 1* 0.2
        ///     *100 = 20 bits
        /// </example>
        public float RequiredRatio
        {
            get => _requiredRatio;
            set
            {
                if (value < 0 || value > 1)
                {
                    throw new ArgumentOutOfRangeException("RequiredRatio should be between 0 and 1");
                }

                _requiredRatio = value;
            }
        }

        /// <summary>
        ///     Define the ratio of Knowledge's size to define the KnowledgeBits required
        ///     (guessing is okay if agent hasn't the knowledge bit) to complete a task
        ///     level Range [0;1]
        /// </summary>
        /// <returns>bits ratio range[0; 0.2F]</returns>
        public float RequiredBitsRatio(float level)
        {
            // For unit tests
            if (Math.Abs(level - FullRequiredBits) < Tolerance)
            {
                return 1;
            }

            // Normal use
            return level * RequiredRatio;
        }

        /// <summary>
        ///     Define the ratio of Knowledge's size to define the KnowledgeBits mandatory to complete a task
        ///     Range [0;1]
        /// </summary>
        public float MandatoryBitsRatio(float level)
        {
            // For unit tests
            if (Math.Abs(level - FullRequiredBits) < Tolerance)
            {
                return 1;
            }

            // Normal use
            return level * MandatoryRatio;
        }
        /// <summary>
        ///     Given a KnowledgeModel and a KnowledgeLevel
        ///     return the required knowledgeBits index for the task: an array fill of random index of the GetBits
        ///     representing the detailed knowledge of an agent
        /// </summary>
        /// <param name="knowledge">knowledge to define the Required Bits Ratio compared to the length of the knowledgeBits</param>
        /// <param name="complexity">complexity of the task from 0 to 1</param>
        /// <returns></returns>
        /// <example>
        ///     given a knowledge of size 10 :
        ///     InitializeBits = [0110110000]
        ///     complexity = 0.3
        ///     GetTaskRequiredBits = [359] : index 3, 5 and 9 are required
        /// </example>
        public byte[] GetTaskRequiredBits(Knowledge knowledge, float complexity)
        {
            if (knowledge is null || knowledge.Length == 0)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var numberRequiredBits = Convert.ToByte(Math.Round(RequiredBitsRatio(complexity) * knowledge.Length));
            return DiscreteUniform.SamplesToByte(numberRequiredBits, knowledge.Length - 1);
        }

        /// <summary>
        ///     Given a KnowledgeModel and a KnowledgeLevel
        ///     return the mandatory knowledgeBits index for the task: an array fill of random index of the GetBits
        ///     representing the detailed knowledge of an agent
        /// </summary>
        /// <param name="knowledge">knowledge to define the mandatory Bits Ratio compared to the length of the knowledgeBits</param>
        /// <param name="complexity">complexity of the task from 0 to 1</param>
        /// <returns></returns>
        /// <example>
        ///     given a knowledge of size 10 :
        ///     InitializeBits = [0110110000]
        ///     complexity = 0.2
        ///     GetTaskMandatoryBits = [17] : index 1 and 7 are mandatory
        /// </example>
        public byte[] GetTaskMandatoryBits(Knowledge knowledge, float complexity)
        {
            if (knowledge is null || knowledge.Length == 0)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            var numberMandatoryBits = Convert.ToByte(Math.Round(MandatoryBitsRatio(complexity) * knowledge.Length));
            return DiscreteUniform.SamplesToByte(numberMandatoryBits, knowledge.Length - 1);
        }
    }
}