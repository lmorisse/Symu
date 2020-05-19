#region Licence

// Description: Symu - SymuEngineTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuEngine.Classes.Agents.Models;

#endregion

namespace SymuEngineTests.Classes.Agents.Models
{
    [TestClass]
    public class ModelEntityTests
    {
        private readonly TestSimulationSimulationModelEntity _entity = new TestSimulationSimulationModelEntity();

        /// <summary>
        ///     Rate 1 - Model Off
        /// </summary>
        [TestMethod]
        public void IsMultiTaskingTest()
        {
            _entity.On = false;
            _entity.RateOfAgentsOn = 1F;
            Assert.IsFalse(_entity.IsAgentOn());
        }

        /// <summary>
        ///     Rate 1 - Model ON
        /// </summary>
        [TestMethod]
        public void IsMultiTaskingTest1()
        {
            _entity.RateOfAgentsOn = 1F;
            _entity.On = true;
            Assert.IsTrue(_entity.IsAgentOn());
        }

        /// <summary>
        ///     Rate 0 - model on
        /// </summary>
        [TestMethod]
        public void IsMultiTaskingTest2()
        {
            _entity.On = true;
            _entity.RateOfAgentsOn = 0F;
            Assert.IsFalse(_entity.IsAgentOn());
        }

        #region Nested type: TestSimulationSimulationModelEntity

        /// <summary>
        ///     To test the abstract class
        /// </summary>
        private class TestSimulationSimulationModelEntity : ModelEntity
        {
        }

        #endregion
    }
}