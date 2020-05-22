#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Classes.Task
{
    /// <summary>
    ///     A task to be complete must go through different activities (Kanban.Columns)
    ///     For each activity, a task to be complete requires a collection of knowledges
    ///     For each knowledge, a task have a RequiredKnowledgesBits and a MandatoryKnowledgesBits
    ///     TaskKnowledgeBits define the tuple Required/mandatory
    /// </summary>
    public class TaskKnowledgeBits
    {
        /// <summary>
        ///     Don't use auto property because of rule CA1819
        /// </summary>
        private byte[] _mandatory;

        /// <summary>
        ///     Don't use auto property because of rule CA1819
        /// </summary>
        private byte[] _required;

        public ushort KnowledgeId { get; set; }

        /// <summary>
        ///     specifies what the values of the required bits must be for an agent to complete a task.
        ///     For each knowledge bit required for the task, if the agents knowledge value does not equal the value specified, the
        ///     agent will guess and possibly complete the task incorrectly
        /// </summary>
        public byte[] GetRequired()
        {
            return _required;
        }

        /// <summary>
        ///     specifies what the values of the required bits must be for an agent to complete a task.
        ///     For each knowledge bit required for the task, if the agents knowledge value does not equal the value specified, the
        ///     agent will guess and possibly complete the task incorrectly
        /// </summary>
        public void SetRequired(byte[] value)
        {
            _required = value;
        }

        /// <summary>
        ///     specifies what the values of the required bits must be for an agent to complete a task without guessing.
        /// </summary>
        public byte[] GetMandatory()
        {
            return _mandatory;
        }

        /// <summary>
        ///     specifies what the values of the required bits must be for an agent to complete a task without guessing.
        /// </summary>
        public void SetMandatory(byte[] value)
        {
            _mandatory = value;
        }
    }
}