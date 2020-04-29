#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using SymuEngine.Environment;
using SymuEngine.Results.Blocker;
using SymuEngine.Results.Organization;

#endregion

namespace SymuEngine.Results
{
    public class IterationResult
    {
        /// <summary>
        ///     Number of iterations
        /// </summary>
        public ushort Iteration { get; set; }

        /// <summary>
        ///     Number of steps in the current iteration
        /// </summary>
        public ushort Step { get; set; }

        public bool Success { get; set; }

        /// <summary>
        ///     One of the most fundamental types of groups is the triads
        ///     Rapid formation and reformation of triads is one key aspect of flexibility
        ///     Over the time, interaction will equalize and so all possible triads will exists
        ///     The important factor is how fast the organizations reaches this state and what happens along the way
        ///     Focus on the time at which stability is reached
        ///     In nonlinear stochastics systems with noise, a standard measure is the 90 % point (90% of its final theoretical)
        ///     value)
        /// </summary>
        public OrganizationFlexibility OrganizationFlexibility { get; private set; }

        /// <summary>
        ///     Get the knowledge and Belief performance for the group
        /// </summary>
        public OrganizationKnowledgeAndBelief OrganizationKnowledgeAndBelief { get; private set; }

        /// <summary>
        ///     Get the Task blockers metrics
        /// </summary>
        public BlockerResults Blockers { get; private set; }

        public float Capacity { get; set; }

        //Specific results
        public List<PostProcessResult> SpecificResults { get; } = new List<PostProcessResult>();

        public virtual void Clear(SymuEnvironment environment)
        {
            if (environment is null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            OrganizationFlexibility = new OrganizationFlexibility(environment);
            OrganizationKnowledgeAndBelief = new OrganizationKnowledgeAndBelief(environment.WhitePages.Network,
                environment.OrganizationEntity.OrganizationModels);
            Blockers = new BlockerResults();
            Iteration = 0;
            Step = 0;
            Success = false;
            HasItemsNotDone = false;
            SeemsToBeBlocked = false;
            NotFinishedInTime = false;
            NumberOfItemsNotDone = 0;
            Capacity = 0;
        }

        #region todo : refactor in SpecificResults

        /// <summary>
        ///     Simulation has been stopped SpecificResults
        /// </summary>
        public bool HasItemsNotDone { get; set; }

        public bool SeemsToBeBlocked { get; set; }

        //public bool NotFastEnoughAtHalfReplay;
        public bool NotFinishedInTime { get; set; }
        public ushort NumberOfItemsNotDone { get; set; }

        #endregion
    }
}