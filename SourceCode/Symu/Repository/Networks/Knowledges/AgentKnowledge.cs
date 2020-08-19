#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace Symu.Repository.Networks.Knowledges
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
        ///     Constructor used by Agent.Cognitive for ForgettingKnowledge
        /// </summary>
        /// <param name="idKnowledge"></param>
        /// <param name="knowledgeBits"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        /// <param name="step"></param>
        public AgentKnowledge(ushort idKnowledge, float[] knowledgeBits, float minimumKnowledge, short timeToLive,
            ushort step)
        {
            KnowledgeId = idKnowledge;
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
            KnowledgeBits.SetBits(knowledgeBits, step);
        }

        public AgentKnowledge(ushort idKnowledge, float[] knowledgeBits, KnowledgeLevel level, float minimumKnowledge,
            short timeToLive, ushort step) : this(
            idKnowledge, knowledgeBits, minimumKnowledge, timeToLive, step)
        {
            KnowledgeLevel = level;
        }

        /// <summary>
        ///     Constructor based on the knowledge Id and the knowledge Level.
        ///     KnowledgeBits is not yet initialized.
        ///     NetworkKnowledges.InitializeAgentKnowledge must be called to initialized KnowledgeBits
        /// </summary>
        /// <param name="idKnowledge"></param>
        /// <param name="level"></param>
        /// <param name="minimumKnowledge"></param>
        /// <param name="timeToLive"></param>
        public AgentKnowledge(ushort idKnowledge, KnowledgeLevel level, float minimumKnowledge, short timeToLive)
        {
            KnowledgeId = idKnowledge;
            KnowledgeLevel = level;
            MinimumKnowledge = minimumKnowledge;
            TimeToLive = timeToLive;
            KnowledgeBits = new KnowledgeBits(minimumKnowledge, timeToLive);
        }

        public ushort KnowledgeId { get; }
        public KnowledgeBits KnowledgeBits { get; private set; }
        public KnowledgeLevel KnowledgeLevel { get; }

        /// <summary>
        ///     If agent has a knowledgeBit, and the forgetting model is on
        ///     Minimum knowledge is the minimum the agent can't forget during the simulation for this KnowledgeBit.
        ///     Range[0;1]
        /// </summary>
        public float MinimumKnowledge { get; set; }

        /// <summary>
        ///     When ForgettingSelectingMode.Oldest is selected, knowledge are forget based on their timeToLive attribute
        ///     -1 for unlimited time to live
        /// </summary>
        public short TimeToLive { get; set; }


        public byte Length => KnowledgeBits?.Length ?? 0;

        /// <summary>
        ///     Initialize KnowledgeBits with a array filled of 0
        /// </summary>
        public void InitializeWith0(byte length, ushort step)
        {
            KnowledgeBits = new KnowledgeBits(MinimumKnowledge, TimeToLive);
            KnowledgeBits.InitializeWith0(length, step);
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits
        ///     so that consumers of this library cannot change its contents
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits CloneBits()
        {
            return KnowledgeBits.Clone();
        }

        /// <summary>
        ///     Get a clone of the knowledgeBits filtered by minimumKnowledge
        ///     if a KnowledgeBit inferior minimumKnowledge then KnowledgeBit = 0
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits CloneWrittenKnowledgeBits(float minimumKnowledge)
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

        /// <summary>
        ///     Get the maximum potential of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetKnowledgePotential()
        {
            return KnowledgeBits.Length;
        }

        public void SetKnowledgeBits(float[] knowledgeBits, ushort step)
        {
            if (KnowledgeBits is null)
            {
                KnowledgeBits = new KnowledgeBits(MinimumKnowledge, TimeToLive);
            }

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

    }
}