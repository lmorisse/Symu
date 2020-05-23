#region Licence

// Description: Symu - SymuBeliefsAndInfluence
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Agents.Models.Templates;
using Symu.Classes.Blockers;
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Messaging.Messages;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;

#endregion

namespace SymuMurphiesAndBlockers.Classes
{
    public sealed class PersonAgent : Agent
    {
        public const byte ClassKey = SymuYellowPages.Actor;

        public PersonAgent(ushort agentKey, SymuEnvironment environment) : base(
            new AgentId(agentKey, ClassKey),
            environment)
        {
            SetCognitive(Environment.Organization.Templates.Human);
        }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        public List<Knowledge> Knowledges => ((ExampleEnvironment) Environment).Knowledges;

        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                Weight = 1
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }
    }
}