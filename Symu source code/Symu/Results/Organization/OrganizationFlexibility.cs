#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Environment;
using Symu.Repository;
using Symu.Repository.Networks.Sphere;
using Symu.Tools.Math;

#endregion

namespace Symu.Results.Organization
{
    /// <summary>
    ///     The ability of an organizationEntity to respond rapidly to the changing environment
    ///     Mainly center on the continual construction and reconstruction of groups
    /// </summary>
    public class OrganizationFlexibility
    {
        /// <summary>
        ///     Network of the simulation
        /// </summary>
        private readonly SymuEnvironment _environment;

        public OrganizationFlexibility(SymuEnvironment environment)
        {
            _environment = environment;
        }

        /// <summary>
        ///     If set to true, OrganizationFlexibility will be filled with value and stored during the simulation
        /// </summary>
        public bool On { get; set; }

        public TimeStepType Frequency { get; set; } = TimeStepType.Monthly;

        /// <summary>
        ///     One of the most fundamental types of groups is the triads
        ///     Rapid formation and reformation of triads is one key aspect of flexibility
        ///     Over the time, interaction will equalize and so all possible triads will exists
        ///     The important factor is how fast the organizations reaches this state and what happens along the way
        ///     Focus on the time at which stability is reached
        ///     In nonlinear stochastics systems with noise, a standard measure is the 90 % point (90% of its final theoretical
        ///     value)
        /// </summary>
        public List<GroupDensityStruct> Triads { get; private set; } = new List<GroupDensityStruct>();

        /// <summary>
        ///     The number of connections between agents
        /// </summary>
        public List<GroupDensityStruct> Links { get; private set; } = new List<GroupDensityStruct>();

        /// <summary>
        ///     Sphere of interaction is the weight of the network in the simulation
        /// </summary>

        public List<GroupDensityStruct> Sphere { get; private set; } = new List<GroupDensityStruct>();

        public void Clear()
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
        public void HandleTriads(ushort agentsCount, ushort step)
        {
            var numberOfTriads =
                InteractionMatrix.NumberOfTriads(_environment.WhitePages.Network.InteractionSphere.Sphere);
            var maxTriads = InteractionMatrix.MaxTriads(agentsCount);
            var triads = new GroupDensityStruct(numberOfTriads, maxTriads, step);
            Triads.Add(triads);
        }

        /// <summary>
        ///     Sphere of interaction is the length of the network in the simulation, the number of connections between agents
        /// </summary>
        public void HandleLinks(ushort agentsCount, ushort step)
        {
            var actualLinks = _environment.WhitePages.Network.NetworkLinks.Count;
            var maxLinks = Combinatorics.Combinations(agentsCount, 2);
            var sphere = new GroupDensityStruct(actualLinks, maxLinks, step);
            Links.Add(sphere);
        }

        /// <summary>
        ///     Sphere of interaction is the length of the network in the simulation, the number of connections between agents
        /// </summary>
        public void HandleSphere(ushort step)
        {
            var actualSphereWeight = _environment.WhitePages.Network.InteractionSphere.GetSphereWeight();
            var maxSphereWeight = _environment.WhitePages.Network.InteractionSphere.GetMaxSphereWeight();
            var sphere = new GroupDensityStruct(actualSphereWeight, maxSphereWeight, step);
            Sphere.Add(sphere);
        }

        public void SetResults(Schedule schedule)
        {
            if (!On)
            {
                return;
            }
            bool handle;
            switch (Frequency)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    handle = true;
                    break;
                case TimeStepType.Weekly:
                    handle = schedule.IsEndOfWeek;
                    break;
                case TimeStepType.Monthly:
                    handle = schedule.IsEndOfMonth;
                    break;
                case TimeStepType.Yearly:
                    handle = schedule.IsEndOfYear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!handle)
            {
                return;
            }

            var actorCount =(ushort) _environment.WhitePages.AllAgents()
                .Count(a => a.Cognitive.InteractionPatterns.IsPartOfInteractionSphere);

            HandleTriads(actorCount, schedule.Step);
            HandleLinks(actorCount, schedule.Step);
            HandleSphere(schedule.Step);
        }

        public void CopyTo(OrganizationFlexibility cloneOrganizationFlexibility)
        {
            if (cloneOrganizationFlexibility == null)
            {
                throw new ArgumentNullException(nameof(cloneOrganizationFlexibility));
            }

            cloneOrganizationFlexibility.Links = new List<GroupDensityStruct>();
            foreach (var result in Links)
            {
                cloneOrganizationFlexibility.Links.Add(result);
            }

            cloneOrganizationFlexibility.Sphere = new List<GroupDensityStruct>();
            foreach (var result in Sphere)
            {
                cloneOrganizationFlexibility.Sphere.Add(result);
            }

            cloneOrganizationFlexibility.Triads = new List<GroupDensityStruct>();
            foreach (var result in Triads)
            {
                cloneOrganizationFlexibility.Triads.Add(result);
            }
        }
    }
}