#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Xml.XPath;
using NCalc2.Grammar;
using Symu.Classes.Agents;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.SysDyn;
using Symu.SysDyn.Model;
using Symu.SysDyn.Simulation;

#endregion

namespace Symu.Environment
{
    /// <summary>
    /// SysDynModel encapsulate Symu.SysDyn and make the link between Symu.SysDyn.Nodes and Symu.Agents and properties
    /// </summary>
    public class SysDynModel
    {
        private readonly StateMachine _stateMachine;
        private readonly List<SysDynVariableAgent> _variableAgent = new List<SysDynVariableAgent>();

        public SysDynModel(string xmlFile)
        {
            _stateMachine = new StateMachine(xmlFile);
        }

        public void Process(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }

            _stateMachine.Process();

            foreach (var variableAgent in _variableAgent)
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                agent.SetProperty(variableAgent.Property, _stateMachine.Variables.GetValue(variableAgent.VariableName));
            }
        }

        public void UpdateVariables(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }
            foreach (var variableAgent in _variableAgent)
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                _stateMachine.Variables.SetValue(variableAgent.VariableName, agent.GetProperty(variableAgent.Property));
            }
        }

        public void AddVariableAgent(string variableName, IAgentId agentId, string property)
        {
            _variableAgent.Add(new SysDynVariableAgent(variableName, agentId, property));
        }

        public float GetVariable(string variableName)
        {
            return _stateMachine.Variables.GetValue(variableName);
        }
    }
}