#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Repository.Networks.Sphere
{
    public readonly struct DerivedParameter
    {
        /// <summary>
        ///     The closer two agents are in each of these areas, the more likely they will be to interact.
        /// </summary>
        public float SocialDemographic { get; }

        /// <summary>
        /// </summary>
        public float RelativeBelief { get; }

        /// <summary>
        ///     An agent’s relative expertise is a function of its expertise and the expertise of an agent with which it is
        ///     interacting, and helps affect the transfer of knowledge between agents.
        ///     While an agent may be an expert in one interaction, it may not be an expert in another interaction since relative
        ///     expertise partially depends upon the knowledge of the interaction partner.
        ///     In general, agents prefer to seek out others who are relative experts, especially when attempting to accomplish a
        ///     particular task.
        /// </summary>
        public float RelativeKnowledge { get; }

        /// <summary>
        ///     An agent’s relative activity
        ///     In general, agents prefer to seek out others who are doing the same activities, especially when attempting to
        ///     accomplish a particular task.
        /// </summary>
        public float RelativeActivity { get; }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     An agent that acts via homophily attempts to ﬁnd an interaction partner that shares its characteristics.
        ///     When searching for suitable partners, the agent will stress agents who have similar socio-demographic parameters,
        ///     similar knowledge, and similar beliefs.
        ///     This process utilizes the derived parameters
        /// </remarks>
        public float Homophily => SocialDemographic + RelativeBelief + RelativeKnowledge + RelativeActivity;

        public DerivedParameter(float socialDemographic, float relativeBelief, float relativeKnowledge,
            float relativeActivity)
        {
            SocialDemographic = socialDemographic;
            RelativeBelief = relativeBelief;
            RelativeKnowledge = relativeKnowledge;
            RelativeActivity = relativeActivity;
        }

        public DerivedParameter(InteractionSphereModel model, float socialDemographic, float relativeBelief,
            float relativeKnowledge, float relativeActivity)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            SocialDemographic = socialDemographic * model.SocialDemographicWeight;
            RelativeBelief = relativeBelief * model.RelativeBeliefWeight;
            RelativeKnowledge = relativeKnowledge * model.RelativeKnowledgeWeight;
            RelativeActivity = relativeActivity * model.RelativeActivityWeight;
        }
    }
}