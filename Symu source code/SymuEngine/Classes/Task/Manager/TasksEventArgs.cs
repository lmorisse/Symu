#region Licence

// Description: Symu - SymuEngine
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace Symu.Classes.Task.Manager
{
    /// <summary>
    ///     The eventArg class for TaskProcessor.
    ///     The eventArg contains a list of tasks
    /// </summary>
    public class TasksEventArgs : EventArgs
    {
        public TasksEventArgs(IEnumerable<SymuTask> tasks)
        {
            Tasks = tasks ?? throw new ArgumentNullException(nameof(tasks));
        }

        public IEnumerable<SymuTask> Tasks { get; set; }
    }
}