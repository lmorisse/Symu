#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Common.Classes;
using Symu.Results;
using SymuTests.Helpers;
using TestResult = SymuTests.Helpers.TestResult;

#endregion

namespace SymuTests.Results
{
    [TestClass]
    public class IterationResultTests: BaseTestClass
    {
        private IterationResult _result;
        private TestResult _specificResult;

        [TestInitialize]
        public void Initialize()
        {
            _result = new IterationResult(Environment);
            _specificResult = new TestResult();
            _result.Add(_specificResult);
            _result.Off();
            _result.SetFrequency(TimeStepType.Daily);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void GetTest()
        {
            _specificResult.On = false;
            _result.SetResults();
            Assert.IsFalse(_result.Get<TestResult>().Result);
        }

        /// <summary>
        ///     Model on - frequency
        /// </summary>
        [TestMethod]
        public void GetTest1()
        {
            _specificResult.On = true;
            _specificResult.Frequency = TimeStepType.Monthly;
            _result.SetResults();
            Assert.IsFalse(_result.Get<TestResult>().Result);
        }

        [TestMethod]
        public void GetTest2()
        {
            _specificResult.On = true;
            _result.SetResults();
            Assert.IsTrue(_result.Get<TestResult>().Result);
        }

        [TestMethod]
        public void CloneTest()
        {
            _specificResult.On = true;
            _result.SetResults();
            Environment.Messages.Result.SentMessagesCost = 1;
            _result.Messages.SetResults();

            var clone = _result.Clone();
            Assert.IsNotNull(clone.Get<TestResult>());
            Assert.IsTrue(clone.Get<TestResult>().Result);
            Assert.AreEqual(1, clone.Messages.SentMessagesCost);
        }
    }
}