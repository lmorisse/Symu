#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using SymuEngine.Environment;
using SymuEngine.Repository;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Results.Organization
{
    /// <summary>
    ///     The ability of an organizationEntity to respond rapidly to the changing environment
    ///     Mainly center on the continual construction and reconstruction of groups
    /// </summary>
    public class OrganizationFlexibility
    {
        /// <summary>
        ///     Set the classKey of the agents that we want to get the flexibility performance
        /// </summary>
        private const byte ClassKey = SymuYellowPages.Actor;

        /// <summary>
        ///     Network of the simulation
        /// </summary>
        private readonly SymuEnvironment _environment;

        public OrganizationFlexibility(SymuEnvironment environment)
        {
            _environment = environment;
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
        public List<TriadsStruct> Triads { get; } = new List<TriadsStruct>();

        public void Clear()
        {
            Triads.Clear();
        }

        /// <summary>
        ///     One of the most fundamental types of groups is the triads
        ///     Rapid formation and reformation of triads is one key aspect of flexibility
        ///     For flexibility, Triads numbers are normalized with maximum potential triads
        /// </summary>
        public void HandleTriads(ushort step)
        {
            var numberOfTriads = KnowledgeMatrix.NumberOfTriads(
                _environment.WhitePages.Network.NetworkKnowledges.Repository,
                _environment.WhitePages.FilteredAgentIdsByClassKey(ClassKey),
                _environment.WhitePages.Network.NetworkKnowledges);
            var maxTriads = KnowledgeMatrix.MaxTriads(_environment.WhitePages.FilteredAgentsByClassCount(ClassKey));
            var triads = new TriadsStruct(numberOfTriads, maxTriads, step);
            Triads.Add(triads);
        }

        public void HandlePerformance(ushort step)
        {
            if (!_environment.Organization.Models.FollowGroupFlexibility)
            {
                return;
            }

            HandleTriads(step);
        }
    }
}