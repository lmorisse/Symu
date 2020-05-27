#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Agents;
using Symu.Classes.Task;

#endregion

namespace SymuTests.Classes.Task
{
    [TestClass]
    public class MasTaskTests
    {
        private SymuTask _task;

        [TestInitialize]
        public void Initialize()
        {
            _task = new SymuTask(0);
        }

        [TestMethod]
        public void SetWeightTest()
        {
            Assert.AreEqual(0, _task.Weight);
            _task.Weight = 1;
            Assert.AreEqual(1, _task.Weight);
        }

        /// <summary>
        ///     With weight >0
        /// </summary>
        [TestMethod]
        public void SetDoneTest()
        {
            _task.Weight = 1;
            Assert.IsTrue(_task.IsNotDone);
            _task.SetDone();
            Assert.IsFalse(_task.IsNotDone);
        }

        /// <summary>
        ///     With weight ==0
        /// </summary>
        [TestMethod]
        public void IsNotDoneTest()
        {
            _task.Weight = 0;
            Assert.IsTrue(_task.IsNotDone);
            _task.Update(0);
            Assert.IsFalse(_task.IsNotDone);
        }

        /// <summary>
        ///     With weight ==1
        /// </summary>
        [TestMethod]
        public void IsNotDoneTest1()
        {
            _task.Weight = 1;
            Assert.IsTrue(_task.IsNotDone);
            _task.WorkToDo = 0;
            _task.Update(0);
            Assert.IsFalse(_task.IsNotDone);
        }

        [TestMethod]
        public void UpdateTest()
        {
            Assert.AreEqual(0, _task.LastTouched);
            _task.Update(1);
            Assert.AreEqual(1, _task.LastTouched);
        }

        /// <summary>
        ///     With weight = 0
        /// </summary>
        [TestMethod]
        public void ResteAFaireTest()
        {
            _task.Weight = 0;
            Assert.AreEqual(0, _task.WorkToDo);
            Assert.AreEqual(100, _task.Progress);
        }

        /// <summary>
        ///     With weight = 1
        /// </summary>
        [TestMethod]
        public void ResteAFaireTest1()
        {
            _task.Weight = 1;
            Assert.AreEqual(1, _task.WorkToDo);
            Assert.AreEqual(0, _task.Progress);
            _task.WorkToDo = 0.5F;
            Assert.AreEqual(0.5F, _task.WorkToDo);
            Assert.AreEqual(50, _task.Progress);
            _task.WorkToDo = 0;
            Assert.AreEqual(0, _task.WorkToDo);
            Assert.AreEqual(100, _task.Progress);
        }

        [TestMethod]
        public void IsBlockedTest()
        {
            Assert.IsFalse(_task.IsBlocked);
            _task.Blockers.Add(1, 0);
            Assert.IsTrue(_task.IsBlocked);
        }

        /// <summary>
        ///     Weight = 0 by default, tasks is done by default
        /// </summary>
        [TestMethod]
        public void IsToDoTest0()
        {
            Assert.IsTrue(_task.IsToDo);
            Assert.IsTrue(_task.IsNotDone);
        }

        [TestMethod]
        public void IsToDoTest()
        {
            _task.Weight = 1;
            Assert.IsTrue(_task.IsToDo);
        }

        /// <summary>
        ///     Blocked > 0
        /// </summary>
        [TestMethod]
        public void IsToDoTest1()
        {
            _task.Blockers.Add(1, 0);
            Assert.IsFalse(_task.IsToDo);
        }

        /// <summary>
        ///     Progress > 0
        /// </summary>
        [TestMethod]
        public void IsToDoTest2()
        {
            _task.Weight = 1;
            _task.WorkToDo = 0.5F;
            _task.Update(0);
            Assert.IsFalse(_task.IsToDo);
        }

        /// <summary>
        ///     Assigned task
        /// </summary>
        [TestMethod]
        public void IsToDoTest3()
        {
            _task.Assigned = new AgentId(1, 1);
            Assert.IsFalse(_task.IsToDo);
        }

        /// <summary>
        ///     Assigned task
        /// </summary>
        [TestMethod]
        public void IsAssignedTest()
        {
            Assert.IsFalse(_task.IsAssigned);
            _task.Assigned = new AgentId(1, 1);
            Assert.IsTrue(_task.IsAssigned);
        }

        [TestMethod]
        public void HasBeenCancelledBy()
        {
            var agentId = new AgentId(1, 1);
            _task.Assigned = agentId;
            Assert.IsFalse(_task.IsCancelledBy(agentId));
            _task.Cancel();
            Assert.IsTrue(_task.IsCancelledBy(agentId));
        }
    }
}