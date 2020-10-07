#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Task;
using Symu.Common.Interfaces;
using Symu.OrgMod.Entities;
using Symu.Repository.Entities;
using SymuTests.Helpers;

#endregion

namespace SymuTests.Classes.Task
{
    [TestClass]
    public class SymuTaskTests : BaseTestClass
    {
        private readonly List<IKnowledge> _knowledges = new List<IKnowledge>();
        private readonly MurphyTask _model = new MurphyTask();
        private SymuTask _task;

        [TestInitialize]
        public void Initialize()
        {
            _task = new SymuTask(0);

            for (var i = 0; i < 2; i++)
            {
                // knowledge length of 10 is arbitrary in this example
                var knowledge = new Knowledge(Network, Organization.Models, i.ToString(), 10);
                _knowledges.Add(knowledge);
            }
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
            _task.Add(1, 0);
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
            _task.Add(1, 0);
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

        /// <summary>
        ///     With RequiredRatio = 0
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest()
        {
            _model.RequiredRatio = 0;
            _model.MandatoryRatio = 0;
            _task.SetKnowledgesBits(_model, _knowledges, 1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(0, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }

        /// <summary>
        ///     With RequiredRatio = 1 & MandatoryRatio =0
        ///     Complexity = 1
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest1()
        {
            _model.RequiredRatio = 1;
            _model.MandatoryRatio = 0;
            _task.SetKnowledgesBits(_model, _knowledges, 1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(10, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }

        /// <summary>
        ///     With RequiredRatio = 1 & MandatoryRatio =1
        ///     Complexity = 1
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest2()
        {
            _model.RequiredRatio = 1;
            _model.MandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 1);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(10, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(10, knowledgeBits.GetMandatory().Length);
            }
        }

        /// <summary>
        ///     With RequiredRatio = 1 & MandatoryRatio =1
        ///     Complexity = 0
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest3()
        {
            _model.RequiredRatio = 1;
            _model.MandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 0);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.AreEqual(0, knowledgeBits.GetRequired().Length);
                Assert.AreEqual(0, knowledgeBits.GetMandatory().Length);
            }
        }

        /// <summary>
        ///     With RequiredRatio = 1 & MandatoryRatio =1
        ///     Complexity = 0.5F
        /// </summary>
        [TestMethod]
        public void SetKnowledgesBitsTest4()
        {
            _model.RequiredRatio = 1;
            _model.MandatoryRatio = 1;
            _task.SetKnowledgesBits(_model, _knowledges, 0.5F);
            foreach (var knowledgeBits in _task.KnowledgesBits.List)
            {
                Assert.IsTrue(knowledgeBits.GetRequired().Length < 10 && knowledgeBits.GetRequired().Length > 0);
                Assert.IsTrue(knowledgeBits.GetMandatory().Length < 10 && knowledgeBits.GetMandatory().Length > 0);
            }
        }
    }
}