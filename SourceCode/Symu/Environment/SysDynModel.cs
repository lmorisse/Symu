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

#endregion

namespace Symu.Environment
{
    /// <summary>
    /// SysDynModel encapsulate Symu.SysDyn and make the link between Symu.SysDyn.Nodes and Symu.Agents and properties
    /// </summary>
    public class SysDynModel
    {
        private readonly StateMachine _stateMachine;
        private readonly List<NodeAgent> _nodeAgentList = new List<NodeAgent>();

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

            foreach (var nodeAgent in _nodeAgentList)
            {
                var agent = agents.Find(x => x.AgentId.Equals(nodeAgent.AgentId));
                agent.SetProperty(nodeAgent.Property, _stateMachine.GetVariable(nodeAgent.NodeId));
            }
        }

        public void AddNodeAgent(string nodeId, IAgentId agentId, string property)
        {
            _nodeAgentList.Add(new NodeAgent(nodeId, agentId, property));
        }

        public float GetVariable(string nodeId)
        {
            return _stateMachine.GetVariable(nodeId);
        }
    }
}