#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Symu.Environment;
using Symu.Results.Blocker;
using Symu.Results.Organization;
using Symu.Results.Task;

#endregion

namespace Symu.Results
{
    public class IterationResult
    {
        private readonly SymuEnvironment _environment;

        public IterationResult(SymuEnvironment environment)
        {
            _environment = environment;
        }

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
        /// </summary>
        public OrganizationFlexibility OrganizationFlexibility { get; private set; }

        /// <summary>
        ///     Get the knowledge and Belief performance for the group
        /// </summary>
        public OrganizationKnowledgeAndBelief OrganizationKnowledgeAndBelief { get; private set; }

        /// <summary>
        ///     Get the Task blockers metrics
        /// </summary>
        public BlockerResults Blockers { get;  } = new BlockerResults();

        /// <summary>
        ///     Get the Tasks model metrics
        /// </summary>
        public TaskResults Tasks { get; } = new TaskResults();

        public float Capacity { get; set; }

        //Specific results
        public List<PostProcessResult> SpecificResults { get; } = new List<PostProcessResult>();

        public virtual void Initialize()
        {
            OrganizationFlexibility = new OrganizationFlexibility(_environment);
            OrganizationKnowledgeAndBelief = new OrganizationKnowledgeAndBelief(_environment.WhitePages.Network,
                _environment.Organization.Models);
            Blockers.Clear();
            Tasks.Clear();
            Iteration = 0;
            Step = 0;
            Success = false;
            HasItemsNotDone = false;
            SeemsToBeBlocked = false;
            NotFinishedInTime = false;
            NumberOfItemsNotDone = 0;
            Capacity = 0;
        }

        /// <summary>
        ///     Triggered at each end of step by SymuEnvironment.
        ///     Use to process metrics
        /// </summary>
        public void PostStep()
        {
            Tasks.SetResults(_environment);
            Blockers.SetResults(_environment);
        }

        #region todo : refactor in SpecificResults

        /// <summary>
        ///     Symu has been stopped SpecificResults
        /// </summary>
        public bool HasItemsNotDone { get; set; }

        public bool SeemsToBeBlocked { get; set; }

        //public bool NotFastEnoughAtHalfReplay;
        public bool NotFinishedInTime { get; set; }
        public ushort NumberOfItemsNotDone { get; set; }

        #endregion
    }
}