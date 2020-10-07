#region Licence

// Description: SymuBiz - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents.Models.CognitiveTemplates;
using Symu.Classes.Task.Manager;
using Symu.Environment;
using Symu.Repository.Entities;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class LearnFromSourceAgent : LearnAgent
    {
        /// <summary>
        ///     Constructor of the agent
        /// </summary>
        /// <remarks>Call the Initialize method after the constructor, or call the factory method</remarks>
        private LearnFromSourceAgent(SymuEnvironment environment,
            CognitiveArchitectureTemplate template) : base(environment, template)
        {
        }

        /// <summary>
        ///     Factory method to create an agent
        ///     Call the Initialize method
        /// </summary>
        /// <returns></returns>
        public new static LearnFromSourceAgent CreateInstance(SymuEnvironment environment,
            CognitiveArchitectureTemplate template)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var agent = new LearnFromSourceAgent(environment, template);
            agent.Initialize();
            return agent;
        }

        /// <summary>
        ///     each task allows to learn a little more about knowledge
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void AfterSetTaskDone(object sender, TaskEventArgs e)
        {
            // The learning is done randomly on the knowledgeBits
            var knowledgeBit = Knowledge.GetRandomBitIndex();
            // Agent is learning via email
            // We simplify here, we should send an email to ask for information to a database, then treat the reply
            // That is why we have to initialize a Bits
            var bits = new Bits(0);
            bits.InitializeWith0(Knowledge.Length);
            // The source has the maximum knowledge bit
            bits.SetBit(knowledgeBit, 1);
            LearningModel.Learn(Knowledge.EntityId, bits,
                Environment.MainOrganization.Communication.Email.MaxRateLearnable,
                Schedule.Step);
            // the information learned is stored in a wiki
            // not the total knowledge of the agent, it is tacit knowledge for the agent
            // wiki will be filled quicker by a non expert 
            Wiki.StoreKnowledge(Knowledge.EntityId, bits, 1, Schedule.Step);
        }
    }
}