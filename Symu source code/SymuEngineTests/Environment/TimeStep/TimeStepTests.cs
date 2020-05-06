#region Licence

// Description: Symu - SymuEngineTests
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#endregion

namespace SymuEngineTests.Environment.TimeStep
{
    [TestClass]
    //Ne pas passer en static
    public class TimeStepTests
    {
        private readonly SymuEngine.Environment.TimeStep _timeStep =
            new SymuEngine.Environment.TimeStep();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void IsWorkingDayTest(int workingDay)
        {
            // Start on monday
            _timeStep.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_timeStep.IsWorkingDay);
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(6)]
        public void IsNotWorkingDayTest(int workingDay)
        {
            _timeStep.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_timeStep.IsWorkingDay);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(30)]
        [DataRow(60)]
        public void IsEndOfMonthTest(int workingDay)
        {
            // Start on monday
            _timeStep.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_timeStep.IsEndOfMonth);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(29)]
        [DataRow(31)]
        public void IsNotEndOfMonthTest(int workingDay)
        {
            _timeStep.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_timeStep.IsEndOfMonth);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(90)]
        [DataRow(180)]
        public void IsEndOfQuarterTest(int workingDay)
        {
            // Start on monday
            _timeStep.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_timeStep.IsEndOfQuarter);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(89)]
        [DataRow(91)]
        public void IsNotEndOfQuarterTest(int workingDay)
        {
            _timeStep.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_timeStep.IsEndOfQuarter);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(365)]
        public void IsEndOfYearTest(int workingDay)
        {
            // Start on monday
            _timeStep.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_timeStep.IsEndOfYear);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(364)]
        [DataRow(91)]
        public void IsNotEndOfYearTest(int workingDay)
        {
            _timeStep.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_timeStep.IsEndOfYear);
        }
    }
}