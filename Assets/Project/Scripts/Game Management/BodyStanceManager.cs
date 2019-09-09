using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Battle
{

    public enum BodyStance { Prone, Crouch, Upright }

    public delegate void StanceChangedHandler(BodyStance newStance);

    public class BodyStanceManager : MonoBehaviour
    {
        public BodyStanceData[] stanceData;
        public BodyStance Stance;
        public event StanceChangedHandler OnStanceChanged;

        private void Start()
        {
            SetStance(Stance);
        }

        public BodyStanceData CurrentStanceData()
        {
            return GetStanceData(Stance);
        }

        public BodyStanceData GetStanceData(BodyStance s)
        {
            return stanceData[(int)s];
        }

        public void StanceMove(bool up)
        {
            var next = (int)Stance;
            if (up)
            {
                next++;
                if (next > 2) { next = 2; }
            }
            else
            {
                next--;
                if (next < 0) { next = 0; }
            }
            if (CheckStance((BodyStance)next))
            {
                SetStance((BodyStance)next);
            }
        }

        public bool CheckStance(BodyStance checkStance)
        {
            var coll = (CapsuleCollider)stanceData[(int)checkStance].BodyCollider;
            var pos = transform.position + coll.center;
            var dir = (coll.direction == 0 ? transform.right : coll.direction == 1 ? transform.up : transform.forward);
            
            var distanceFromCenter = coll.height * .5f - coll.radius;
            var sphere1Pos = pos + dir * distanceFromCenter;
            var sphere2Pos = pos - dir * distanceFromCenter;

            var r = new Ray(transform.position, dir);
            Debug.DrawRay(transform.position, dir, Color.red, coll.height);
            var hits = Physics.RaycastAll(r, coll.height);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("BodyStance"))
                {
                    continue;
                }
                return false;
            }
            return true;
        }

        public void SetStance(BodyStance newStance)
        {
            stanceData[(int)newStance].BodyCollider.enabled = true;
            if (newStance != Stance)
            {
                stanceData[(int)newStance].BodyCollider.enabled = true;
                stanceData[(int)Stance].BodyCollider.enabled = false;
                Stance = newStance;
                OnStanceChanged.Invoke(Stance);
            }
        }

    }
}