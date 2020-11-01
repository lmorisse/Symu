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
using System.Xml.XPath;
using NCalc2.Grammar;
using Symu.Classes.Agents;
using Symu.Common.Classes;
using Symu.Common.Interfaces;
using Symu.Engine;
using Symu.SysDyn;
using Symu.SysDyn.Functions;
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
        public static string ExternalUpdate => SysDyn.Functions.ExternalUpdate.Value;
        public StateMachine StateMachine { get; }
        private readonly List<SysDynVariableAgent> _variableAgent = new List<SysDynVariableAgent>();
        public SysDynModel()
        {
            StateMachine = new StateMachine {Optimized = true};
        }
        public SysDynModel(string xmlFile)
        {
            StateMachine = new StateMachine(xmlFile);
        }

        public void Process(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }

            StateMachine.Process();

            // update only non constant variables
            foreach (var variableAgent in _variableAgent.Where(x => StateMachine.Variables.Exists(x.VariableName)))
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                agent.SetProperty(variableAgent.Property, StateMachine.Variables.GetValue(variableAgent.VariableName));
            }
        }

        public void UpdateVariables(List<ReactiveAgent> agents)
        {
            if (agents == null)
            {
                throw new ArgumentNullException(nameof(agents));
            }
            // update only non constant variables
            foreach (var variableAgent in _variableAgent.Where( x=> StateMachine.Variables.Exists(x.VariableName)))
            {
                var agent = agents.Find(x => x.AgentId.Equals(variableAgent.AgentId));
                StateMachine.Variables.SetValue(variableAgent.VariableName,
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

        /// <summary>
        /// Set the simulation
        /// </summary>
        /// <param name="pauseInterval"></param>
        /// <param name="fidelity"></param>
        /// <param name="timeUnits"></param>
        public void SetSimulation(Fidelity fidelity, ushort pauseInterval, TimeStepType timeUnits)
        {
            switch (fidelity)
            {
                case Fidelity.Low:
                    StateMachine.Simulation.DeltaTime = 0.5F;
                    break;
                case Fidelity.Medium:
                    StateMachine.Simulation.DeltaTime = 0.25F;
                    break;
                case Fidelity.High:
                    StateMachine.Simulation.DeltaTime = 0.125F;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fidelity), fidelity, null);
            }
            StateMachine.Simulation.PauseInterval = pauseInterval;
            StateMachine.Simulation.TimeUnits = timeUnits;

        }

        public void Clear()
        {
            StateMachine.Clear();
        }

        public void Initialize()
        {
            Auxiliary.CreateInstance(StateMachine.Variables, "FrequencyFactor", Schedule.FrequencyFactor(StateMachine.Simulation.TimeUnits).ToString());
            StateMachine.Initialize();
        }
        /// <summary>
        /// Add a model 
        /// </summary>
        /// <param name="model">List of the variables of the model</param>
        public void Add(List<IVariable> model)
        {
            StateMachine.Variables.AddRange(model);
        }
    }
}