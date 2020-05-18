#region Licence

// Description: Symu - SymuLearnAndForget
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using SymuEngine.Classes.Task.Manager;
using SymuEngine.Environment;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuLearnAndForget.Classes
{
    public sealed class LearnFromSourceAgent : LearnAgent
    {
        public LearnFromSourceAgent(ushort agentKey, SymuEnvironment environment) : base(agentKey, environment)
        {
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
            LearningModel.Learn(Knowledge.Id, bits,
                Environment.Organization.Templates.Email.MaxRateLearnable,
                TimeStep.Step);
            // the information learned is stored in a wiki
            // not the total knowledge of the agent, it is tacit knowledge for the agent
            // wiki will be filled quicker by a non expert 
            Wiki.StoreKnowledge(Knowledge.Id, bits, 1, TimeStep.Step);
        }
    }
}