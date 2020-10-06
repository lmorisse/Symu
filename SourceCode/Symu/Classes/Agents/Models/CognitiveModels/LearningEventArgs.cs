#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common.Interfaces;


#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     The eventArg class for Learning events
    ///     The eventArg contains information about the new learning
    /// </summary>
    public class LearningEventArgs : EventArgs
    {
        public LearningEventArgs(IAgentId knowledgeId, byte knowledgeBit, float learning)
        {
            KnowledgeId = knowledgeId;
            KnowledgeBit = knowledgeBit;
            Learning = learning;
        }

        public IAgentId KnowledgeId { get; set; }
        public byte KnowledgeBit { get; set; }
        public float Learning { get; set; }
    }
}