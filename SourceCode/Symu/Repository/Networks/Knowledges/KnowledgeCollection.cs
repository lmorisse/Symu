#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     List of all the knowledges
    ///     Use by NetworkKnowledge
    /// </summary>
    /// <example>
    ///     hard skills
    /// </example>
    public class KnowledgeCollection
    {
        /// <summary>
        ///     Key => ComponentId
        ///     Values => List of Knowledge
        /// </summary>
        public List<Knowledge> List { get; } = new List<Knowledge>();

        public int Count => List.Count;

        public bool Any()
        {
            return List.Any();
        }

        public void Clear()
        {
            List.Clear();
        }

        public void Add(Knowledge knowledge)
        {
            if (!Contains(knowledge))
            {
                List.Add(knowledge);
            }
        }

        public bool Contains(Knowledge knowledge)
        {
            return List.Contains(knowledge);
        }

        public Knowledge GetKnowledge(ushort knowledgeId)
        {
            return List.Find(k => k.Id == knowledgeId);
        }

        public IEnumerable<ushort> GetIds()
        {
            return List.Select(x => x.Id);
        }
    }
}