#region Licence

// Description: Symu - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using Symu.Classes.Agents;
using Symu.Classes.Organization;
using Symu.Classes.Scenario;
using Symu.Common;
using Symu.Environment;
using Symu.Results;

#endregion

namespace Symu.Engine
{
    /// <summary>
    ///     Symu Engine to use in batch mode
    ///     Use SymuForm in GUI mode
    /// </summary>
    public class SymuEngine
    {
        private readonly List<SimulationScenario> _scenarii = new List<SimulationScenario>();

        /// <summary>
        ///     Manage the multiple iterations of the symu
        ///     A interaction is a number of interaction steps
        ///     Multiple iterations are used to replay a symu for MonteCarlo process or to vary parameters
        /// </summary>
        public Iterations Iterations { get; set; } = new Iterations();

        public SimulationResults SimulationResults { get; set; } = new SimulationResults();
        public AgentState State { get; private set; } = AgentState.Stopped;

        /// <summary>
        ///     Environment of the symu
        /// </summary>
        protected SymuEnvironment Environment { get; set; }

        public IterationResult IterationResult => Environment.IterationResult;

        #region Step level

        /// <summary>
        ///     Used when Event OnNextDay is triggered by this class
        /// </summary>
        public virtual void OnNextStep()
        {
            Environment.OnNextStep();
            Environment.ManageAgentsToStop();
        }

        #endregion

        #region Initialize / set

        /// <summary>
        ///     Add a new environment to the symu engine
        /// </summary>
        /// <param name="environment"></param>
        public void SetEnvironment(SymuEnvironment environment)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public void PreProcess()
        {
            SimulationResults.Clear();
            Iterations.SetUp();
        }

        public void AddScenario(SimulationScenario scenario)
        {
            if (!_scenarii.Exists(s => s.Id.Equals(scenario.Id)))
            {
                _scenarii.Add(scenario);
            }
        }

        #endregion

        #region Process level

        public virtual SimulationResults Process()
        {
            State = AgentState.Starting;
            PreProcess();
            while (!StopProcess())
            {
                Iteration();
            }

            PostProcess();
            return SimulationResults;
        }

        protected virtual void PostProcess()
        {
        }

        protected bool StopProcess()
        {
            return Iterations.Stop();
        }

        #endregion

        #region Process by AgentId

        public virtual void ProcessByAgent(AgentId agentId)
        {
            PreProcessByAgent();
            ProcessAgent(agentId);
            PostProcessByAgent();
        }

        protected virtual void PreProcessByAgent()
        {
            //ByTeam = true;
        }

        protected virtual void PostProcessByAgent()
        {
        }

        public virtual void ProcessAgent(AgentId agentId)
        {
            throw new NotImplementedException("ProcessTeam");
            //ProcessingTeam = team;
            //Process();
        }

        #endregion

        #region Iteration level

        public void Iteration()
        {
            PreIteration();
            while (!StopIteration())
            {
                OnNextStep();
            }

            PostIteration();
        }

        /// <summary>
        ///     When to stop Timer event
        ///     Call each Environment StopIteration
        /// </summary>
        /// <returns></returns>
        public virtual bool StopIteration()
        {
            return Environment.StopIteration();
        }

        private void PostIteration()
        {
            SimulationResults.List.Add(Environment.SetIterationResult(Iterations.Number));
            AnalyzeIteration();
            State = AgentState.Stopped;
        }

        public void PreIteration()
        {
            State = AgentState.Starting;
            InitializeIteration();
            Iterations.UpdateIteration(_scenarii);
            Environment.Start();
            Environment.WaitingForStart();
            Environment.SetInteractionSphere(true);
            State = AgentState.Started;
        }

        protected virtual void AnalyzeIteration()
        {
            if (!Environment.IterationResult.Success)
            {
            }
        }

        public virtual void InitializeIteration()
        {
            Environment.InitializeIteration();
            // SetScenarii should stay after initialize
            SetScenariiAndTimeStep();
        }

        /// <summary>
        ///     Initialize the engine, environment ready to launch the first step
        ///     Used in unit tests
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="organization"></param>
        public void Initialize(SymuEnvironment environment, OrganizationEntity organization)
        {
            if (environment == null)
            {
                throw new ArgumentNullException(nameof(environment));
            }

            environment.SetOrganization(organization);
            SetEnvironment(environment);
            PreIteration();
            InitializeIteration();
            environment.Start();
            environment.PreStep();
        }

        protected void SetScenariiAndTimeStep()
        {
            ushort step0 = 0;
            foreach (var scenario in _scenarii.Where(sc => sc.IsActive))
            {
                var clone = scenario.Clone();
                clone.SetUp();
                // scenarii could have different Day0 (>0)
                step0 = step0 == 0 ? clone.Day0 : Math.Min(step0, clone.Day0);
            }

            Environment.Schedule.Step = step0;
        }

        #endregion
    }
}