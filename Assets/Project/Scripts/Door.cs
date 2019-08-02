using System.Collections;
using System.Collections.Generic;
using TNet;
using UnityEngine;

namespace Battle
{

    public class Door : Interactable, IInteractable
    {
        [SerializeField] public Vector3 openAngle = new Vector3(0, -90f, 0);
        [SerializeField] public float openSpeed = 2f;
        private Quaternion initial;
        private Quaternion targetAngle;
        private bool moving, isOpen;
        public bool touched;
        // Start is called before the first frame update
        void Start()
        {
            initial = transform.localRotation;
        }

        // Update is called once per frame
        void Update()
        {
            if (touched)
            {
                Interact(null);
                touched = false;
            }

            if (moving)
            {
                var currentRot = transform.localRotation;
                //var rot = Quaternion.Lerp(currentRot, targetAngle, openSpeed * Time.deltaTime);
                var rot = Quaternion.RotateTowards(currentRot, targetAngle, openSpeed);
                if (transform.localRotation == targetAngle) { moving = false; }
                transform.localRotation = rot;
            }
        }

        public override void Interact(BattlePlayer p)
        {
            if (!moving)
            {
                tno.Send("TriggerInteraction", Target.AllSaved);
            }
        }

        [RFC]
        public void TriggerInteraction()
        {
            if (isOpen)
            {
                targetAngle = initial;
            }
            else
            {
                targetAngle = Quaternion.Euler(openAngle);
            }
            moving = true;
            isOpen = !isOpen;
        }
    }
}
