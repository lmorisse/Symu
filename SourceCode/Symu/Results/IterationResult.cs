#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.DNA;
using Symu.Environment;
using Symu.Results.Blockers;
using Symu.Results.Messaging;
using Symu.Results.Organization;
using Symu.Results.Tasks;

#endregion

namespace Symu.Results
{
    /// <summary>
    /// The iteration result manage and store all results of an iteration
    /// </summary>
    public class IterationResult
    {
        public IterationResult(SymuEnvironment environment)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
            OrganizationFlexibility = new OrganizationFlexibility(Environment);
            KnowledgeAndBeliefResults = new KnowledgeAndBeliefResults(Environment);
            Blockers = new BlockerResults(Environment);
            Tasks = new TaskResults(Environment);
            Messages = new MessageResults(Environment);

            Results.Add(OrganizationFlexibility);
            Results.Add(KnowledgeAndBeliefResults);
            Results.Add(Blockers);
            Results.Add(Tasks);
            Results.Add(Messages);
            Results.Add(KeyFrames);
        }

        protected SymuEnvironment Environment { get; }

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
        ///     Symu has been stopped Results
        /// </summary>
        public bool HasItemsNotDone { get; set; }

        public bool SeemsToBeBlocked { get; set; }
        public bool NotFinishedInTime { get; set; }
        public ushort NumberOfItemsNotDone { get; set; }

        /// <summary>
        ///     Keyframes from Symu.DNA: list of meta-network snapshot over the time
        /// </summary>
        public KeyFrames KeyFrames { get; } = new KeyFrames();

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
        public BlockerResults Blockers { get; }

        /// <summary>
        ///     Get the Tasks model metrics
        /// </summary>
        public TaskResults Tasks { get; }

        /// <summary>
        ///     Get the message model metrics
        /// </summary>
        public MessageResults Messages { get; }

        //Specific results
        public List<IResult> Results { get; } = new List<IResult>();

        public virtual void Initialize()
        {
            foreach (var result in Results)
            {
                result.Clear();
            }

            Iteration = 0;
            Step = 0;
            Success = false;
            HasItemsNotDone = false;
            SeemsToBeBlocked = false;
            NotFinishedInTime = false;
            NumberOfItemsNotDone = 0;
        }

        /// <summary>
        ///     Triggered at each end of step by SymuEnvironment.
        ///     Use to process metrics
        /// </summary>
        public void SetResults()
        {
            foreach (var result in Results.Where(ComputeResults))
            {
                if (result is KeyFrames keyFrame)
                {
                    keyFrame.Add(Environment.Schedule.Step, Environment.MainOrganization.MetaNetwork);
                    keyFrame.SetResults();
                }
                else
                {
                    result.SetResults();
                }
            }
        }

        public bool ComputeResults(IResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (!result.On)
            {
                return false;
            }

            bool set;
            switch (result.Frequency)
            {
                case TimeStepType.Intraday:
                case TimeStepType.Daily:
                    set = true;
                    break;
                case TimeStepType.Weekly:
                    set = Environment.Schedule.IsEndOfWeek;
                    break;
                case TimeStepType.Monthly:
                    set = Environment.Schedule.IsEndOfMonth;
                    break;
                case TimeStepType.Yearly:
                    set = Environment.Schedule.IsEndOfYear;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return set;
        }

        public IterationResult Clone()
        {
            var clone = new IterationResult(Environment);

            foreach (var result in Results)
            {
                var cloneResult = clone.Get(result.GetType());
                if (cloneResult == null)
                {
                    // specific results
                    clone.Add(result.Clone());
                }
                else
                {
                    // default results
                    result.CopyTo(cloneResult);
                }
            }

            clone.Iteration = Iteration;
            clone.Step = Step;
            clone.Success = Success;
            HasItemsNotDone = false;
            SeemsToBeBlocked = false;
            NotFinishedInTime = false;
            NumberOfItemsNotDone = 0;
            return clone;
        }

        /// <summary>
        ///     Set all results to On
        /// </summary>
        public virtual void On()
        {
            foreach (var result in Results)
            {
                result.On = true;
            }
        }

        /// <summary>
        ///     Set all results to Off
        /// </summary>
        public virtual void Off()
        {
            foreach (var result in Results)
            {
                result.On = false;
            }
        }

        /// <summary>
        ///     Add a results to the collection
        /// </summary>
        /// <param name="result"></param>
        public void Add(IResult result)
        {
            Results.Add(result);
        }

        /// <summary>
        ///     Get a result from the collection by its type
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <returns></returns>
        public TResult Get<TResult>() where TResult : IResult
        {
            return (TResult) Results.Find(x => x is TResult);
        }

        /// <summary>
        ///     Get a result from the collection by its type
        /// </summary>
        /// <returns></returns>
        public IResult Get(Type type)
        {
            return Results.Find(x => x.GetType() == type);
        }

        public void SetFrequency(TimeStepType frequency)
        {
            foreach (var result in Results)
            {
                result.Frequency = frequency;
            }
        }
    }
}