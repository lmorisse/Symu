#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using Symu.Classes.Agents;
using Symu.Classes.Blockers;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Repository.Networks.Knowledges;
using Symu.Results.Blocker;
using static Symu.Tools.Constants;

#endregion

namespace Symu.Classes.Task
{
    /// <summary>
    ///     Base class for task
    /// </summary>
    public class SymuTask
    {
        private float _weight;
        private TasksManager _tasksManager;

        public SymuTask(ushort step)
        {
            Created = step;
        }
        public void SetTasksManager(TasksManager tasksManager)
        {
            if (tasksManager != null)
            {
                _tasksManager = tasksManager;
            }
        }

        /// <summary>
        ///     Parent of the task, it may be another task or an agent
        /// </summary>
        public object Parent { get; set; }

        /// <summary>
        ///     Manage all the blockers that block the task
        /// </summary>
        public BlockerCollection Blockers { get; } = new BlockerCollection();

        /// <summary>
        ///     Last step when a worker has worked on that task
        ///     Updated in update method
        /// </summary>
        public ushort LastTouched { get; set; }

        /// <summary>
        ///     the Key of the ParentId (group, process, ...) in which the task must be performed
        /// </summary>
        public ushort KeyActivity { get; set; }

        /// <summary>
        ///     Type of the task use to have specific behaviour
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        ///     Task has an associated TaskKnowledgesBits
        ///     which is a collection of Required and Mandatory KnowledgesBits for every knowledge that is required for the
        ///     activity
        /// </summary>
        public TaskKnowledgesBits KnowledgesBits { get; } = new TaskKnowledgesBits();

        /// <summary>
        ///     The task may be complete but with some level of rightness
        /// </summary>
        public ImpactLevel Incorrectness { get; set; } = ImpactLevel.None;

        /// <summary>
        ///     AgentId that is assigned on the task and will performed it.
        ///     Only one agent can perform a task at the same time
        /// </summary>
        public AgentId Assigned { get; set; }

        /// <summary>
        ///     The creator of the task
        /// </summary>
        public AgentId Creator { get; set; }

        /// <summary>
        ///     If only one agent can perform a task at the same time, agent can cancel a task and another agent can take the task.
        /// </summary>
        public List<AgentId> HasBeenCancelledBy { get; } = new List<AgentId>();

        /// <summary>
        ///     The weight of the task
        ///     Use SetWeight
        /// </summary>
        public float Weight
        {
            get => _weight;
            set
            {
                if (value < 0 || float.IsInfinity(value) || float.IsNaN(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(_weight));
                }

                _weight = value;
                WorkToDo = value;
            }
        }

        /// <summary>
        ///     0 if not started - 100 if finished
        ///     Range[0;100]
        /// </summary>
        public byte Progress =>
            Weight > 0 ? Convert.ToByte(Math.Ceiling((Weight - WorkToDo) * 100 / Weight)) : (byte) 100;


        public virtual float WorkToDo { get; set; }

        /// <summary>
        ///     Priority of the task (compared to other tasks)
        /// </summary>
        public byte Priority { get; set; }

        public bool IsAssigned => Assigned.Key > 0;

        public bool IsStarted { get; private set; }
        public bool IsToDo => !IsStarted && !IsBlocked && !IsAssigned;
        public bool IsNotDone => !IsStarted || WorkToDo > Tolerance;
        public bool IsBlocked => Blockers.IsBlocked;

        /// <summary>
        ///     Step of creation of the task, to be able to check TimeToLive
        /// </summary>
        public ushort Created { get; }

        /// <summary>
        ///     Time to live : task may be created have a limited time to live,
        ///     it will self-destruct if the time is exceeded
        ///     -1 for unlimited time to live
        /// </summary>
        /// <example>An information on an IRC channel has a more limited lifetime than an email</example>
        /// <example>Slack offer a limited history, some messaging system have a limited storage capacity</example>
        public short TimeToLive { get; set; } = -1;

        /// <summary>
        ///     CopyTo the task done
        /// </summary>
        public void SetDone()
        {
            IsStarted = true;
            WorkToDo = 0;
        }

        /// <summary>
        ///     Update Last touched value with the new step
        /// </summary>
        /// <param name="step"></param>
        public void Update(ushort step)
        {
            IsStarted = true;
            LastTouched = step;
        }

        /// <summary>
        ///     CopyTo RequiredKnowledges && MandatoryKnowledges based on the task complexity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="knowledges"></param>
        /// <param name="complexity"></param>
        public void SetKnowledgesBits(MurphyTask model, IEnumerable<Knowledge> knowledges, float complexity)
        {
            if (knowledges == null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            foreach (var knowledge in knowledges)
            {
                var bit = new TaskKnowledgeBits
                {
                    KnowledgeId = knowledge.Id
                };
                bit.SetRequired(knowledge.GetTaskRequiredBits(model, complexity));
                bit.SetMandatory(knowledge.GetTaskMandatoryBits(model, complexity));
                KnowledgesBits.Add(bit);
            }
        }

        /// <summary>
        ///     Check if the task has been cancelled by agentId
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns>true if agentId has already cancelled the task</returns>
        public bool IsCancelledBy(AgentId agentId)
        {
            return HasBeenCancelledBy.Exists(x => x.Equals(agentId));
        }

        /// <summary>
        ///     CancelBlocker a task
        /// </summary>
        public void Cancel()
        {
            HasBeenCancelledBy.Add(Assigned);
        }

        #region blockers
        /// <summary>
        ///     Add a blocker with two parameters
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public Blocker Add(int type, ushort step, object parameter1, object parameter2)
        {
            var blocker = new Blocker(type, step, parameter1, parameter2);
            return Add(blocker);
        }

        /// <summary>
        ///     Add a blocker with one parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        /// <param name="parameter"></param>
        public Blocker Add(int type, ushort step, object parameter)
        {
            var blocker = new Blocker(type, step, parameter);
            return Add(blocker);
        }
        /// <summary>
        ///     Add a blocker without parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">step of creation of the blocker</param>
        public Blocker Add(int type, ushort step)
        {
            var blocker = new Blocker(type, step);
            return Add(blocker);
        }

        public Blocker Add(Blocker blocker)
        {
            SetBlockerInProgress();
            return Blockers.Add(blocker);
        }
        /// <summary>
        ///     Remove an existing blocker from a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="blocker"></param>
        /// <param name="resolution"></param>
        public void Recover(Blocker blocker, BlockerResolution resolution)
        {
            Blockers.Recover(blocker);

            SetBlockerDone(resolution);
        }


        /// <summary>
        ///     CancelBlocker an existing blocker from a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="blocker"></param>
        public void CancelBlocker(Blocker blocker)
        {
            if (Blockers.Cancel(blocker))
            {
                SetBlockerCancelled();
            }
        }

        public void SetBlockerInProgress()
        {
            if (_tasksManager != null)
            {
                _tasksManager.BlockerResult.InProgress++;
            }
        }

        public void SetBlockerDone(BlockerResolution resolution)
        {
            if (_tasksManager == null)
            {
                return;
            }
            var blockerResult =
                _tasksManager.BlockerResult;
            switch (resolution)
            {
                case BlockerResolution.Internal:
                    blockerResult.InternalHelp++;
                    break;
                case BlockerResolution.External:
                    blockerResult.ExternalHelp++;
                    break;
                case BlockerResolution.Guessing:
                    blockerResult.Guess++;
                    break;
                case BlockerResolution.Searching:
                    blockerResult.Search++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(resolution), resolution, null);
            }

            blockerResult.Done++;
            blockerResult.InProgress--;
        }

        public void SetBlockerCancelled()
        {
            if (_tasksManager == null)
            {
                return;
            }
            var blockerResult =
                _tasksManager.BlockerResult;
            blockerResult.Cancelled++;
            blockerResult.InProgress--;
        }
        #endregion

        public void ClearBlockers()
        {
            SetBlockerDone(BlockerResolution.Internal);
            Blockers.Clear();
        }
    }
}