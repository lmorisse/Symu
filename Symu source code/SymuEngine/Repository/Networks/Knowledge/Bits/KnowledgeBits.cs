#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

namespace SymuEngine.Repository.Networks.Knowledge.Bits
{
    /// <summary>
    ///     Describe every bit of knowledge
    /// </summary>
    public class KnowledgeBits : Bits
    {
        /// <summary>
        ///     Don't use auto property because of rule CA1819
        /// </summary>
        private ushort[] _lastTouched;

        public KnowledgeBits(float rangeMin) : base(rangeMin)
        {
            SetLastTouched(new ushort[Length]);
        }

        public KnowledgeBits(float[] bits, float rangeMin) : base(bits, rangeMin)
        {
            SetLastTouched(new ushort[Length]);
        }

        /// <summary>
        ///     Array of last touched
        ///     Bits of information can be forget or be obsolete if not read or learn often enough
        ///     lastTouched is the last step when the bit has been read or learned
        /// </summary>
        public ushort[] GetLastTouched()
        {
            return _lastTouched;
        }

        /// <summary>
        ///     Array of last touched
        ///     Bits of information can be forget or be obsolete if not read or learn often enough
        ///     lastTouched is the last step when the bit has been read or learned
        /// </summary>
        public void SetLastTouched(ushort[] value)
        {
            _lastTouched = value;
        }

        public void SetBits(float[] knowledgeBits, ushort step)
        {
            SetBits(knowledgeBits);
            SetLastTouched(Initialize(Length, step));
        }

        public float GetBit(byte index, ushort step)
        {
            GetLastTouched()[index] = step;
            return GetBit(index);
        }

        public void SetBit(byte index, float value, ushort step)
        {
            GetLastTouched()[index] = step;
            SetBit(index, value);
        }

        /// <summary>
        ///     Forget knowledgeBits based on knowledgeBits.LastTouched and timeToLive value
        /// </summary>
        public void Forget(short timeToLive, float forgettingRate, float minimumRemainingLevel, ushort step)
        {
            for (byte i = 0; i < Length; i++)
            {
                if (step - GetLastTouched()[i] > timeToLive)
                {
                    Forget(i, forgettingRate, minimumRemainingLevel);
                }
            }
        }

        /// <summary>
        ///     Initialize Bits with a array filled of 0
        /// </summary>
        public void InitializeWith0(byte length, ushort step)
        {
            SetBits(Initialize(length, (float) 0), step);
        }
    }
}