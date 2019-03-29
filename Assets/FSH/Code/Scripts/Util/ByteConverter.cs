using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
	public static class ByteConverter {
		public static byte BoolArrayToByte(bool[] source)
        {
            byte result = 0;
            int index = 8 - source.Length;

            // Loop through the array
            foreach (bool b in source) {
                // if the element is 'true' set the bit at that position
                if (b)
                    result |= (byte)(1 << (7 - index));

                index++;
            }

            return result;
        }

        /// <summary>
        /// Converts a byte into an array of bools
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool[] ByteToBoolArray(byte b, int count = 8)
        {
            // prepare the return result
            bool[] result = new bool[count];

            // check each bit in the byte. if 1 set to true, if 0 set to false
            for (int i = 0; i < count; i++)
                result[i] = (b & (1 << i)) == 0 ? false : true;

            // reverse the array
            //Array.Reverse(result);

            return result;
        }
    }
}
