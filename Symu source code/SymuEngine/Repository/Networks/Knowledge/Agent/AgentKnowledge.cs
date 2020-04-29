#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using SymuEngine.Repository.Networks.Knowledge.Bits;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Repository.Networks.Knowledge.Agent
{
    /// <summary>
    ///     Describe the Knowledge of an agent :
    ///     KnowledgeId, KnowledgeLevel, KnowledgeBits
    /// </summary>
    /// <example>Dev Java, test, project management, sociology, ...</example>
    public class AgentKnowledge
    {
        /// <summary>
        ///     Constructor used by WorkerCognitiveAgent for ForgettingKnowledge
        /// </summary>
        /// <param name="idKnowledge"></param>
        /// <param name="knowledgeBits"></param>
        public AgentKnowledge(ushort idKnowledge, KnowledgeBits knowledgeBits)
        {
            KnowledgeId = idKnowledge;
            KnowledgeBits = knowledgeBits;
        }

        /// <summary>
        ///     Constructor used by WorkerCognitiveAgent for ForgettingKnowledge
        /// </summary>
        /// <param name="idKnowledge"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="step"></param>
        public AgentKnowledge(ushort idKnowledge, float[] knowledgeBits, ushort step)
        {
            KnowledgeId = idKnowledge;
            KnowledgeBits.SetBits(knowledgeBits, step);
        }

        public AgentKnowledge(ushort idKnowledge, float[] knowledgeBits, KnowledgeLevel level, ushort step) : this(
            idKnowledge, knowledgeBits, step)
        {
            KnowledgeLevel = level;
        }

        public AgentKnowledge(ushort idKnowledge, KnowledgeBits knowledgeBits, KnowledgeLevel level) : this(idKnowledge,
            knowledgeBits)
        {
            KnowledgeLevel = level;
        }

        public AgentKnowledge(ushort idKnowledge, KnowledgeLevel level)
        {
            KnowledgeId = idKnowledge;
            KnowledgeLevel = level;
        }

        public ushort KnowledgeId { get; }
        public KnowledgeBits KnowledgeBits { get; } = new KnowledgeBits(0);
        public KnowledgeLevel KnowledgeLevel { get; }

        public byte Length => KnowledgeBits?.Length ?? 0;

        /// <summary>
        ///     Initialize KnowledgeBits with a array filled of 0
        /// </summary>
        public void InitializeWith0(byte length, ushort step)
        {
            KnowledgeBits.InitializeWith0(length, step);
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits
        ///     so that consumers of this library cannot change its contents
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits.Bits CloneBits()
        {
            return KnowledgeBits.Clone();
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits filtered by minimumKnowledge
        ///     if a KnowledgeBit inferior minimumKnowledge then KnowledgeBit = 0
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits.Bits CloneWrittenKnowledgeBits(float minimumKnowledge)
        {
            var clone = KnowledgeBits.Clone();

            if (clone.IsNull)
            {
                return null;
            }

            for (byte i = 0; i < clone.Length; i++)
                // intentionally strictly < 
            {
                if (clone.GetBit(i) < minimumKnowledge)
                {
                    clone.SetBit(i, 0);
                }
            }

            return clone;
        }

        /// <summary>
        ///     Forget knowledgeBits based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public void ForgettingProcess(short timeToLive, float forgettingRate, float minimumRemainingLevel, ushort step)
        {
            KnowledgeBits.Forget(timeToLive, forgettingRate, minimumRemainingLevel, step);
        }

        /// <summary>
        ///     Get the knowledgeBit at the index i
        /// </summary>
        /// <param name="index"></param>
        /// <returns>-1 if knowledgeBits == null</returns>
        public float GetKnowledgeBit(byte index)
        {
            return KnowledgeBits.GetBit(index);
        }

        /// <summary>
        ///     Get the sum of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetKnowledgeSum()
        {
            return KnowledgeBits.GetSum();
        }

        public void SetKnowledgeBits(float[] knowledgeBits, ushort step)
        {
            KnowledgeBits.SetBits(knowledgeBits, step);
        }

        /// <summary>
        ///     Agent forget _knowledgeBits at a forgetRate coming from ForgettingModel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public void SetKnowledgeBit(byte index, float value, ushort step)
        {
            KnowledgeBits.SetBit(index, value, step);
        }

        /// <summary>
        ///     Check if agentKnowLedgeBits include or not taskKnowledge
        /// </summary>
        /// <param name="taskKnowledgeIndexes">indexes of the KnowledgeBits that must be checked</param>
        /// <param name="index"></param>
        /// <param name="knowledgeThreshHoldForDoing"></param>
        /// <param name="step"></param>
        /// <returns>0 if agentKnowLedgeBits include taskKnowledge</returns>
        /// <returns>index if agentKnowLedgeBits include taskKnowledge</returns>
        public bool Check(byte[] taskKnowledgeIndexes, out byte index, float knowledgeThreshHoldForDoing, ushort step)
        {
            if (taskKnowledgeIndexes is null)
            {
                throw new ArgumentNullException(nameof(taskKnowledgeIndexes));
            }

            for (byte i = 0; i < taskKnowledgeIndexes.Length; i++)
            {
                if (!KnowsEnough(taskKnowledgeIndexes[i], knowledgeThreshHoldForDoing, step))
                {
                    index = taskKnowledgeIndexes[i];
                    return false;
                }
            }

            index = 0;
            return true;
        }

        /// <summary>
        ///     Check that agent has the knowledgeBit
        /// </summary>
        /// <param name="index">index of the knowledgeBit</param>
        /// <param name="knowledgeThreshHoldForAnswer"></param>
        /// <param name="step"></param>
        /// <returns>true if agent has the knowledge</returns>
        public bool KnowsEnough(byte index, float knowledgeThreshHoldForAnswer, ushort step)
        {
            if (Length == 0)
            {
                return false;
            }

            if (index >= Length)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return KnowledgeBits.GetBit(index, step) >= knowledgeThreshHoldForAnswer;
        }

        /// <summary>
        ///     Agent learn _knowledgeBits at a learningRate coming from MicroLearningModel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="learningRate"></param>
        /// <param name="step"></param>
        public void Learn(byte index, float learningRate, ushort step)
        {
            if (Math.Abs(learningRate) < tolerance)
            {
                return;
            }

            SetKnowledgeBit(index, KnowledgeBits.GetBit(index) + learningRate, step);
        }

        /// <summary>
        ///     Agent forget _knowledgeBits at a forgetRate coming from ForgettingModel
        /// </summary>
        /// <param name="index">Index of the knowledgeBit</param>
        /// <param name="forgetRate">value of the decrement</param>
        /// <param name="minimumLevel">KnowledgeBit will not be decreased below this level</param>
        public void Forget(byte index, float forgetRate, float minimumLevel)
        {
            var value = KnowledgeBits.GetBit(index) - forgetRate;
            if (Math.Abs(forgetRate) < tolerance || value < minimumLevel)
            {
                return;
            }

            KnowledgeBits.SetBit(index, value);
        }
    }
}