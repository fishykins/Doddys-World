using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW {
	public static class MathDW {
        #region Variables

        #endregion;

        #region Custom Methods
        /// <summary>
        /// Rounds a float to the nearest division of an int.
        /// </summary>
        /// <param name="number">The float to round</param>
        /// <param name="intDivisions">number of divisions within the int</param>
        /// <returns>float rounded to the nearest nth of an int</returns>
        public static float RoundToFloat(float number, float intDivisions)
        {
            return (Mathf.RoundToInt(number * intDivisions)) / intDivisions;
        }

        /// <summary>
        /// Gets the volume of a sphere with given radius
        /// </summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static float GetSphereVolume(float radius)
        {
            //true formula is (4f / 3f) * Mathf.PI * Mathf.Pow(radius, 3);
            return 4.18879f * Mathf.Pow(radius, 3);
        }

        public static float GetSphereSurfaceArea(float radius)
        {
            return 12.56637f * radius * radius;
        }

        /// <summary>
        /// Checks to see if two floats are within a certian range of eachother. 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static bool ApproximatelyEqual(float a, float b, float range = 1f)
        {
            return (a < b + range && a > b - range);
        }

        /// <summary>
        /// Checks to see if two floats have the same sign (+/-)
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EqualSign(float a, float b)
        {
            return (Mathf.Sign(a) == Mathf.Sign(b));
        }
        #endregion
    }
}
