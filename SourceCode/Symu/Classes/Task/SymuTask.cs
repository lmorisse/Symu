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
using Symu.Classes.Agents;
using Symu.Classes.Blockers;
using Symu.Classes.Task.Manager;
using Symu.Common;
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.DNA.OneModeNetworks.Knowledge;
using Symu.Repository.Entity;
using Symu.Results.Blocker;
using static Symu.Common.Constants;

#endregion

namespace Symu.Classes.Task
{
    /// <summary>
    ///     Base class for task
    /// </summary>
    public class SymuTask
    {
        private TasksManager _tasksManager;
        private float _weight;

        public SymuTask(ushort day)
        {
            Created = day;
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
        ///     Last day when a worker has worked on that task
        ///     Updated in update method
        /// </summary>
        public ushort LastTouched { get; set; }

        /// <summary>
        ///     the Key of the ParentId (group, process, ...) in which the task must be performed
        /// </summary>
        public IId KeyActivity { get; set; }

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
        public IAgentId Assigned { get; set; }

        /// <summary>
        ///     The creator of the task
        /// </summary>
        public IAgentId Creator { get; set; }

        /// <summary>
        ///     If only one agent can perform a task at the same time, agent can cancel a task and another agent can take the task.
        /// </summary>
        public List<IAgentId> HasBeenCancelledBy { get; } = new List<IAgentId>();

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

                var delta = value - _weight;
                _weight = value;
                // as agent may have already been working on the task, we assign only the delta value
                WorkToDo += delta;
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

        public bool IsAssigned => !(Assigned == null || Assigned.IsNull);
        public bool HasCreator => !(Creator == null || Creator.IsNull);

        public bool IsStarted { get; private set; }
        public bool IsToDo => !IsStarted && !IsBlocked && !IsAssigned;
        public bool IsNotDone => !IsStarted || WorkToDo > Tolerance;
        public bool IsBlocked => Blockers.IsBlocked;

        /// <summary>
        ///     Day of creation of the task, to be able to check TimeToLive
        /// </summary>
        public ushort Created { get; }

        /// <summary>
        ///     Time to live : task may be created have a limited time to live,
        ///     it will self-destruct if the time is exceeded
        ///     -1 for unlimited time to live
        ///     TimeToLive is in days
        /// </summary>
        /// <example>An information on an IRC channel has a more limited lifetime than an email</example>
        /// <example>Slack offer a limited history, some messaging system have a limited storage capacity</example>
        public short TimeToLive { get; set; } = -1;

        public void SetTasksManager(TasksManager tasksManager)
        {
            if (tasksManager != null)
            {
                _tasksManager = tasksManager;
            }
        }

        /// <summary>
        ///     Clone the task done
        /// </summary>
        public void SetDone()
        {
            IsStarted = true;
            WorkToDo = 0;
        }

        /// <summary>
        ///     Update Last touched value with the new day
        /// </summary>
        /// <param name="step"></param>
        public void Update(ushort step)
        {
            IsStarted = true;
            LastTouched = step;
        }

        /// <summary>
        ///     Clone RequiredKnowledges && MandatoryKnowledges based on the task complexity
        /// </summary>
        /// <param name="model"></param>
        /// <param name="knowledges"></param>
        /// <param name="complexity"></param>
        public void SetKnowledgesBits(MurphyTask model, IEnumerable<IKnowledge> knowledges, float complexity)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (knowledges == null)
            {
                throw new ArgumentNullException(nameof(knowledges));
            }

            foreach (var knowledge in knowledges.Cast<Knowledge>())
            {
                var bit = new TaskKnowledgeBits
                {
                    KnowledgeId = knowledge.Id
                };
                bit.SetRequired(model.GetTaskRequiredBits(knowledge, complexity));
                bit.SetMandatory(model.GetTaskMandatoryBits(knowledge, complexity));
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
        ///     Cancel a task
        /// </summary>
        public void Cancel()
        {
            HasBeenCancelledBy.Add(Assigned);
            CancelBlockers();
            // intentionally after CancelBlockers
            UnAssign();
        }

        //UnAssign the task
        public void UnAssign()
        {
            // UnAssigned
            Assigned = new AgentId();
            _tasksManager = null;
        }


        #region blockers

        public void ClearBlockers()
        {
            while (Blockers.List.Any())
            {
                var blocker = Blockers.List.First();
                Recover(blocker, BlockerResolution.Internal);
            }
        }

        /// <summary>
        ///     Add a blocker with two parameters
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">day of creation of the blocker</param>
        /// <param name="parameter1"></param>
        /// <param name="parameter2"></param>
        public Blocker Add(int type, ushort step, object parameter1, object parameter2)
        {
            var blocker = new Blocker(type, step, parameter1, parameter2);
            Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Add a blocker with one parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">day of creation of the blocker</param>
        /// <param name="parameter"></param>
        public Blocker Add(int type, ushort step, object parameter)
        {
            var blocker = new Blocker(type, step, parameter);
            Add(blocker);
            return blocker;
        }

        /// <summary>
        ///     Add a blocker without parameter
        ///     And follow it in the IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="type">type of the blocker</param>
        /// <param name="step">day of creation of the blocker</param>
        public Blocker Add(int type, ushort step)
        {
            var blocker = new Blocker(type, step);
            Add(blocker);
            return blocker;
        }

        public void Add(Blocker blocker)
        {
            SetBlockerInProgress();
            Blockers.Add(blocker);
        }

        /// <summary>
        ///     Remove an existing blocker from a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        /// <param name="blocker"></param>
        /// <param name="resolution"></param>
        public void Recover(Blocker blocker, BlockerResolution resolution)
        {
            if (Blockers.Remove(blocker))
            {
                SetBlockerDone(resolution);
            }
        }


        /// <summary>
        ///     Cancel Blockers of a task
        ///     And update IterationResult if FollowBlocker is true
        /// </summary>
        public void CancelBlockers()
        {
            while (Blockers.List.Any())
            {
                var blocker = Blockers.List.First();
                if (Blockers.Remove(blocker))
                {
                    SetBlockerCancelled();
                }
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
    }
}