#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using System.Linq;

#endregion

namespace Symu.Classes.Task
{
    /// <summary>
    ///     A task to be complete must go through different activities (Kanban.Columns)
    ///     For each activity, a task to be complete requires a collection of knowledges
    ///     For each knowledge, a task have a RequiredKnowledgesBits and a MandatoryKnowledgesBits
    /// </summary>
    public class TaskKnowledgesBits
    {
        /// <summary>
        ///     Key => Knowledge.Id
        ///     Value => TaskKnowledgeBits
        /// </summary>
        public List<TaskKnowledgeBits> List { get; } = new List<TaskKnowledgeBits>();

        public IEnumerable<ushort> KnowledgeIds => List.Select(x => x.KnowledgeId);

        public void Add(TaskKnowledgeBits bit)
        {
            List.Add(bit);
        }

        public TaskKnowledgeBits GetBits(ushort knowledgeId)
        {
            return List.Find(x => x.KnowledgeId == knowledgeId);
        }

        /// <summary>
        ///     Remove the first item of the Required Array, because the task has been done
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void RemoveFirstMandatory(ushort knowledgeId)
        {
            var bits = GetBits(knowledgeId);
            if (bits is null)
            {
                return;
            }

            byte[] removedMandatory;
            if (bits.GetMandatory().Length > 1)
            {
                removedMandatory = new byte[bits.GetMandatory().Length - 1];
                for (byte i = 1; i < bits.GetMandatory().Length; i++)
                {
                    removedMandatory[i - 1] = bits.GetMandatory()[i];
                }
            }
            else
            {
                removedMandatory = new byte[0];
            }

            bits.SetMandatory(removedMandatory);
        }

        /// <summary>
        ///     Remove the first item of the Required Array, because the task has been done
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void RemoveFirstRequired(ushort knowledgeId)
        {
            var bits = GetBits(knowledgeId);
            byte[] removedRequired;
            if (bits.GetRequired().Length > 1)
            {
                removedRequired = new byte[bits.GetRequired().Length - 1];
                for (byte i = 1; i < bits.GetRequired().Length; i++)
                {
                    removedRequired[i - 1] = bits.GetRequired()[i];
                }
            }
            else
            {
                removedRequired = new byte[0];
            }

            bits.SetRequired(removedRequired);
        }
    }
}