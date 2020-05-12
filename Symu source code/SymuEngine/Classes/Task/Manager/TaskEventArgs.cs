#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;

#endregion

namespace SymuEngine.Classes.Task.Manager
{
    /// <summary>
    ///     The eventArg class for TaskProcessor.
    ///     The eventArg contains a task
    /// </summary>
    public class TaskEventArgs : EventArgs
    {
        public TaskEventArgs(SymuTask task)
        {
            Task = task ?? throw new ArgumentNullException(nameof(task));
        }

        public SymuTask Task { get; set; }
    }
}