#region Licence

// Description: Symu - SymuMurphiesAndBlockersTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Engine;
using Symu.Messaging.Messages;
using Symu.Repository.Networks.Beliefs;
using Symu.Repository.Networks.Knowledges;
using SymuScenariosAndEvents.Classes;
using SymuTools;

#endregion


namespace SymuMurphiesAndBlockersTests
{
    /// <summary>
    ///     Integration tests using SymuEngine
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        private const int NumberOfSteps = 61; // 3 IterationResult computations
        private readonly ExampleEnvironment _environment = new ExampleEnvironment();
        private readonly OrganizationEntity _organization = new OrganizationEntity("1");
        private readonly SymuEngine _symu = new SymuEngine();

        [TestInitialize]
        public void Initialize()
        {
            _environment.SetOrganization(_organization);
            _symu.SetEnvironment(_environment);
            _environment.SetDebug(true);
            var scenario = new TimeBasedScenario(_environment)
            {
                NumberOfSteps = NumberOfSteps
            };
            _symu.AddScenario(scenario);
        }

       
    }
}