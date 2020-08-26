using Microsoft.VisualStudio.TestTools.UnitTesting;

using Symu.Classes.Task;

using System;
using System.Collections.Generic;
using System.Text;
using Symu.Repository.Entity;

namespace Symu.Classes.Task.Tests
{
    [TestClass]
    public class MurphyTaskTests
    {
        private readonly Knowledge _knowledge =
            new Knowledge(1, "1", 10);

        private readonly MurphyTask _taskModel = new MurphyTask();
        [TestMethod]
        public void GetTaskRequiredBitsTest()
        {
            var taskRequiredBits = _taskModel.GetTaskRequiredBits(_knowledge, 0.8F);
            var numberRequiredBits = Convert.ToByte(Math.Round(_taskModel.RequiredBitsRatio(0.8F) * _knowledge.Length));
            Assert.AreEqual(numberRequiredBits, taskRequiredBits.Length);
            for (byte i = 0; i < taskRequiredBits.Length; i++)
                //It's an index of a Array[knowledge.Size]
            {
                Assert.IsTrue(taskRequiredBits[i] < _knowledge.Length);
            }
        }

        [TestMethod]
        public void GetTaskMandatoryBitsTest()
        {
            var taskMandatoryBits = _taskModel.GetTaskMandatoryBits(_knowledge, 0.8F);
            for (byte i = 0; i < taskMandatoryBits.Length; i++)
                //It's an index of a Array[knowledge.Size]
            {
                Assert.IsTrue(taskMandatoryBits[i] < _knowledge.Length);
            }
        }

        /// <summary>
        ///     MandatoryRatio = 0
        /// </summary>
        [TestMethod]
        public void GetTaskMandatoryBitsTest1()
        {
            _taskModel.MandatoryRatio = 0;
            var taskMandatoryBits = _taskModel.GetTaskMandatoryBits(_knowledge, 0.8F);
            Assert.AreEqual(0, taskMandatoryBits.Length);
        }
    }
}