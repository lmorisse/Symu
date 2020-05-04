﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Murphies;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Classes.Task
{
    /// <summary>
    ///     Model task's murphies
    /// </summary>
    public class MurphyTask : Murphy
    {
        public const float noRequiredBits = 0;
        public const float fullRequiredBits = 2;

        /// <summary>
        ///     MandatoryBits = RequiredBits / RequiredMandatoryRatio
        ///     MandatoryBits must be known by the agent
        /// </summary>
        public float RequiredMandatoryRatio { get; set; } = 5;

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
        public float RequiredRatio { get; set; } = 0.2F;

        /// <summary>
        ///     Define the ratio of Knowledge's size to define the KnowledgeBits required
        ///     (guessing is okay if agent hasn't the knowledge bit) to complete a task
        ///     level Range [0;1]
        /// </summary>
        /// <returns>bits ratio range[0; 0.2F]</returns>
        public static float RequiredBitsRatio(float level)
        {
            // For unit tests
            if (Math.Abs(level - fullRequiredBits) < Tolerance)
            {
                return 1;
            }

            // Normal use
            return level * 0.2F;
        }

        /// <summary>
        ///     Define the ratio of Knowledge's size to define the KnowledgeBits mandatory to complete a task
        ///     Range [0;1]
        /// </summary>
        public float MandatoryBitsRatio(float level)
        {
            if (Math.Abs(RequiredMandatoryRatio) < Tolerance)
            {
                return 0;
            }

            return RequiredBitsRatio(level) / RequiredMandatoryRatio;
        }
    }
}