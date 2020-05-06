#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SymuEngine.Classes.Agent;
using SymuTools.Algorithm;

#endregion

namespace SymuEngine.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Knowledge matrix
    ///     use to compute metrics based on Knowledges and NetworkKnowledges
    /// </summary>
    /// <example></example>
    public static class KnowledgeMatrix
    {
        /// <summary>
        ///     Transform networkKnowledge into a dense matrix (filled with 0 & 1)
        /// </summary>
        /// <param name="knowledges">Column</param>
        /// <param name="actors">Row</param>
        /// <param name="knowledgeNetwork"></param>
        /// <returns>Knowledges x Agents matrix</returns>
        public static sbyte[,] GetMatrixKnowledge(KnowledgeCollection knowledges, IEnumerable<AgentId> actors,
            NetworkKnowledges knowledgeNetwork)
        {
            #region Null

            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            if (actors is null)
            {
                throw new ArgumentNullException(nameof(actors));
            }

            if (knowledgeNetwork is null)
            {
                throw new ArgumentNullException(nameof(knowledgeNetwork));
            }

            #endregion

            var agentIds = actors.ToList();
            var actorsCount = agentIds.Count();
            var arrays = new sbyte[actorsCount, knowledges.Count];
            for (var i = 0; i < actorsCount; i++)
            for (var j = 0; j < knowledges.Count; j++)
            {
                if (knowledgeNetwork.Exists(agentIds.ElementAt(i), knowledges.List[j].Id))
                {
                    arrays[i, j] = 1;
                }
                else
                {
                    arrays[i, j] = 0;
                }
            }

            return arrays;
        }

        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     the actor i knows the fact k or not
        /// </summary>
        /// <param name="knowledges"></param>
        /// <param name="actors"></param>
        /// <param name="knowledgeNetwork"></param>
        /// <returns></returns>
        public static float[,] GetPassiveInteractionMatrix(KnowledgeCollection knowledges, IEnumerable<AgentId> actors,
            NetworkKnowledges knowledgeNetwork)
        {
            #region Null

            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            if (actors is null)
            {
                throw new ArgumentNullException(nameof(actors));
            }

            if (knowledgeNetwork is null)
            {
                throw new ArgumentNullException(nameof(knowledgeNetwork));
            }

            #endregion

            var agentIds = actors.ToList();
            var matrixKnowledge = GetMatrixKnowledge(knowledges, agentIds, knowledgeNetwork);

            var actorsCount = agentIds.Count();
            var arrays = new float[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            {
                float denominator = 0;
                for (var j = 0; j < actorsCount; j++)
                for (var k = 0; k < knowledges.Count; k++)
                {
                    denominator += matrixKnowledge[i, k] * matrixKnowledge[j, k];
                }

                for (var j = 0; j < actorsCount; j++)
                {
                    if (denominator > 0)
                    {
                        float numerator = 0;
                        for (var k = 0; k < knowledges.Count; k++)
                        {
                            numerator += matrixKnowledge[i, k] * matrixKnowledge[j, k];
                        }

                        arrays[i, j] = numerator / denominator;
                    }
                    else
                    {
                        arrays[i, j] = 0;
                    }
                }
            }

            return arrays;
        }

        /// <summary>
        ///     Combinations without repeating triplets
        /// </summary>
        /// <param name="agentsCount"></param>
        /// <returns></returns>

        //TODO refactor to another class
        public static uint MaxTriads(int agentsCount)
        {
            if (agentsCount < 3)
            {
                return 0;
            }

            if (agentsCount == 3)
            {
                return 1;
            }

            var arrangements = SpecialFunctions.Factorial(agentsCount) / SpecialFunctions.Factorial(agentsCount - 3);
            uint max = 0;
            try
            {
                max = Convert.ToUInt32(Math.Round(arrangements / SpecialFunctions.Factorial(3)));
            }
            catch (OverflowException)
            {
            }

            return max;
        }

        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     The actor i does not know the information k, but j does
        ///     only if the actor has an active interaction style
        /// </summary>
        /// <param name="knowledges"></param>
        /// <param name="actors"></param>
        /// <param name="actorInteractionsSytles"></param>
        /// <param name="knowledgeNetwork"></param>
        /// <returns></returns>
        public static float[,] GetActiveInteractionMatrix(KnowledgeCollection knowledges, IEnumerable<AgentId> actors,
            List<bool> actorInteractionsSytles, NetworkKnowledges knowledgeNetwork)
        {
            #region Null

            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            if (actors is null)
            {
                throw new ArgumentNullException(nameof(actors));
            }

            if (actorInteractionsSytles is null)
            {
                throw new ArgumentNullException(nameof(actorInteractionsSytles));
            }

            if (knowledgeNetwork is null)
            {
                throw new ArgumentNullException(nameof(knowledgeNetwork));
            }

            #endregion

            var agentIds = actors.ToList();
            var matrixKnowledge = GetMatrixKnowledge(knowledges, agentIds, knowledgeNetwork);
            var actorsCount = agentIds.Count();
            var arrays = new float[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            {
                if (actorInteractionsSytles[i])
                {
                    float denominator = 0;
                    for (var j = 0; j < actorsCount; j++)
                    for (var k = 0; k < knowledges.Count; k++)
                    {
                        if (matrixKnowledge[i, k] == 0 && matrixKnowledge[j, k] == 1)
                        {
                            denominator += 1;
                        }
                    }

                    for (var j = 0; j < actorsCount; j++)
                    {
                        if (denominator > 0)
                        {
                            float numerator = 0;
                            for (var k = 0; k < knowledges.Count; k++)
                                // The goal of getting new information
                            {
                                if (matrixKnowledge[i, k] == 0 && matrixKnowledge[j, k] == 1)
                                {
                                    numerator += 1;
                                }
                            }

                            arrays[i, j] = numerator / denominator;
                        }
                        else
                        {
                            arrays[i, j] = 0;
                        }
                    }
                }
                else
                {
                    // Actor is passive
                    for (var j = 0; j < actorsCount; j++)
                    {
                        arrays[i, j] = 0;
                    }
                }
            }

            return arrays;
        }

        //TODO refactor to another class
        public static uint NumberOfTriads(KnowledgeCollection knowledges, IEnumerable<AgentId> actors,
            NetworkKnowledges knowledgeNetwork)
        {
            #region Null

            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            if (actors is null)
            {
                throw new ArgumentNullException(nameof(actors));
            }

            if (knowledgeNetwork is null)
            {
                throw new ArgumentNullException(nameof(knowledgeNetwork));
            }

            #endregion

            var agentIds = actors.ToList();
            var actorsCount = agentIds.Count();
            if (actorsCount < 3)
            {
                return 0;
            }

            var averageInteraction = GetAverageInteractionMatrix(knowledges, agentIds, knowledgeNetwork);
            var interactionMatrix = GetPassiveInteractionMatrix(knowledges, agentIds, knowledgeNetwork);
            var interactionForTriads = new sbyte[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            for (var j = 0; j < actorsCount; j++)
            {
                if (i != j && interactionMatrix[i, j] >= averageInteraction)
                {
                    interactionForTriads[i, j] = 1;
                }
                else
                {
                    interactionForTriads[i, j] = 0;
                }
            }

            uint numberOfTriads = 0;

            for (var i = 0; i < actorsCount - 2; i++)
            {
                if (interactionForTriads[i, i + 1] + interactionForTriads[i + 1, i] > 0
                    && interactionForTriads[i, i + 2] + interactionForTriads[i + 2, i] > 0
                    && interactionForTriads[i + 1, i + 2] + interactionForTriads[i + 2, i + 1] > 0)
                {
                    numberOfTriads++;
                }
            }

            return numberOfTriads;
        }

        public static float GetAverageInteractionMatrix(KnowledgeCollection knowledges, IEnumerable<AgentId> actors,
            NetworkKnowledges knowledgeNetwork)
        {
            #region Null

            if (knowledges is null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            if (actors is null)
            {
                throw new ArgumentNullException(nameof(actors));
            }

            if (knowledgeNetwork is null)
            {
                throw new ArgumentNullException(nameof(knowledgeNetwork));
            }

            #endregion

            var agentIds = actors.ToList();
            var interactionMatrix = GetPassiveInteractionMatrix(knowledges, agentIds, knowledgeNetwork);

            var actorsCount = agentIds.Count();
            if (actorsCount == 0 || actorsCount == 1)
            {
                return 0;
            }

            float average = 0;
            for (var i = 0; i < agentIds.Count(); i++)
            for (var j = 0; j < agentIds.Count(); j++)
            {
                if (i != j)
                {
                    average += interactionMatrix[i, j];
                }
            }

            return average / (actorsCount * (actorsCount - 1));
        }
    }
}