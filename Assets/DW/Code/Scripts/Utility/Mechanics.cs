using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DW {
	public static class Mechanics {
        #region Variables
        //Public & Serialized

        //Private

        #endregion;

        #region Properties

        #endregion;

        #region Custom Methods
        public static float CalculateTorque(float power, float rpm)
        {
            return (rpm != 0) ? power * 9549f / rpm : 0f;
        }

        public static float CalculateRPM(float power, float torque)
        {
            return (torque != 0) ? power * 9549f / torque : 0f;
        }

        public static float CalculatePower(float rpm, float torque)
        {
            return torque * rpm / 9549f;
        }
        #endregion
    }
}
