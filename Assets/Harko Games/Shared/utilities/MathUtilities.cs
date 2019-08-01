using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames.Utilities
{
    /// <summary>
    /// many from https://wiki.unity3d.com/index.php/3d_Math_functions
    /// </summary>
    public static class MathUtilities
    {
        //Calculate the intersection point of two lines. Returns true if lines intersect, otherwise false.
        //Note that in 3d, two lines do not intersect most of the time. So if the two lines are not in the 
        //same plane, use ClosestPointsOnTwoLines() instead.
        public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
        {

            Vector3 lineVec3 = linePoint2 - linePoint1;
            Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
            Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

            float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

            //is coplanar, and not parrallel
            if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
            {
                float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
                intersection = linePoint1 + (lineVec1 * s);
                return true;
            }
            else
            {
                intersection = Vector3.zero;
                return false;
            }
        }

        public static Vector3 GetMidpointBetweenTwoDirectionVectors(Transform v1, Transform v2, float distance)
        {
            var check1 = v1.position + v1.forward * distance;
            var check2 = v2.position + v2.forward * distance;
            return GetMidpoint(check1, check2);
        }

        public static Vector3 GetMidpoint(Vector3 v1, Vector3 v2)
        {
            var result = new Vector3();
            result.x = v2.x - v1.x;
            result.y = v2.y - v1.y;
            result.z = v2.z - v1.z;

            return result;
        }
    }
}
