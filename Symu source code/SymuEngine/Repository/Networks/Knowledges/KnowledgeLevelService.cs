﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;

#endregion

namespace SymuEngine.Repository.Networks.Knowledges
{
    /// <summary>
    ///     A utility to easily switch from KnowledgeLevel to values
    /// </summary>
    public static class KnowledgeLevelService
    {
        /// <summary>
        ///     Get all names of the KnowledgeLevel enum
        /// </summary>
        /// <returns></returns>
        public static string[] GetNames()
        {
            return Enum.GetNames(typeof(KnowledgeLevel)).ToArray();
        }

        /// <summary>
        ///     Get the value based on the GenericLevel name
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static KnowledgeLevel GetValue(string level)
        {
            switch (level)
            {
                case "NoKnowledge":
                    return KnowledgeLevel.NoKnowledge;
                case "BasicKnowledge":
                    return KnowledgeLevel.BasicKnowledge;
                case "Foundational":
                    return KnowledgeLevel.Foundational;
                case "Intermediate":
                    return KnowledgeLevel.Intermediate;
                case "FullProficiency":
                    return KnowledgeLevel.FullProficiency;
                case "Expert":
                    return KnowledgeLevel.Expert;
                case "FullKnowledge":
                    return KnowledgeLevel.FullKnowledge;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Get the name of a KnowledgeLevel
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string GetName(KnowledgeLevel level)
        {
            switch (level)
            {
                case KnowledgeLevel.NoKnowledge:
                    return "NoKnowledge";
                case KnowledgeLevel.BasicKnowledge:
                    return "BasicKnowledge";
                case KnowledgeLevel.Foundational:
                    return "Foundational";
                case KnowledgeLevel.Intermediate:
                    return "Intermediate";
                case KnowledgeLevel.FullProficiency:
                    return "FullProficiency";
                case KnowledgeLevel.Expert:
                    return "Expert";
                case KnowledgeLevel.FullKnowledge:
                    return "FullKnowledge";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}