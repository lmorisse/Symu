#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuTools;
using SymuTools.Math;

#endregion

namespace SymuEngine.Repository.Networks.Sphere
{
    /// <summary>
    ///     Knowledge matrix
    ///     use to compute metrics based on Knowledges and NetworkKnowledges
    /// </summary>
    /// <example></example>
    public static class InteractionMatrix
    {
        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     the actor i knows the fact k or not
        /// </summary>
        /// <param name="interactionMatrix"></param>
        /// <returns></returns>
        public static float GetAverageInteractionMatrix(float[,] interactionMatrix)
        {
            if (interactionMatrix == null)
            {
                throw new ArgumentNullException(nameof(interactionMatrix));
            }

            var actorsCount = interactionMatrix.GetLength(0);
            if (actorsCount == 0 || actorsCount == 1)
            {
                return 0;
            }

            float average = 0;
            for (var i = 0; i < actorsCount; i++)
            for (var j = 0; j < actorsCount; j++)
            {
                if (i != j)
                {
                    average += interactionMatrix[i, j];
                }
            }

            return average / (actorsCount * (actorsCount - 1));
        }

        public static uint NumberOfTriads(DerivedParameter[,] network)
        {
            if (network == null)
            {
                return 0;
            }

            var actorsCount = network.GetLength(0);
            if (actorsCount < 3)
            {
                return 0;
            }

            var interactionMatrix = GetInteractionMatrix(network, actorsCount);

            var averageInteraction = GetAverageInteractionMatrix(interactionMatrix);
            var interactionForTriads = new sbyte[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            for (var j = 0; j < actorsCount; j++)
            {
                if (interactionMatrix[i, j] >= averageInteraction - Constants.Tolerance &&
                    interactionMatrix[i, j] > Constants.Tolerance)
                {
                    interactionForTriads[i, j] = 1;
                }
                else
                {
                    interactionForTriads[i, j] = 0;
                }
            }

            uint numberOfTriads = 0;
            for (var i = 0; i < actorsCount; i++)
            {
                for (var j = i + 1; j < actorsCount; j++)
                {
                    for (var k = j + 1; k < actorsCount; k++)
                    {
                        if (interactionForTriads[i, j] + interactionForTriads[j, i] > Constants.Tolerance
                            && interactionForTriads[i, k] + interactionForTriads[k, i] > Constants.Tolerance
                            && interactionForTriads[j, k] + interactionForTriads[k, j] > Constants.Tolerance)
                        {
                            numberOfTriads++;
                        }
                    }
                }
            }

            return numberOfTriads;
        }

        public static float[,] GetInteractionMatrix(DerivedParameter[,] network, int actorsCount)
        {
            var interactionMatrix = new float[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            for (var j = 0; j < actorsCount; j++)
            {
                interactionMatrix[i, j] = network[i, j].Homophily;
            }

            return interactionMatrix;
        }

        /// <summary>
        ///     Maximum number of combinations without repeating triplets
        /// </summary>
        /// <param name="agentsCount"></param>
        /// <returns></returns>
        public static uint MaxTriads(int agentsCount)
        {
            return Combinatorics.Combinations(agentsCount, 3);
        }
    }
}