using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fishy {
	public static class Math3D {

        //increase or decrease the length of vector by size
        public static Vector3 AddVectorLength(Vector3 vector, float size)
        {

            //get the vector length
            float magnitude = Vector3.Magnitude(vector);

            //calculate new vector length
            float newMagnitude = magnitude + size;

            //calculate the ratio of the new length to the old length
            float scale = newMagnitude / magnitude;

            //scale the vector
            return vector * scale;
        }

        //create a vector of direction "vector" with length "size"
        public static Vector3 SetVectorLength(Vector3 vector, float size)
        {

            //normalize the vector
            Vector3 vectorNormalized = Vector3.Normalize(vector);

            //scale the vector
            return vectorNormalized *= size;
        }

        //Get the intersection between a line and a plane. 
        //If the line and plane are not parallel, the function outputs true, otherwise false.
        public static bool LinePlaneIntersection(out Vector3 intersection, Vector3 linePoint, Vector3 lineVec, Vector3 planeNormal, Vector3 planePoint)
        {

            float length;
            float dotNumerator;
            float dotDenominator;
            Vector3 vector;
            intersection = Vector3.zero;

            //calculate the distance between the linePoint and the line-plane intersection point
            dotNumerator = Vector3.Dot((planePoint - linePoint), planeNormal);
            dotDenominator = Vector3.Dot(lineVec, planeNormal);

            //line and plane are not parallel
            if (dotDenominator != 0.0f) {
                length = dotNumerator / dotDenominator;

                //create a vector from the linePoint to the intersection point
                vector = SetVectorLength(lineVec, length);

                //get the coordinates of the line-plane intersection point
                intersection = linePoint + vector;

                return true;
            }

            //output not valid
            else {
                return false;
            }
        }
    }
}
