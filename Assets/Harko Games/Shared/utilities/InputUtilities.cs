using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HarkoGames.Utilities
{
    public static class InputUtilities
    {
        public static Quaternion CalculateFacingDirection(Transform t, Vector3 dr, float RotateSpeed)
        {
            Quaternion facingDir = new Quaternion();
            //message += "\ninitial dr: " + dr.ToString();
            if (dr != Vector3.zero) // only do rotation if there is actually some input
            {
                /* handle facing direction - taken from charactermotor code in gta shooter example*/
                dr = (dr.magnitude > 1 ? dr.normalized : dr);
                // Calculate which way character should be facing
                float facingWeight = dr.magnitude;
                Vector3 combinedFacingDirection = (
                    t.rotation * dr * (1 - facingWeight)
                    + dr * facingWeight
                );
                //message += " combined Facing Direction: " + combinedFacingDirection.ToString();
                combinedFacingDirection = combinedFacingDirection - Vector3.Project(combinedFacingDirection, t.up); //Util.ProjectOntoPlane(combinedFacingDirection, transform.up);
                combinedFacingDirection = .5f * combinedFacingDirection; // alignCorrection .5f ?

                if (combinedFacingDirection.sqrMagnitude > 0.01f)
                {
                    //message += string.Format(" updating facingDir: {0} to ", facingDir);
                    Vector3 newForward = Vector3.Slerp(t.forward, combinedFacingDirection, RotateSpeed * Time.deltaTime);//Util.ConstantSlerp(transform.forward, combinedFacingDirection, RotateSpeed * Time.deltaTime);
                    newForward = newForward - Vector3.Project(newForward, t.up);//Util.ProjectOntoPlane(newForward, transform.up);   

                    Vector3 drawPosition = t.position + new Vector3(0, 2, 0);
                    Debug.DrawLine(drawPosition, drawPosition + newForward, Color.yellow);

                    facingDir.SetLookRotation(newForward, t.up);
                    //message += string.Format("{0} ", facingDir);
                }
            }
            else
            {
                // message += " keeping rotation: " + transform.rotation.ToString();
                //facingDir = myAnim.bodyRotation; // don't change if no input vector was sent
                facingDir = t.rotation; // don't change if no input vector was sent
            }
            //message += " dr: " + dr + " facingDir: " + facingDir.ToString() + "\n";
            return facingDir;
        }

        public static float GetFacingDifference(Quaternion rotA, Quaternion rotB)
        {
            // found on unity answers http://answers.unity3d.com/questions/26783/how-to-get-the-signed-angle-between-two-quaternion.html

            // get a "forward vector" for each rotation
            var forwardA = rotA * Vector3.forward;
            var forwardB = rotB * Vector3.forward;

            return GetFacingDifference(forwardA, forwardB);
        }

        public static float GetFacingDifference(Vector3 rotA, Vector3 rotB)
        {
            // found on unity answers http://answers.unity3d.com/questions/26783/how-to-get-the-signed-angle-between-two-quaternion.html
            float diff = 0f;

            // get a "forward vector" for each rotation
            var forwardA = rotA.normalized;
            var forwardB = rotB.normalized;

            // get a numeric angle for each vector, on the X-Z plane (relative to world forward)
            var angleA = Mathf.Atan2(forwardA.x, forwardA.z) * Mathf.Rad2Deg;
            var angleB = Mathf.Atan2(forwardB.x, forwardB.z) * Mathf.Rad2Deg;

            // get the signed difference in these angles
            diff = Mathf.DeltaAngle(angleA, angleB);

            return diff;
        }

        /// <summary>
        /// Get the transformed velocity based on the camera position, the character position, and the input vector
        /// </summary>
        /// <param name="cameraTransform"></param>
        /// <param name="targetTransform"></param>
        /// <param name="inputVector"></param>
        /// <returns></returns>
        public static Vector3 GetDesiredVelocity(Transform cameraTransform, Transform targetTransform, Vector3 inputVector)
        {
            // make the input vector magnitude >= 1
            Vector3 directionVector = (inputVector.magnitude > 1 ? inputVector.normalized : inputVector);
            directionVector = directionVector.normalized * Mathf.Pow(directionVector.magnitude, 2);

            // rotate the input vector into camera space so up is camera's up and right is camera's right
            directionVector = cameraTransform.rotation * directionVector;

            // make input to be perpindicular to character's up vector
            Quaternion camToCharSpace = Quaternion.FromToRotation(cameraTransform.forward * -1, targetTransform.up);
            directionVector = (camToCharSpace * directionVector);

            return directionVector;
        }

        public static Vector3 GetDesiredFacingDirection(Transform cameraTransform, Transform targetTransform, Vector3 inputVector)
        {
            // Get input vector from kayboard or analog stick and make it length 1 at most
            Vector3 directionVector = inputVector;
            //if (directionVector.magnitude > 1) 
            directionVector = directionVector.normalized;

            // Rotate input vector into camera space so up is camera's up and right is camera's right
            directionVector = cameraTransform.rotation * directionVector;

            // Rotate input vector to be perpendicular to character's up vector
            Quaternion camToCharacterSpace = Quaternion.FromToRotation(cameraTransform.forward * -1, targetTransform.up);
            directionVector = (camToCharacterSpace * directionVector);
            //Debug.Log("DirectionVector: " + directionVector.ToString());
            return directionVector;
        }

        public static Vector3 GetMouseLookRotation(Transform cameraTransform, Vector3 inputVector)
        {
            if (cameraTransform != null)
            {
                return cameraTransform.rotation * Vector3.forward;
            }
            else
            {
                Debug.LogWarning("Attempting to use Mouselook but no camera is set.");
                return Vector3.zero;
            }
        }
    }
}

