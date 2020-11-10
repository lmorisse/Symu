#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Common.Interfaces;
using Symu.Common.Math;
using Symu.Environment;
using Symu.OrgMod.GraphNetworks.TwoModesNetworks.Sphere;

#endregion

namespace Symu.Results.Organization
{
    /// <summary>
    ///     The ability of an organizationEntity to respond rapidly to the changing environment
    ///     Mainly center on the continual construction and reconstruction of groups
    /// </summary>
    public sealed class OrganizationFlexibility : Result
    {
        public OrganizationFlexibility(SymuEnvironment environment) : base(environment)
        {
        }

        /// <summary>
        ///     One of the most fundamental types of groups is the triads
        ///     Rapid formation and reformation of triads is one key aspect of flexibility
        ///     Over the time, interaction will equalize and so all possible triads will exists
        ///     The important factor is how fast the organizations reaches this state and what happens along the way
        ///     Focus on the time at which stability is reached
        ///     In nonlinear stochastics systems with noise, a standard measure is the 90 % point (90% of its final theoretical
        ///     value)
        /// </summary>
        public List<DensityStruct> Triads { get; private set; } = new List<DensityStruct>();

        /// <summary>
        ///     The number of connections between agents
        /// </summary>
        public List<DensityStruct> Links { get; private set; } = new List<DensityStruct>();

        /// <summary>
        ///     Sphere of interaction is the weight of the network in the simulation
        /// </summary>

        public List<DensityStruct> Sphere { get; private set; } = new List<DensityStruct>();

        public override void Clear()
        {
            Triads.Clear();
            Links.Clear();
            Sphere.Clear();
        }

        /// <summary>
        ///     One of the most fundamental types of groups is the triads
        ///     Rapid formation and reformation of triads is one key aspect of flexibility
        ///     For flexibility, Triads numbers are normalized with maximum potential triads
        /// </summary>
        public void HandleTriads(ushort agentsCount)
        {
            var numberOfTriads =
                InteractionMatrix.NumberOfTriads(Environment.MainOrganization.ArtifactNetwork.InteractionSphere.Sphere);
            var maxTriads = InteractionMatrix.MaxTriads(agentsCount);
            var triads = new DensityStruct(numberOfTriads, maxTriads, Environment.Schedule.Step);
            Triads.Add(triads);
        }

        /// <summary>
        ///     Sphere of interaction is the length of the network in the simulation, the number of connections between agents
        /// </summary>
        public void HandleLinks(ushort agentsCount)
        {
            var actualLinks = Environment.MainOrganization.ArtifactNetwork.ActorActor.Count;
            var maxLinks = Combinatorics.Combinations(agentsCount, 2);
            var sphere = new DensityStruct(actualLinks, maxLinks, Environment.Schedule.Step);
            Links.Add(sphere);
        }

        /// <summary>
        ///     Sphere of interaction is the length of the network in the simulation, the number of connections between agents
        /// </summary>
        public void HandleSphere()
        {
            var actualSphereWeight = Environment.MainOrganization.ArtifactNetwork.InteractionSphere.GetSphereWeight();
            var maxSphereWeight = Environment.MainOrganization.ArtifactNetwork.InteractionSphere.GetMaxSphereWeight();
            var sphere = new DensityStruct(actualSphereWeight, maxSphereWeight, Environment.Schedule.Step);
            Sphere.Add(sphere);
        }

        public override void SetResults()
        {
            var actorCount = Environment.AgentNetwork.GetInteractionSphereCount;

            HandleTriads(actorCount);
            HandleLinks(actorCount);
            HandleSphere();
        }

        public override void CopyTo(object clone)
        {
            if (!(clone is OrganizationFlexibility cloneOrganizationFlexibility))
            {
                return;
            }

            cloneOrganizationFlexibility.Links = new List<DensityStruct>();
            foreach (var result in Links)
            {
                cloneOrganizationFlexibility.Links.Add(result);
            }

            cloneOrganizationFlexibility.Sphere = new List<DensityStruct>();
            foreach (var result in Sphere)
            {
                cloneOrganizationFlexibility.Sphere.Add(result);
            }

            cloneOrganizationFlexibility.Triads = new List<DensityStruct>();
            foreach (var result in Triads)
            {
                cloneOrganizationFlexibility.Triads.Add(result);
            }
        }

        public override IResult Clone()
        {
            var clone = new OrganizationFlexibility(Environment);
            CopyTo(clone);
            return clone;
        }
    }
}