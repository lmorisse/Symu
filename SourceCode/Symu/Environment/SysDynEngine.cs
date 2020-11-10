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
using Symu.Common.Interfaces;
using Symu.SysDyn.Engine;
using Symu.SysDyn.Models.Symu;

#endregion

namespace Symu.Environment
{
    /// <summary>
    /// SysDynModel encapsulate Symu.SysDyn
    /// There are two way to link Symu and SysDyn :
    /// * Synchronize Symu.SysDyn.Variables and Symu.Agents and properties
    /// * Synchronize Symu.SysDyn.Variables and Symu.ModelNetwork
    /// </summary>
    public class SysDynEngine : StateMachine
    {
        private readonly List<SysDynVariableAgent> _variableAgents = new List<SysDynVariableAgent>();
        public SysDynEngine()
        {
        }
        public SysDynEngine(string xmlFile): base(xmlFile)
        {
        }
        public void Add(ModelNetwork modelNetwork)
        {
            if (modelNetwork == null)
            {
                throw new ArgumentNullException(nameof(modelNetwork));
            }

            Add(modelNetwork.GlobalModel());
        }

        /// <summary>
        /// Process SysDynEngine
        /// Synchronize VariableAgents between Symu and SysDyn if you have setted some VariableAgents
        /// Synchronize ModelNetwork between Symu and SysDyn if you have setted some ModelNetwork.Entities
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="modelNetwork"></param>
        public void Process(List<ReactiveAgent> agents, ModelNetwork modelNetwork)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }       
            
            // First update variables with the agents' properties' values
            SynchronizeSysDyn(agents,modelNetwork);
            // Then Process
            Process();
            // Finally update agents with the variables' values
            SynchronizeSymu(agents, modelNetwork);
        }

        /// <summary>
        /// Update Agents from SysDyn.Variables
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="modelNetwork"></param>
        public void SynchronizeSymu(List<ReactiveAgent> agents, ModelNetwork modelNetwork)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }

            foreach (var variableName in Variables.Outputs)
            {            
                // Synchronize VariableAgent from Variables 
                if (_variableAgents.Exists(x => x.VariableName == variableName))
                //foreach (var variableAgent in _variableAgent.Where(x => Variables.Exists(x.VariableName)))
                {
                    var variableAgent = _variableAgents.Find(x => x.VariableName == variableName);
                    var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                    agent.SetProperty(variableAgent.Property, Variables.GetValue(variableName));
                }

                if (modelNetwork == null)
                {
                    continue;
                }
                // Synchronize ModelNetwork from Variables
                foreach (var entity in modelNetwork)
                {
                    if (entity.Model.Variables.Exists(variableName))
                    {
                        entity.Model.Variables.SetValue(variableName, Variables.GetValue(variableName));
                    }
                }
            }
        }

        /// <summary>
        /// Update SysDyn.Variables from VariablesAgents and ModelNetwork
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="modelNetwork"></param>
        public void SynchronizeSysDyn(List<ReactiveAgent> agents, ModelNetwork modelNetwork)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }

            foreach (var variableName in Variables.Inputs)
            {
                // Synchronize Variables from VariableAgent
                if (_variableAgents.Exists(x => x.VariableName == variableName))
                {
                    var variableAgent = _variableAgents.Find(x => x.VariableName == variableName);
                    var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                    Variables.SetValue(variableName, agent.GetProperty(variableAgent.Property));
                }

                if (modelNetwork == null)
                {
                    continue;
                }
                // Synchronize Variables from ModelNetwork
                foreach (var entity in modelNetwork)
                {
                    if (entity.Model.Variables.Exists(variableName))
                    {
                        Variables.SetValue(variableName, entity.Model.Variables.GetValue(variableName));
                    }
                }
            }
            //// Synchronize Variables from VariableAgent
            //foreach (var variableAgent in _variableAgents.Where( x=> Variables.Exists(x.VariableName)))
            //{
            //    var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
            //    Variables.SetValue(variableAgent.VariableName,
            //        agent.GetProperty(variableAgent.Property));
            //}
            //// Synchronize Variables from ModelNetwork
            //foreach (var entity in _modelNetwork)
            //{
            //    foreach (var outputName in entity.Model.Variables.Outputs)
            //    {
            //        Variables.SetValue(outputName, entity.Model.Variables.GetValue(outputName));
            //    }
            //}
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
            _variableAgents.Add(new SysDynVariableAgent(variableName, agentId, property));
        }
    }
}