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
using Symu.Common.Interfaces.Agent;
using Symu.Common.Interfaces.Entity;
using Symu.Environment;
using Symu.Repository;
using Symu.Repository.Networks.Resources;
using Symu.Repository.Networks.Roles;

#endregion

namespace SymuTests.Helpers
{
    /// <summary>
    ///     Class for tests
    /// </summary>
    internal sealed class TestResource : IResource
    {

        private readonly UId _id;
        public TestResource(UId id)
        {
            _id = (UId)id;
        }
        public TestResource(ushort id)
        {
            _id = new UId(id);
        }
        /// <summary>
        ///     The unique agentId of the resource
        /// </summary>
        public IId Id => _id;
    }
}