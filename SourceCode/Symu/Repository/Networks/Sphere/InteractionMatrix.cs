#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveModels;
using Symu.Common;
using Symu.Common.Math;

#endregion

namespace Symu.Repository.Networks.Sphere
{
    /// <summary>
    ///     Knowledge matrix
    ///     use to compute metrics based on Knowledge and NetworkKnowledges
    /// </summary>
    /// <example></example>
    public static class InteractionMatrix
    {
        /// <summary>
        ///     Max value of the interaction matrix
        /// </summary>
        /// <param name="interactionMatrix"></param>
        /// <returns></returns>
        public static float GetMaxInteractionMatrix(float[,] interactionMatrix)
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

            float max = 0;
            for (var i = 0; i < actorsCount; i++)
            for (var j = i + 1; j < actorsCount; j++)
            {
                max = Math.Max(max, interactionMatrix[i, j]);
            }

            return max;
        }

        /// <summary>
        ///     Max value of the interaction matrix
        /// </summary>
        /// <param name="interactionMatrix"></param>
        /// <returns></returns>
        public static float GetMinInteractionMatrix(float[,] interactionMatrix)
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

            float min = 0;
            for (var i = 0; i < actorsCount; i++)
            for (var j = i + 1; j < actorsCount; j++)
            {
                min = Math.Min(min, interactionMatrix[i, j]);
            }

            return min;
        }

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
            for (var j = i + 1; j < actorsCount; j++)
            {
                average += interactionMatrix[i, j];
            }

            return 2 * average / (actorsCount * (actorsCount - 1));
        }

        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     the actor i knows the fact k or not
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static DerivedParameter GetAverageInteractionMatrix(DerivedParameter[,] network)
        {
            var knowledge = GetAverageInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Knowledge));
            var activities = GetAverageInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Activities));
            var beliefs = GetAverageInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Beliefs));
            var socialDemographics =
                GetAverageInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.SocialDemographics));
            return new DerivedParameter(socialDemographics, beliefs, knowledge, activities);
        }

        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     the actor i knows the fact k or not
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static DerivedParameter GetMaxInteractionMatrix(DerivedParameter[,] network)
        {
            var knowledge = GetMaxInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Knowledge));
            var activities = GetMaxInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Activities));
            var beliefs = GetMaxInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Beliefs));
            var socialDemographics =
                GetMaxInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.SocialDemographics));
            return new DerivedParameter(socialDemographics, beliefs, knowledge, activities);
        }

        /// <summary>
        ///     The likelihood that one actor is to attempt to interact with another is defined by the fact that
        ///     the actor i knows the fact k or not
        /// </summary>
        /// <param name="network"></param>
        /// <returns></returns>
        public static DerivedParameter GetMinInteractionMatrix(DerivedParameter[,] network)
        {
            var knowledge = GetMinInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Knowledge));
            var activities = GetMinInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Activities));
            var beliefs = GetMinInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.Beliefs));
            var socialDemographics =
                GetMinInteractionMatrix(GetInteractionMatrix(network, InteractionStrategy.SocialDemographics));
            return new DerivedParameter(socialDemographics, beliefs, knowledge, activities);
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

            var interactionMatrix = GetInteractionMatrix(network);

            var averageInteraction = GetAverageInteractionMatrix(interactionMatrix);
            var interactionForTriads = new sbyte[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)
            for (var j = i + 1; j < actorsCount; j++)
            {
                if (interactionMatrix[i, j] >= averageInteraction - Constants.Tolerance &&
                    interactionMatrix[i, j] > Constants.Tolerance)
                {
                    interactionForTriads[i, j] = 1;
                    interactionForTriads[j, i] = 1;
                }
                else
                {
                    interactionForTriads[i, j] = 0;
                    interactionForTriads[j, i] = 0;
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

        public static float[,] GetInteractionMatrix(DerivedParameter[,] network)
        {
            return GetInteractionMatrix(network, InteractionStrategy.Homophily);
        }

        public static float[,] GetInteractionMatrix(DerivedParameter[,] network,
            InteractionStrategy interactionStrategy)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            var actorsCount = network.GetLength(0);
            var interactionMatrix = new float[actorsCount, actorsCount];
            for (var i = 0; i < actorsCount; i++)

                // InteractionSphere is a symmetrical matrix with identity == 0
            for (var j = i + 1; j < actorsCount; j++)
            {
                switch (interactionStrategy)
                {
                    case InteractionStrategy.Homophily:
                        interactionMatrix[i, j] = network[i, j].Homophily;
                        interactionMatrix[j, i] = network[i, j].Homophily;
                        break;
                    case InteractionStrategy.Knowledge:
                        interactionMatrix[i, j] = network[i, j].RelativeKnowledge;
                        interactionMatrix[j, i] = network[i, j].RelativeKnowledge;
                        break;
                    case InteractionStrategy.Activities:
                        interactionMatrix[i, j] = network[i, j].RelativeActivity;
                        interactionMatrix[j, i] = network[i, j].RelativeActivity;
                        break;
                    case InteractionStrategy.Beliefs:
                        interactionMatrix[i, j] = network[i, j].RelativeBelief;
                        interactionMatrix[j, i] = network[i, j].RelativeBelief;
                        break;
                    case InteractionStrategy.SocialDemographics:
                        interactionMatrix[i, j] = network[i, j].SocialDemographic;
                        interactionMatrix[j, i] = network[i, j].SocialDemographic;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(interactionStrategy), interactionStrategy, null);
                }
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

        public static float GetDensity(DerivedParameter[,] network)
        {
            if (network == null)
            {
                throw new ArgumentNullException(nameof(network));
            }

            var actorsCount = network.GetLength(0);
            var density = 0;
            var total = 0;
            for (var i = 0; i < actorsCount; i++)

                // InteractionSphere is a symmetrical matrix with identity == 0
            for (var j = i + 1; j < actorsCount; j++)
            {
                total++;
                if (network[i, j].Homophily > 0)
                {
                    density++;
                }
            }

            if (total == 0)
            {
                return 0;
            }

            return (float) density / total;
        }
    }
}