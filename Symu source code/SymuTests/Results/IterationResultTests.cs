#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Environment;
using Symu.Results;
using SymuTests.Helpers;
using TestResult = SymuTests.Helpers.TestResult;

#endregion

namespace SymuTests.Results
{
    [TestClass]
    public class IterationResultTests
    {
        private readonly SymuEnvironment _environment = new TestEnvironment();
        private IterationResult _result;
        private TestResult _specificResult;

        [TestInitialize]
        public void Initialize()
        {
            _result = new IterationResult(_environment);
            _specificResult = new TestResult(_environment);
            _result.Add(_specificResult);
            _result.On();
            _result.SetFrequency(TimeStepType.Daily);
        }

        /// <summary>
        ///     Model off
        /// </summary>
        [TestMethod]
        public void GetTest()
        {
            _specificResult.On = false;
            _result.Get<TestResult>().SetResults();
            Assert.IsFalse(_result.Get<TestResult>().Result);
        }

        /// <summary>
        ///     Model on - frequency
        /// </summary>
        [TestMethod]
        public void GetTest1()
        {
            _specificResult.Frequency = TimeStepType.Monthly;
            _result.Get<TestResult>().SetResults();
            Assert.IsFalse(_result.Get<TestResult>().Result);
        }

        [TestMethod]
        public void GetTest2()
        {
            _result.Get<TestResult>().SetResults();
            Assert.IsTrue(_result.Get<TestResult>().Result);
        }

        [TestMethod]
        public void CloneTest()
        {
            _result.Get<TestResult>().SetResults();
            _environment.Messages.Result.SentMessagesCost = 1;
            _result.Messages.SetResults();

            var clone = _result.Clone();
            Assert.IsNotNull(clone.Get<TestResult>());
            Assert.IsTrue(clone.Get<TestResult>().Result);
            Assert.AreEqual(1, clone.Messages.SentMessagesCost);
        }
    }
}