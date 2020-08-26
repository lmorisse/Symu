#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Linq;
using Symu.Classes.Organization;
using Symu.Classes.Task;
using Symu.Common;
using Symu.Common.Interfaces.Entity;
using Symu.Common.Math.ProbabilityDistributions;
using Symu.Repository.Entity;
using Symu.Repository.Networks.Knowledges;
using static Symu.Common.Constants;

#endregion

namespace Symu.Classes.Agents.Models.CognitiveModels
{
    /// <summary>
    ///     CognitiveArchitecture define how an actor will forget
    ///     ForgettingEntity enable or not this mechanism for all the agents during the simulation
    ///     The ForgettingModel initialize the real value of the agent's forgetting parameters
    /// </summary>
    /// <remarks>From Construct Software</remarks>
    /// <remarks>In addition, we have the MacroLearningModel</remarks>
    public class ForgettingModel : ModelEntity
    {
        private readonly byte _randomLevel;
        private bool _isAgentOnToday;
        /// <summary>
        ///     Accumulates all forgetting of the agent for this knowledge during the simulation
        /// </summary>
        public float CumulativeForgetting { get; private set; }


        /// <summary>
        ///     Percentage of all forgetting of the agent for all knowledge during the simulation
        /// </summary>
        public float PercentageForgetting
        {
            get
            {
                float percentage = 0;
                var sum = CumulativeForgetting;
                var sumKnowledge = GetKnowledgeSum();
                if (sumKnowledge > Tolerance)
                {
                    percentage = 100 * sum / sumKnowledge;
                }

                return percentage;
            }
        }
        /// <summary>
        ///     Get the sum of all the knowledge
        /// </summary>
        /// <returns></returns>
        public float GetKnowledgeSum()
        {
            return Expertise.GetAgentKnowledges<AgentKnowledge>().Sum(l => l.GetKnowledgeSum());
        }

        public ForgettingModel(ModelEntity entity, bool knowledgeModelOn, CognitiveArchitecture cognitive,
            byte randomLevel) :
            base(entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (cognitive is null)
            {
                throw new ArgumentNullException(nameof(cognitive));
            }

            InternalCharacteristics = cognitive.InternalCharacteristics;
            _randomLevel = randomLevel;
            if (!knowledgeModelOn || !cognitive.KnowledgeAndBeliefs.HasKnowledge || !InternalCharacteristics.CanForget)
            {
                // If KnowledgeModel Off or has no knowledge, there is no knowledge to forget
                // Agent is not concerned by this model
                On = false;
            }
        }

        public ForgettingModel(AgentExpertise expertise, CognitiveArchitecture cognitive, ModelEntity entity, byte randomLevel) :
            this(entity, true, cognitive, randomLevel)
        {
            Expertise = expertise;
        }

        public ForgettingModel(AgentExpertise expertise, CognitiveArchitecture cognitive, OrganizationModels models) :
            this(models.Forgetting, models.Knowledge.On, cognitive, models.RandomLevelValue)
        {
            Expertise = expertise;
        }

        public InternalCharacteristics InternalCharacteristics { get; set; }

        private AgentExpertise Expertise { get;  }
        public AgentExpertise ForgettingExpertise { get; } = new AgentExpertise();

        /// <summary>
        ///     Return the next forgetting rate
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextRate if model is On</returns>
        public float NextMean()
        {
            if (InternalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(InternalCharacteristics));
            }

            if (!_isAgentOnToday)
            {
                return 0;
            }

            var forgettingStandardDeviation =
                ForgettingStandardDeviationValue(InternalCharacteristics.ForgettingStandardDeviation);
            return InternalCharacteristics.ForgettingMean *
                   Normal.Sample(1, forgettingStandardDeviation * _randomLevel);
        }

        public void UpdateForgettingProcess(TaskKnowledgesBits knowledgesBits)
        {
            if (knowledgesBits is null)
            {
                throw new ArgumentNullException(nameof(knowledgesBits));
            }

            // Check if forgetting process is On
            if (!_isAgentOnToday)
            {
                return;
            }

            foreach (var knowledgeId in knowledgesBits.KnowledgeIds)
            {
                UpdateForgettingProcess(knowledgeId, knowledgesBits.GetBits(knowledgeId));
            }
        }

        /// <summary>
        ///     Return the next forgetting rate
        ///     Use at the initialization of the agent
        /// </summary>
        /// <returns>0 if model is Off</returns>
        /// <returns>NextPartialRate if model is On</returns>
        public float NextRate()
        {
            if (InternalCharacteristics is null)
            {
                throw new ArgumentNullException(nameof(InternalCharacteristics));
            }

            if (!_isAgentOnToday)
            {
                return 0;
            }

            return InternalCharacteristics.PartialForgetting ? InternalCharacteristics.PartialForgettingRate : 1;
        }

        public static float ForgettingStandardDeviationValue(GenericLevel level)
        {
            switch (level)
            {
                case GenericLevel.None:
                    return 0;
                case GenericLevel.VeryLow:
                    return 0.05F;
                case GenericLevel.Low:
                    return 0.1F;
                case GenericLevel.Medium:
                    return 0.15F;
                case GenericLevel.High:
                    return 0.2F;
                case GenericLevel.VeryHigh:
                    return 0.25F;
                case GenericLevel.Complete:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        /// <summary>
        ///     Clone the knowledges of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the day
        /// </summary>
        public void InitializeForgettingProcess()
        {
            // Check if forgetting process is On
            _isAgentOnToday = IsAgentOn();
            if (!_isAgentOnToday)
            {
                return;
            }

            if (Expertise == null)
            {
                throw new NullReferenceException(nameof(Expertise));
            }

            ForgettingExpertise.Clear();
            foreach (var knowledge in Expertise.GetAgentKnowledges<AgentKnowledge>())
            {
                ForgettingExpertise.Add(InitializeForgettingKnowledge(knowledge));
            }
        }

        /// <summary>
        ///     Finalize the forgetting process of the day by forgetting the knowledges bits
        ///     Use it at the end of the day
        /// </summary>
        /// <param name="step"></param>
        public void FinalizeForgettingProcess(ushort step)
        {
            // Check if forgetting process is On
            if (!_isAgentOnToday)
            {
                return;
            }

            foreach (var forget in ForgettingExpertise.GetAgentKnowledges<AgentKnowledge>())
            {
                FinalizeForgettingKnowledge(forget, step);
            }
        }

        public void FinalizeForgettingKnowledge(AgentKnowledge forget, ushort step)
        {
            if (forget is null)
            {
                throw new ArgumentNullException(nameof(forget));
            }

            var forgetBits = forget.CloneBits();
            var agentKnowledge = Expertise.GetAgentKnowledge<AgentKnowledge>(forget.KnowledgeId);
            switch (InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    for (byte i = 0; i < forgetBits.Length; i++)
                    {
                        AgentKnowledgeForget(agentKnowledge, i, forgetBits.GetBit(i), step);
                    }

                    break;
                case ForgettingSelectingMode.Oldest:
                    AgentKnowledgeForgetOldest(agentKnowledge, NextRate(), step);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        ///     Forget knowledgeBits based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="forgettingRate"></param>
        /// <param name="step"></param>
        /// <returns>The real forgetting value</returns>
        public float AgentKnowledgeForgetOldest(AgentKnowledge agentKnowledge, float forgettingRate, ushort step)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var realForgetting = agentKnowledge.KnowledgeBits.ForgetOldest(forgettingRate, step);
            CumulativeForgetting += realForgetting;
            return realForgetting;
        }

        /// <summary>
        ///     Agent forget _knowledgeBits at a forgetRate coming from ForgettingModel
        ///     If forgetRate is below the minimumLevel of KnowledgeBit that should stay, the forgetRate is adjusted to stay at the
        ///     minimumLevel
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="index">Index of the knowledgeBit</param>
        /// <param name="forgetRate">value of the decrement</param>
        /// <param name="step"></param>
        /// <returns>The real forgetting value</returns>
        public float AgentKnowledgeForget(AgentKnowledge agentKnowledge, byte index, float forgetRate, ushort step)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var value = agentKnowledge.KnowledgeBits.GetBit(index) - forgetRate;
            if (value < agentKnowledge.MinimumKnowledge)
            {
                // forgetRate > 0 
                forgetRate = Math.Max(0, agentKnowledge.KnowledgeBits.GetBit(index) - agentKnowledge.MinimumKnowledge);
            }

            if (Math.Abs(forgetRate) < Tolerance)
            {
                return 0;
            }

            var realForgetting = agentKnowledge.KnowledgeBits.UpdateBit(index, -forgetRate, step);
            CumulativeForgetting += realForgetting;
            return realForgetting;
        }

        /// <summary>
        ///     Clone the KnowledgeBits of the day that the worker can forget if he don't work on it
        ///     Called at the beginning of the step
        /// </summary>
        public AgentKnowledge InitializeForgettingKnowledge(AgentKnowledge agentKnowledge)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            float[] forgettingKnowledgeBits;
            switch (InternalCharacteristics.ForgettingSelectingMode)
            {
                case ForgettingSelectingMode.Random:
                    forgettingKnowledgeBits = InitializeForgettingKnowledgeRandom(agentKnowledge, NextRate());
                    break;
                case ForgettingSelectingMode.Oldest:
                    forgettingKnowledgeBits = Bits.Initialize(agentKnowledge.Length, 0F);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return new AgentKnowledge(agentKnowledge.KnowledgeId, forgettingKnowledgeBits, 0, -1, 0);
        }

        /// <summary>
        ///     Initialize the forgetting knowledge process with a random Selecting mode
        /// </summary>
        /// <param name="agentKnowledge"></param>
        /// <param name="nextForgettingRate"></param>
        /// <returns></returns>
        public float[] InitializeForgettingKnowledgeRandom(AgentKnowledge agentKnowledge, float nextForgettingRate)
        {
            if (agentKnowledge is null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            var forgettingKnowledgeBits = ContinuousUniform.Samples(agentKnowledge.Length, 0, 1);
            var threshold = NextMean();
            for (byte i = 0; i < agentKnowledge.Length; i++)
            {
                forgettingKnowledgeBits[i] = forgettingKnowledgeBits[i] < threshold ? nextForgettingRate : 0;
            }

            return forgettingKnowledgeBits;
        }

        /// <summary>
        ///     Forget knowledges from the expertise based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public void ForgettingProcess(float forgettingRate, ushort step)
        {
            Expertise.GetAgentKnowledges<AgentKnowledge>().ToList().ForEach(x => ForgettingProcess(x, forgettingRate, step));
        }

        /// <summary>
        ///     Forget knowledgeBits based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public static void ForgettingProcess(AgentKnowledge agentKnowledge, float forgettingRate, ushort step)
        {
            if (agentKnowledge == null)
            {
                throw new ArgumentNullException(nameof(agentKnowledge));
            }

            agentKnowledge.KnowledgeBits.ForgetOldest(forgettingRate, step);
        }

        /// <summary>
        ///     As the worker will work on specific taskBits, they can't be forget
        ///     ForgettingExpertiseOfTheDay is updated
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="workingBits"></param>
        public void UpdateForgettingProcess(IId knowledgeId, byte[] workingBits)
        {
            if (workingBits is null)
            {
                throw new ArgumentNullException(nameof(workingBits));
            }

            var forgettingKnowledge = ForgettingExpertise.GetAgentKnowledge<AgentKnowledge>(knowledgeId);
            if (forgettingKnowledge == null)
            {
                return;
            }

            for (byte i = 0; i < workingBits.Length; i++)
                // the KnowledgeBit will not be forgotten today
            {
                forgettingKnowledge.SetKnowledgeBit(workingBits[i], 0, 0);
            }
        }

        /// <summary>
        ///     As the worker will work on specific TaskKnowledgeBits, they can't be forget
        ///     ForgettingExpertiseOfTheDay is updated
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="taskBits"></param>
        public void UpdateForgettingProcess(IId knowledgeId, TaskKnowledgeBits taskBits)
        {
            if (taskBits is null)
            {
                throw new ArgumentNullException(nameof(taskBits));
            }

            // Check if forgetting process is On
            if (!_isAgentOnToday)
            {
                return;
            }

            UpdateForgettingProcess(knowledgeId, taskBits.GetMandatory());
            UpdateForgettingProcess(knowledgeId, taskBits.GetRequired());
        }
    }
}