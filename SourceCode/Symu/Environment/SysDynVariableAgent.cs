#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces;
using Symu.SysDyn.Model;

namespace Symu.Environment
{
    /// <summary>
    /// NodeAgent is used with SysDynModel
    /// It make the link between a Symu.SysDyn.Node and a Symu.Agent and its property used in SysDyn
    /// </summary>
    public readonly struct SysDynVariableAgent
    {
        public SysDynVariableAgent(string variableName, IAgentId agentId, string property)
        {
            VariableName = variableName;
            AgentId = agentId;
            Property = property;
        }
        public string VariableName { get; }
        public IAgentId AgentId { get; }
        public string Property { get; }
    }
}