using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Battle
{
    public class BodyStanceData : MonoBehaviour
    {
        public Transform RotationPoint;
        public Collider BodyCollider;
        public GameCamera FollowCam;
        public GameCamera AimCam;
        public bool StickToGroundAngle;
        public float AngleSpeedAdjust;
        public float SpeedFactor;
    }
}
