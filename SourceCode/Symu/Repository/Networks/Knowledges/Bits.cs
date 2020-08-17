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

#endregion

namespace Symu.Repository.Networks.Knowledges
{
    /// <summary>
    ///     Describe every bit of knowledge or belief
    /// </summary>
    public class Bits
    {
        /// <summary>
        ///     Maximum number of Bits
        /// </summary>
        public const byte MaxBits = 100;

        /// <summary>
        ///     Array of bits
        ///     Every bit is a float of range [0, 1]
        /// </summary>
        private float[] _bits;

        public Bits(float rangeMin)
        {
            RangeMin = rangeMin;
        }

        public Bits(float[] bits, float rangeMin) : this(rangeMin)
        {
            _bits = bits;
        }

        public float RangeMin { get; }

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

            var clone = new Bits(RangeMin);
            clone.SetBits(_bits);
            return clone;
        }

        /// <summary>
        ///     Get the knowledgeBit at the index i
        /// </summary>
        /// <param name="index"></param>
        /// <returns>0 if knowledgeBits == null</returns>
        public float GetBit(byte index)
        {
            if (IsNull)
            {
                return 0;
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
        ///     Clone bit with a value at a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public void SetBit(byte index, float value)
        {
            if (Length <= index)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            if (value < RangeMin)
            {
                value = RangeMin;
            }

            if (value > 1)
            {
                value = 1;
            }

            _bits[index] = value;
        }

        /// <summary>
        ///     Update bit with a value at a specified index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <returns>The real delta value that has updated the bit</returns>
        public float UpdateBit(byte index, float value)
        {
            if (Length <= index)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            if (Math.Sign(value) == -1 && _bits[index] <= RangeMin)
            {
                return 0;
            }

            var newValue = _bits[index] + value;
            var realDelta = value;
            if (newValue < RangeMin)
            {
                realDelta = RangeMin - _bits[index];
                newValue = RangeMin;
            }

            if (newValue > 1)
            {
                realDelta = 1 - _bits[index];
                newValue = 1;
            }

            _bits[index] = newValue;
            return realDelta;
        }

        public override bool Equals(object obj)
        {
            return obj is Bits bits &&
                   EqualityComparer<float[]>.Default.Equals(_bits, bits._bits) &&
                   Length == bits.Length;
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


        /// <summary>
        ///     Get a normalized value of the bits1 * bits2 / Length
        ///     This is used to set the relative expertise or belief of an agent vs another
        /// </summary>
        /// <param name="bits1"></param>
        /// <param name="bits2"></param>
        /// <returns></returns>
        public static float GetRelativeBits(Bits bits1, Bits bits2)
        {
            if (bits1 == null)
            {
                throw new ArgumentNullException(nameof(bits1));
            }

            if (bits2 == null)
            {
                throw new ArgumentNullException(nameof(bits2));
            }

            var relativeKnowledgeBits = 0F;
            for (byte i = 0; i < bits1.Length; i++)
            {
                relativeKnowledgeBits += bits1.GetBit(i) * bits2.GetBit(i);
            }

            return bits1.Length > 0 ? relativeKnowledgeBits / bits1.Length : 0;
        }
    }
}