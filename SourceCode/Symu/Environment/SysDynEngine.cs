#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.SysDyn.Engine;
using Symu.SysDyn.Models;

#endregion

namespace Symu.Environment
{
    /// <summary>
    /// SysDynModel encapsulate Symu.SysDyn and make the link between Symu.SysDyn.Nodes and Symu.Agents and properties
    /// </summary>
    public class SysDynEngine : StateMachine
    {
        private readonly List<SysDynVariableAgent> _variableAgent = new List<SysDynVariableAgent>();
        public SysDynEngine()
        {
        }
        public SysDynEngine(string xmlFile): base(xmlFile, true)
        {
        }

        public void Process(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }

            Process();
            var variables = Models.GetVariables();
            // update only non constant variables
            foreach (var variableAgent in _variableAgent.Where(x => variables.Exists(x.VariableName)))
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                agent.SetProperty(variableAgent.Property, variables.GetValue(variableAgent.VariableName));
            }
        }

        public void UpdateVariables(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }
            var variables = Models.GetVariables();
            // update only non constant variables
            foreach (var variableAgent in _variableAgent.Where( x=> variables.Exists(x.VariableName)))
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                variables.SetValue(variableAgent.VariableName,
                    agent.GetProperty(variableAgent.Property));
            }
        }
        /// <summary>
        /// Add a new SysDynVariableAgent 
        /// It make the link between a Symu.SysDyn.Variable and a Symu.Agent and its property used in SysDyn
        /// In case of similar variable and property names
        /// </summary>
        /// <param name="name"></param>
        /// <param name="agentId"></param>
        public void AddVariableAgent(string name, IAgentId agentId)
        {
            AddVariableAgent(name, agentId, name);
        }
        /// <summary>
        /// Add a new SysDynVariableAgent 
        /// It make the link between a Symu.SysDyn.Variable and a Symu.Agent and its property used in SysDyn
        /// </summary>
        /// <param name="variableName"></param>
        /// <param name="agentId"></param>
        /// <param name="property"></param>
        public void AddVariableAgent(string variableName, IAgentId agentId, string property)
        {
            _variableAgent.Add(new SysDynVariableAgent(variableName, agentId, property));
        }
    }
}