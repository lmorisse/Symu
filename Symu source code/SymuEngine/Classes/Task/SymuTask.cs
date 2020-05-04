﻿#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agent;
using SymuEngine.Classes.Task.Knowledge;
using SymuEngine.Common;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Classes.Task
{
    /// <summary>
    ///     Base class for task
    /// </summary>
    public class SymuTask
    {
        private float _weight;

        public SymuTask(ushort step)
        {
            Created = step;
        }

        /// <summary>
        ///     Parent of the task, it may be another task or an agent
        /// </summary>
        public object Parent { get; set; }

        /// <summary>
        ///     Manage all the blockers that block the task
        /// </summary>
        public Blockers.Blockers Blockers { get; } = new Blockers.Blockers();

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
        public ImpactLevel Incorrect { get; set; } = ImpactLevel.None;

        /// <summary>
        ///     AgentId that is assigned on the task and will performed it.
        ///     Only one agent can perform a task at the same time
        /// </summary>
        public AgentId Assigned { get; set; }

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
        public bool IsToDo => !IsStarted && !IsAssigned && !IsBlocked;
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
        ///     Set the task done
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
    }
}