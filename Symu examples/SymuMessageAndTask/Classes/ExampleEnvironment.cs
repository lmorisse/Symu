#region Licence

// Description: SymuBiz - SymuMessageAndTask
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Organization;
using Symu.Environment;
using Symu.Messaging.Templates;
using Symu.Repository.Networks.Databases;

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
                    throw new ArgumentOutOfRangeException("NumberOfMessages should be >= 0");
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

        public override void SetOrganization(OrganizationEntity organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization));
            }

            base.SetOrganization(organization);

            IterationResult.Off();
            IterationResult.Tasks.On = true;
            IterationResult.Messages.On = true;
            organization.Murphies.Off();

            SetDebug(false);
        }

        public override void SetAgents()
        {
            base.SetAgents();
            var group = new GroupAgent(Organization.NextEntityIndex(), this);
            for (var i = 0; i < WorkersCount; i++)
            {
                var actor = new PersonAgent(Organization.NextEntityIndex(), this, Organization.Templates.Human)
                {
                    GroupId = group.AgentId
                };
                CommunicationTemplate communication = new EmailTemplate();
                var entity = new DataBaseEntity(actor.AgentId, communication);
                var email = new Database(entity, Organization.Models, WhitePages.MetaNetwork.Knowledge);
                WhitePages.MetaNetwork.Databases.Add(actor.AgentId, email);
                WhitePages.MetaNetwork.AddAgentToGroup(actor.AgentId, 100, group.AgentId, false);
            }
        }
    }
}