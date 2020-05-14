#region Licence

// Description: Symu - SymuGroupAndInteractionTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SymuBeliefsAndInfluence.Classes;
using SymuEngine.Classes.Agents.Models.CognitiveArchitecture;
using SymuEngine.Classes.Organization;
using SymuEngine.Classes.Scenario;
using SymuEngine.Common;
using SymuEngine.Engine;
using SymuEngine.Environment;
using SymuEngine.Repository.Networks.Knowledges;

#endregion


namespace SymuBeliefsAndInfluenceTests
{
    /// <summary>
    ///     Integration tests using SimulationEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 31; // 2 organizationFlexibility computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SimulationEngine _simulation = new SimulationEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _simulation.SetEnvironment(_environment);
            var scenario = new TimeStepScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _simulation.AddScenario(scenario);
            _environment.TimeStep.Type = TimeStepType.Daily;
            _organization.Models.Generator = RandomGenerator.RandomUniform;
            
        }

    }
}