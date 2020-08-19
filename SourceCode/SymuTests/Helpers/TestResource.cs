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
using Symu.Repository.Networks.Resources;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal sealed class TestResource : IResource
    {

        private readonly TestAgentId _id;
        public TestResource(IAgentId id)
        {
            _id = (TestAgentId)id;
        }
        public TestResource(ushort id, byte classId)
        {
            _id = new TestAgentId(id, classId);
        }
        /// <summary>
        ///     The unique agentId of the resource
        /// </summary>
        public IAgentId Id => _id;
    }
}