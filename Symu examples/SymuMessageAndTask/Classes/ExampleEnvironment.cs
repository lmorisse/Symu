﻿#region Licence

// Description: Symu - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Agents.Models.Templates.Communication;
using SymuEngine.Environment;

#endregion

namespace SymuMessageAndTask.Classes
{
    public class ExampleEnvironment : SymuEnvironment
    {
        private float _costOfTask = 1F;

        private float _initialCapacity = 1F;

        private int _numberOfTasks = 1;

        private float _switchingContextCost = 1F;
        private int _workersCount = 5;

        public int WorkersCount
        {
            get => _workersCount;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("WorkersCount should be > 0");
                }

                _workersCount = value;
            }
        }

        public float InitialCapacity
        {
            get => _initialCapacity;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("InitialCapacity should be >= 0");
                }

                _initialCapacity = value;
            }
        }

        public int NumberOfTasks
        {
            get => _numberOfTasks;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("NumberOfTasks should be >= 0");
                }

                _numberOfTasks = value;
            }
        }

        public float CostOfTask
        {
            get => _costOfTask;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("CostOfTask should be >= 0");
                }

                _costOfTask = value;
            }
        }

        public float SwitchingContextCost
        {
            get => _switchingContextCost;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException("SwitchingContextCost should be >= 1");
                }

                _switchingContextCost = value;
            }
        }

        public override void SetModelForAgents()
        {
            base.SetModelForAgents();
            Organization.Templates.Human.Cognitive.InteractionPatterns.IsolationIsRandom = true;
            Organization.Models.FollowTasks = true;
            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            for (var i = 0; i < WorkersCount; i++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this)
                {
                    GroupId = group.Id
                };
                CommunicationTemplate communication = new EmailTemplate();
                WhitePages.Network.AddEmail(actor.Id, communication);
                WhitePages.Network.AddMemberToGroup(actor.Id, 100, group.Id);
            }
        }
    }
}