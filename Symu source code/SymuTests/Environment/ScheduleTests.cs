#region Licence

// Description: Symu - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Environment;

#endregion

namespace SymuTests.Environment
{
    [TestClass]
    //Ne pas passer en static
    public class ScheduleTests
    {
        private readonly Schedule _schedule = new Schedule();

        [TestMethod]
        [DataRow(0)]
        [DataRow(1)]
        [DataRow(2)]
        [DataRow(3)]
        [DataRow(4)]
        public void IsWorkingDayTest(int workingDay)
        {
            // Start on monday
            _schedule.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_schedule.IsWorkingDay);
        }

        [TestMethod]
        [DataRow(5)]
        [DataRow(6)]
        public void IsNotWorkingDayTest(int workingDay)
        {
            _schedule.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_schedule.IsWorkingDay);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(30)]
        [DataRow(60)]
        public void IsEndOfMonthTest(int workingDay)
        {
            // Start on monday
            _schedule.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_schedule.IsEndOfMonth);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(29)]
        [DataRow(31)]
        public void IsNotEndOfMonthTest(int workingDay)
        {
            _schedule.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_schedule.IsEndOfMonth);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(90)]
        [DataRow(180)]
        public void IsEndOfQuarterTest(int workingDay)
        {
            // Start on monday
            _schedule.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_schedule.IsEndOfQuarter);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(89)]
        [DataRow(91)]
        public void IsNotEndOfQuarterTest(int workingDay)
        {
            _schedule.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_schedule.IsEndOfQuarter);
        }

        [TestMethod]
        [DataRow(0)]
        [DataRow(365)]
        public void IsEndOfYearTest(int workingDay)
        {
            // Start on monday
            _schedule.Step = Convert.ToUInt16(workingDay);
            Assert.IsTrue(_schedule.IsEndOfYear);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(364)]
        [DataRow(91)]
        public void IsNotEndOfYearTest(int workingDay)
        {
            _schedule.Step = Convert.ToUInt16(workingDay);
            // Start on monday
            Assert.IsFalse(_schedule.IsEndOfYear);
        }
    }
}