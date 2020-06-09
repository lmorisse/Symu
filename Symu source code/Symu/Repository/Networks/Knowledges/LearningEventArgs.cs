#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     The eventArg class for Learning events
    ///     The eventArg contains information about the new learning
    /// </summary>
    public class LearningEventArgs : EventArgs
    {
        public LearningEventArgs(ushort knowledgeId, byte knowledgeBit, float learning)
        {
            KnowledgeId = knowledgeId;
            KnowledgeBit = knowledgeBit;
            Learning = learning;
        }

        public ushort KnowledgeId { get; set; }
        public byte KnowledgeBit { get; set; }
        public float Learning { get; set; }
    }
}