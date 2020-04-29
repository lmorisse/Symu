#region Licence

// Description: Symu - SymuEngine
// Website: Website:     https://symu.org
// Copyright: (c) 2020 laurent morisseau
// License : the program is distributed under the terms of the GNU General Public License

#endregion

#region using directives

using System;
using System.Collections.Generic;
using System.Linq;
using static SymuTools.Classes.Algorithm.Constants;

#endregion

namespace SymuEngine.Repository.Networks.Knowledge.Bits
{
    /// <summary>
    ///     Describe every bit of knowledge or belief
    /// </summary>
    public class Bits
    {
        private readonly float _rangeMin;

        /// <summary>
        ///     Array of bits
        ///     Every bit is a float of range [0, 1]
        /// </summary>
        private float[] _bits;

        public Bits(float rangeMin)
        {
            _rangeMin = rangeMin;
        }

        public Bits(float[] bits, float rangeMin) : this(rangeMin)
        {
            _bits = bits;
        }

        public byte Length => IsNull ? (byte) 0 : Convert.ToByte(_bits.Length);

        public bool IsNull => _bits == null;

        /// <summary>
        ///     Get a clone of the knowledgeBits
        ///     so that consumers of this library cannot change its contents
        /// </summary>
        /// <returns>clone of knowledgeBits</returns>
        /// <returns>null of knowledgeBits == null</returns>
        public Bits Clone()
        {
            if (IsNull)
            {
                return null;
            }

            var clone = new Bits(_rangeMin);
            clone.SetBits(_bits);
            return clone;
        }

        /// <summary>
        ///     Get the knowledgeBit at the index i
        /// </summary>
        /// <param name="index"></param>
        /// <returns>-1 if knowledgeBits == null</returns>
        public float GetBit(byte index)
        {
            if (IsNull)
            {
                return -1;
            }

            return _bits[index];
        }

        /// <summary>
        ///     Get the sum of all the _knowledgeBits of this knowledgeId
        /// </summary>
        /// <returns>if _knowledgeBits == null, return 0;</returns>
        public float GetSum()
        {
            if (IsNull)
            {
                return 0;
            }

            return _bits.Sum();
        }

        public void SetBits(float[] knowledgeBits)
        {
            if (knowledgeBits is null)
            {
                throw new ArgumentNullException(nameof(knowledgeBits));
            }

            _bits = (float[]) knowledgeBits.Clone();
        }

        /// <summary>
        ///     Agent forget _knowledgeBits at a forgetRate coming from ForgettingModel
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetBit(byte index, float value)
        {
            if (Length <= index)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            if (value < _rangeMin)
            {
                value = _rangeMin;
            }

            if (value > 1)
            {
                value = 1;
            }

            _bits[index] = value;
        }

        public override bool Equals(object obj)
        {
            return obj is Bits bits &&
                   EqualityComparer<float[]>.Default.Equals(_bits, bits._bits) &&
                   Length == bits.Length;
        }

        /// <summary>
        ///     Forget a bit of knowledge at a forgetRate coming from ForgettingModel
        /// </summary>
        /// <param name="index">Index of the knowledgeBit</param>
        /// <param name="forgettingRate">value of the decrement</param>
        /// <param name="minimumLevel">KnowledgeBit will not be decreased below this level</param>
        public void Forget(byte index, float forgettingRate, float minimumLevel)
        {
            var value = GetBit(index) - forgettingRate;
            if (Math.Abs(forgettingRate) < tolerance || value < minimumLevel)
            {
                return;
            }

            SetBit(index, value);
        }

        /// <summary>
        ///     Initialize Bits with a array filled of 0
        /// </summary>
        public void InitializeWith0(byte length)
        {
            SetBits(Initialize(length, (float) 0));
        }

        public static float[] Initialize(byte length, float value)
        {
            var bits = new float[length];
            for (byte i = 0; i < length; i++)
            {
                bits[i] = value;
            }

            return bits;
        }

        public static ushort[] Initialize(byte length, ushort value)
        {
            var bits = new ushort[length];
            for (byte i = 0; i < length; i++)
            {
                bits[i] = value;
            }

            return bits;
        }
    }
}