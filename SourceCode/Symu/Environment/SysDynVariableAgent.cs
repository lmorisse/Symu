#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

using Symu.Common.Interfaces;

namespace Symu.Environment
{
    /// <summary>
    /// VariableAgent is used with SysDynModel
    /// It make the link between a Symu.SysDyn.Variable and a Symu.Agent and its property used in SysDyn
    /// </summary>
    /// <remarks>Symu.agent must implement IAgent.SetProperty</remarks>
    public readonly struct SysDynVariableAgent
    {
        public SysDynVariableAgent(string variableName, IAgentId agentId, string property)
        {
            VariableName = variableName;
            AgentId = agentId;
            Property = property;
        }
        /// <summary>
        /// Symu.SysDyn.Variable.Name
        /// </summary>
        public string VariableName { get; }
        /// <summary>
        /// Symu.Agent.AgentId
        /// </summary>
        public IAgentId AgentId { get; }
        /// <summary>
        /// Symu.Agent.PropertyName
        /// </summary>
        public string Property { get; }
    }
}