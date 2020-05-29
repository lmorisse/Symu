#region Licence

// Description: Symu - SymuMurphiesAndBlockers
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Task;
using Symu.Environment;
using Symu.Repository;
using Symu.Repository.Networks.Knowledges;
using SymuTools;
using SymuTools.Math.ProbabilityDistributions;

#endregion

namespace SymuScenariosAndEvents.Classes
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

        public AgentId GroupId { get; set; }

        private MurphyTask Model => ((ExampleEnvironment) Environment).Model;
        public List<Knowledge> Knowledges => ((ExampleEnvironment) Environment).Knowledges;
        
        public override void GetNewTasks()
        {
            var task = new SymuTask(Schedule.Step)
            {
                // Weight is randomly distributed around 1, but has a minimum of 0
                Weight = Math.Max(0,Normal.Sample(1,0.1F*Environment.Organization.Models.RandomLevelValue)),
                // Creator is randomly  a person of the group - for the incomplete information murphy
                Creator = Environment.WhitePages.FilteredAgentIdsByClassKey(ClassKey).Shuffle().First()
            };
            task.SetKnowledgesBits(Model, Knowledges, 1);
            Post(task);
        }
    }
}