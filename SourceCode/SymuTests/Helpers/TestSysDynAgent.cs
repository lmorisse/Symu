#region Licence

// Description: SymuBiz - SymuTests
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Classes.Agents;
using Symu.Common.Interfaces;
using Symu.Environment;
using Symu.Repository;

#endregion

namespace SymuTests.Helpers
{
    internal sealed class TestSysDynAgent : TestReactiveAgent
    {
        public float Property1 { get; set; }
        public float Property2 { get; set; }
        public TestSysDynAgent(IAgentId id, SymuEnvironment environment) : base(id, environment){}

        public override void SetProperty(string propertyName, float value)
        {
            switch (propertyName)
            {
                case "Property1":
                    Property1 = value;
                    break;
                case "Property2":
                    Property2 = value;
                    break;
            }
        }
        public override float GetProperty(string propertyName)
        {
            return propertyName switch
            {
                "Property1" => Property1,
                "Property2" => Property2,
                _ => throw new ArgumentOutOfRangeException(nameof(propertyName))
            };
        }
    }
}