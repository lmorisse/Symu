#region Licence

// Description: SymuBiz - Symu
// Website: https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using Symu.Common;

#endregion

namespace Symu.Repository.Networks.Knowledges
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


        public KnowledgeBits(float rangeMin, short timeToLive) : base(rangeMin)
        {
            SetLastTouched(new ushort[Length]);
            TimeToLive = timeToLive;
        }

        public KnowledgeBits(float[] bits, float rangeMin, short timeToLive) : base(bits, rangeMin)
        {
            SetLastTouched(new ushort[Length]);
            TimeToLive = timeToLive;
        }

        /// <summary>
        ///     When ForgettingSelectingMode.Oldest is selected, knowledge are forget based on their timeToLive attribute
        ///     -1 for unlimited time to live
        /// </summary>
        public short TimeToLive { get; set; }

        /// <summary>
        ///     The knowledge obsolescence : 1 - LastTouched.Average()/LastStep
        /// </summary>
        /// <returns>0 for the first step of the simulation</returns>
        public float Obsolescence(float step)
        {
            return step > 0 ? 1F - _lastTouched.Average() / step : 0;
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
            _lastTouched[index] = step;
            return GetBit(index);
        }

        /// <summary>
        ///     Clone bit with a deltaValue at a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="step"></param>
        public void SetBit(byte index, float value, ushort step)
        {
            SetBit(index, value);
            // Intentionally before UpdateLastTouched
            UpdateLastTouched(index, step);
        }

        /// <summary>
        ///     Update bit with a deltaValue at a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="deltaValue"></param>
        /// <param name="step"></param>
        /// <returns>The real delta Value that has updated the bit</returns>
        public float UpdateBit(byte index, float deltaValue, ushort step)
        {
            var realValue = UpdateBit(index, deltaValue);
            if (Math.Abs(realValue) < Constants.Tolerance)
            {
                return 0;
            }

            UpdateLastTouched(index, step);
            return realValue;

            // Intentionally before UpdateLastTouched
        }

        /// <summary>
        ///     Update lastTouched
        ///     Clone to step if LastTouched > 0
        ///     Clone to 0 if LastTouched == 0
        /// </summary>
        /// <param name="index"></param>
        /// <param name="step"></param>
        public void UpdateLastTouched(byte index, ushort step)
        {
            if (Math.Abs(GetBit(index) - RangeMin) < Constants.Tolerance)
            {
                // If deltaValue is at minimum, we stop trying to forget it
                // So LastTouched is reset to 0
                _lastTouched[index] = 0;
            }
            else
            {
                _lastTouched[index] = step;
            }
        }

        /// <summary>
        ///     Forget knowledgeBits based on knowledgeBits.LastTouched and timeToLive deltaValue
        /// </summary>
        /// <returns>The real delta deltaValue that has updated the bit</returns>
        public float ForgetOldest(float forgettingRate, ushort step)
        {
            if (TimeToLive == -1)
            {
                return 0;
            }

            float realUpdate = 0;
            for (byte i = 0; i < Length; i++)
            {
                if (step - _lastTouched[i] > TimeToLive)
                {
                    realUpdate += UpdateBit(i, -forgettingRate, step);
                }
            }

            return realUpdate;
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