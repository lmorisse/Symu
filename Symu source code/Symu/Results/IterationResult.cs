﻿#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Environment;
using Symu.Results.Blocker;
using Symu.Results.Messaging;
using Symu.Results.Organization;
using Symu.Results.Task;

#endregion

namespace Symu.Results
{
    public class IterationResult
    {
        protected SymuEnvironment Environment { get; }

        public IterationResult(SymuEnvironment environment)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            OrganizationFlexibility = new OrganizationFlexibility(Environment);
            KnowledgeAndBeliefResults = new KnowledgeAndBeliefResults(Environment);
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
        public OrganizationFlexibility OrganizationFlexibility { get; }

        /// <summary>
        ///     Get the knowledge and Belief performance for the group
        /// </summary>
        public KnowledgeAndBeliefResults KnowledgeAndBeliefResults { get; }

        /// <summary>
        ///     Get the Task blockers metrics
        /// </summary>
        public BlockerResults Blockers { get; } = new BlockerResults();

        /// <summary>
        ///     Get the Tasks model metrics
        /// </summary>
        public TaskResults Tasks { get; } = new TaskResults();
        /// <summary>
        ///     Get the message model metrics
        /// </summary>
        public MessageResults Messages { get; } = new MessageResults();

        public float Capacity { get; set; }

        //Specific results
        public List<PostProcessResult> SpecificResults { get; } = new List<PostProcessResult>();

        public virtual void Initialize()
        {
            OrganizationFlexibility.Clear();
            KnowledgeAndBeliefResults.Clear();
            Blockers.Clear();
            Tasks.Clear();
            Messages.Clear();
            SpecificResults.Clear();
            Iteration = 0;
            Step = 0;
            Capacity = 0;
            Success = false;
            //HasItemsNotDone = false;
            //SeemsToBeBlocked = false;
            //NotFinishedInTime = false;
            //NumberOfItemsNotDone = 0;
        }

        /// <summary>
        ///     Triggered at each end of step by SymuEnvironment.
        ///     Use to process metrics
        /// </summary>
        public void SetResults()
        {
            Tasks.SetResults(Environment);
            Blockers.SetResults(Environment);
            Messages.SetResults(Environment);
            KnowledgeAndBeliefResults.SetResults(Environment.Schedule);
            OrganizationFlexibility.SetResults(Environment.Schedule);
        }

        public IterationResult Clone()
        {
            var clone = new IterationResult(Environment);
            clone.Initialize();
            OrganizationFlexibility.CopyTo(clone.OrganizationFlexibility);
            KnowledgeAndBeliefResults.CopyTo(clone.KnowledgeAndBeliefResults);
            Blockers.CopyTo(clone.Blockers);
            Tasks.CopyTo(clone.Tasks);
            Messages.CopyTo(clone.Messages);
            clone.Iteration = Iteration;
            clone.Step = Step;
            clone.Success = Success;
            clone.Capacity = Capacity;
            //SpecificResults.CopyTo(clone.SpecificResults);
            //HasItemsNotDone = false;
            //SeemsToBeBlocked = false;
            //NotFinishedInTime = false;
            //NumberOfItemsNotDone = 0;
            return clone;
        }

        /// <summary>
        ///     Set all results to On
        /// </summary>
        public virtual void On()
        {
            OrganizationFlexibility.On = true;
            KnowledgeAndBeliefResults.On = true;
            Tasks.On = true;
            Blockers.On = true;
            Messages.On = true;
        }

        /// <summary>
        ///     Set all results to Off
        /// </summary>
        public virtual void Off()
        {
            OrganizationFlexibility.On = false;
            KnowledgeAndBeliefResults.On = false;
            Tasks.On = false;
            Blockers.On = false;
            Messages.On = false;
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