#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Task;

#endregion

namespace SymuEngineTests.Classes.Task
{
    [TestClass]
    public class TasksLimitTests
    {
        private readonly TasksLimit _tasksLimit = new TasksLimit();
        private readonly TasksLimit _tasksLimit2 = new TasksLimit();

        [TestInitialize]
        public void Initialize()
        {
            _tasksLimit.LimitSimultaneousTasks = true;
            _tasksLimit.LimitTasksInTotal = true;
            _tasksLimit.MaximumSimultaneousTasks = 10;
            _tasksLimit.MaximumTasksInTotal = 20;
        }


        [TestMethod]
        public void CopyToTest()
        {
            _tasksLimit.CopyTo(_tasksLimit2);
            Assert.IsTrue(_tasksLimit2.LimitSimultaneousTasks);
            Assert.IsTrue(_tasksLimit2.LimitTasksInTotal);
            Assert.AreEqual(10, _tasksLimit2.MaximumSimultaneousTasks);
            Assert.AreEqual(20, _tasksLimit2.MaximumTasksInTotal);
        }

        [TestMethod]
        public void NullSetAgentTasksLimitTest()
        {
            Assert.ThrowsException<ArgumentNullException>(() => _tasksLimit2.SetAgentTasksLimit(null));
        }

        [TestMethod]
        public void SetAgentTasksLimitTest()
        {
            _tasksLimit2.LimitSimultaneousTasks = true;
            _tasksLimit2.LimitTasksInTotal = true;
            _tasksLimit2.MaximumSimultaneousTasks = 1;
            _tasksLimit2.MaximumTasksInTotal = 1;
            _tasksLimit2.SetAgentTasksLimit(_tasksLimit);
            Assert.AreEqual(1, _tasksLimit.MaximumSimultaneousTasks);
            Assert.AreEqual(1, _tasksLimit.MaximumTasksInTotal);
        }

        [TestMethod]
        public void SetAgentTasksLimitTest1()
        {
            _tasksLimit2.LimitSimultaneousTasks = false;
            _tasksLimit2.LimitTasksInTotal = false;
            _tasksLimit2.SetAgentTasksLimit(_tasksLimit);
            Assert.IsFalse(_tasksLimit.LimitSimultaneousTasks);
            Assert.IsFalse(_tasksLimit.LimitTasksInTotal);
        }

        [TestMethod]
        public void HasReachedTotalMaximumLimitTest()
        {
            Assert.IsFalse(_tasksLimit.HasReachedTotalMaximumLimit(1));
            Assert.IsTrue(_tasksLimit.HasReachedTotalMaximumLimit(20));
        }

        [TestMethod]
        public void HasReachedSimultaneousMaximumLimitTest()
        {
            Assert.IsFalse(_tasksLimit.HasReachedSimultaneousMaximumLimit(1));
            Assert.IsTrue(_tasksLimit.HasReachedSimultaneousMaximumLimit(10));
        }
    }
}