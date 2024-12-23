using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Text;
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
        [SerializeField] private string uniqueId = "";
		[SerializeField] private bool moving, isOpen;
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
				tno.Send(nameof(TriggerInteraction) + "/" + uniqueId, TNet.Target.AllSaved);
            }
        }

		//public string GetFullPath(Transform tr)
		//{
		//	var parents = tr.GetComponentsInParent<Transform>();

		//	var str = new StringBuilder(parents[^1].name);
		//	for (var i = parents.Length - 2; i >= 0; i--)
  //              str.Append($"/{parents[1].name}");
		//    return str.ToString();
		//}

		[RFC("uniqueId")]
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
