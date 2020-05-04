#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Classes.Task.Knowledge;
using SymuEngine.Repository.Networks;
using SymuEngine.Repository.Networks.Beliefs;
using SymuEngine.Repository.Networks.Knowledges;

#endregion

namespace SymuEngine.Classes.Agent.Models.CognitiveArchitecture
{
    /// <summary>
    ///     Knowledge & Beliefs from Construct Software
    ///     Knowledge and knowledge Transactive memory
    ///     Beliefs and beliefs transactive memory
    ///     Referral
    /// </summary>
    /// <remarks>Knowledge & Beliefs from Construct Software</remarks>
    public class KnowledgeAndBeliefs
    {
        private readonly AgentId _id;
        private readonly Network _network;

        public KnowledgeAndBeliefs(Network network, AgentId id)
        {
            _network = network;
            _id = id;
        }

        /// <summary>
        ///     Clone KnowledgeAndBeliefs
        /// </summary>
        /// <param name="knowledgeAndBeliefs"></param>
        public void CopyTo(KnowledgeAndBeliefs knowledgeAndBeliefs)
        {
            if (knowledgeAndBeliefs is null)
            {
                throw new ArgumentNullException(nameof(knowledgeAndBeliefs));
            }

            knowledgeAndBeliefs.HasInitialKnowledge = HasInitialKnowledge;
            knowledgeAndBeliefs.HasKnowledge = HasKnowledge;
            knowledgeAndBeliefs.KnowledgeThreshHoldForDoing = KnowledgeThreshHoldForDoing;
            knowledgeAndBeliefs.HasInitialBelief = HasInitialBelief;
            knowledgeAndBeliefs.HasBelief = HasBelief;
            knowledgeAndBeliefs.BeliefThreshHoldForReacting = BeliefThreshHoldForReacting;
        }

        #region Knowledge

        /// <summary>
        ///     This parameter specify whether agents of this class can store knowledge
        /// </summary>
        public bool HasKnowledge { get; set; }

        /// <summary>
        ///     This parameter specify whether agents of this class has initial knowledge
        /// </summary>
        public bool HasInitialKnowledge { get; set; }

        /// <summary>
        ///     Get the Agent Expertise
        /// </summary>
        public AgentExpertise Expertise =>
            HasKnowledge ? _network.NetworkKnowledges.GetAgentExpertise(_id) : new AgentExpertise();

        /// <summary>
        ///     To do the task, an agent must have enough knowledge
        ///     [0 - 1]
        /// </summary>
        /// <example>if KnowledgeThreshHoldForDoing = 0.05 and agent KnowledgeId[index] = 0.6 => he can do to the question</example>
        public float KnowledgeThreshHoldForDoing { get; set; } = 0.1F;

        /// <summary>
        ///     Check Knowledge required by a task against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="taskBitIndexes">KnowledgeBits indexes of the task that must be checked against worker Knowledge</param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        /// <param name="step"></param>
        public void CheckKnowledge(ushort knowledgeId, TaskKnowledgeBits taskBitIndexes, ref bool mandatoryCheck,
            ref bool requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex, ushort step)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            // workerKnowledge may don't have the knowledge at all
            var workerKnowledge = Expertise?.GetKnowledge(knowledgeId);
            if (workerKnowledge == null)
            {
                return;
            }

            mandatoryCheck = workerKnowledge.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex,
                KnowledgeThreshHoldForDoing, step);
            requiredCheck = workerKnowledge.Check(taskBitIndexes.GetRequired(), out requiredIndex,
                KnowledgeThreshHoldForDoing, step);
        }

        /// <summary>
        ///     Check Knowledge required against the worker expertise
        /// </summary>
        /// <param name="knowledgeId"></param>
        /// <param name="knowledgeBit">KnowledgeBit index of the task that must be checked against worker Knowledge</param>
        /// <param name="step"></param>
        /// <returns>True if the knowledgeBit is known enough</returns>
        public bool CheckKnowledge(ushort knowledgeId, byte knowledgeBit, ushort step)
        {
            // workerKnowledge may don't have the knowledge at all
            var workerKnowledge = Expertise?.GetKnowledge(knowledgeId);
            return workerKnowledge != null &&
                   workerKnowledge.KnowsEnough(knowledgeBit, KnowledgeThreshHoldForDoing, step);
        }

        /// <summary>
        ///     Initialize the expertise of the agent based on the knowledge network
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public void InitializeExpertise(ushort step)
        {
            if (!HasKnowledge)
            {
                return;
            }

            if (!_network.NetworkKnowledges.Exists(_id))
                // An agent may be able to have knowledge but with no expertise for the moment
            {
                _network.NetworkKnowledges.AddAgentId(_id);
            }

            _network.NetworkKnowledges.InitializeExpertise(_id, !HasInitialKnowledge, step);
        }

        /// <summary>
        ///     Add an agentId's expertise to the network
        /// </summary>
        /// <param name="expertise"></param>
        public void AddExpertise(AgentExpertise expertise)
        {
            if (!HasKnowledge)
            {
                return;
            }

            _network.NetworkKnowledges.Add(_id, expertise);
            AddBeliefs(expertise);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="level"></param>
        /// <param name="internalCharacteristics"></param>
        public void AddKnowledge(Knowledge knowledge, KnowledgeLevel level,
            InternalCharacteristics internalCharacteristics)
        {
            if (internalCharacteristics == null)
            {
                throw new ArgumentNullException(nameof(internalCharacteristics));
            }

            AddKnowledge(knowledge, level, internalCharacteristics.MinimumRemainingKnowledge,
                internalCharacteristics.TimeToLive);
        }

        /// <summary>
        ///     Add an agentId's Knowledge to the network
        /// </summary>
        /// <param name="knowledge"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public void AddKnowledge(Knowledge knowledge, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            if (knowledge == null)
            {
                throw new ArgumentNullException(nameof(knowledge));
            }

            if (!HasKnowledge)
            {
                return;
            }

            _network.NetworkKnowledges.Add(_id, knowledge, level, minimumKnowledge, timeToLive);
            AddBelief(knowledge.Id);
        }

        #endregion

        #region Beliefs

        /// <summary>
        ///     This parameter specify whether agents of this class can store beliefs
        /// </summary>
        public bool HasBelief { get; set; }

        /// <summary>
        ///     This parameter specify whether agents of this class has initial beliefs
        /// </summary>
        public bool HasInitialBelief { get; set; }

        /// <summary>
        ///     To react, an agent must have enough belief
        ///     [0 - 1]
        /// </summary>
        /// <example>if BeliefThreshHoldForReacting = 0.05 and agent BeliefId[index] = 0.6 => he won't react</example>
        public float BeliefThreshHoldForReacting { get; set; } = 0.1F;

        /// <summary>
        ///     Get the agent Beliefs
        /// </summary>
        public AgentBeliefs Beliefs => HasBelief ? _network.NetworkBeliefs.GetAgentBeliefs(_id) : null;

        /// <summary>
        ///     Initialize the beliefs of the agent based on the belief network
        /// </summary>
        public void InitializeBeliefs()
        {
            if (!HasBelief)
            {
                return;
            }

            if (!_network.NetworkBeliefs.Exists(_id))
                // An agent may be able to have belief but with no expertise for the moment
            {
                _network.NetworkBeliefs.AddAgentId(_id);
            }

            _network.NetworkBeliefs.InitializeBeliefs(_id, !HasInitialBelief);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on agentExpertise to the network
        /// </summary>
        /// <param name="expertiseAgent"></param>
        public void AddBeliefs(AgentExpertise expertiseAgent)
        {
            if (!HasBelief)
            {
                return;
            }

            _network.NetworkBeliefs.Add(_id, expertiseAgent);
        }

        /// <summary>
        ///     Add an agentId's beliefs based on a knowledgeId to the network
        /// </summary>
        /// <param name="knowledgeId"></param>
        public void AddBelief(ushort knowledgeId)
        {
            if (!HasBelief)
            {
                return;
            }

            _network.NetworkBeliefs.Add(_id, knowledgeId);
        }

        /// <summary>
        ///     Check belief required by a task against the worker expertise
        /// </summary>
        /// <param name="beliefId"></param>
        /// <param name="taskBitIndexes">KnowledgeBot indexes of the task that must be checked against agent's beliefs</param>
        /// <param name="mandatoryCheck"></param>
        /// <param name="requiredCheck"></param>
        /// <param name="mandatoryIndex"></param>
        /// <param name="requiredIndex"></param>
        public void CheckBelief(ushort beliefId, TaskKnowledgeBits taskBitIndexes, ref float mandatoryCheck,
            ref float requiredCheck, ref byte mandatoryIndex, ref byte requiredIndex)
        {
            if (taskBitIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskBitIndexes));
            }

            // workerKnowledge may don't have the belief at all
            if (Beliefs is null)
            {
                return;
            }

            var workerBelief = Beliefs.GetBelief(beliefId);
            var belief = _network.NetworkBeliefs.GetBelief(beliefId);
            if (belief is null)
            {
                throw new NullReferenceException(nameof(belief));
            }

            if (workerBelief == null)
            {
                return;
            }

            mandatoryCheck = workerBelief.Check(taskBitIndexes.GetMandatory(), out mandatoryIndex, belief,
                BeliefThreshHoldForReacting);
            requiredCheck = workerBelief.Check(taskBitIndexes.GetRequired(), out requiredIndex, belief,
                BeliefThreshHoldForReacting);
        }

        #endregion

        #region Transactive memories

        #endregion

        #region Referral

        #endregion
    }
}