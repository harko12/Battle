using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCameraRig : MonoBehaviour {
    [SerializeField] private Transform followTransform;
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject rotationAnchorObject;
    //public GameObject GlobalRotationAnchor;
    //public GameObject DefaultTarget;
    [SerializeField] private Vector3 translationOffset;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private float maxViewingAngle;
    [SerializeField] private float minViewingAngle;
//    [SerializeField] private float rotationSensitivity;

    private float verticalRotationAngle, horizontalRotationAngle;
    /*
    public Transform GetRotationAnchor()
    {
        if (rotationAnchorObject != null)
        {
            return rotationAnchorObject.transform;
        }
        return GlobalRotationAnchor.transform;
    }

    public Transform GetTarget()
    {
        if (target != null)
        {
            return target.transform;
        }
        return DefaultTarget.transform;
    }
    */
    public void Init(BattlePlayer p)
    {
        //target = p.focalPoint;
       // rotationAnchorObject = p.rotationPoint;
    }

    public void Release()
    {
        //target = null;
        //rotationAnchorObject = null;
    }

    private void FixedUpdate()
	{
        //verticalRotationAngle = 0;
        //transform.position = followTransform.position;
        if (target != null)
        {
            var t = target.transform; // GetTarget();
            // Make the camera look at the target.
            float yAngle = t.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, yAngle, 0);
            
            //transform.position = t.position - (rotation * followOffset);
            /*
            transform.LookAt(t.position + translationOffset);
            */
            var verticalInput = BattlePlayerInput.instance.VerticalAngle;
            var turnAmount = BattlePlayerInput.instance.m_TurnAmount;
            if (BattlePlayerInput.instance.Settings.InvertMouse)
            {
                verticalInput *= -1;
            }
            var rotationSensitivity = BattlePlayerInput.instance.Settings.MouseSensitivity;
            // Make the camera look up or down.
            verticalRotationAngle = Mathf.Clamp(t.eulerAngles.x + verticalInput * rotationSensitivity, minViewingAngle, maxViewingAngle);
//            verticalRotationAngle = Mathf.Clamp(verticalRotationAngle + verticalInput * rotationSensitivity, minViewingAngle, maxViewingAngle);
            var rot = rotationAnchorObject;//  GetRotationAnchor();
            var rotAngles = t.eulerAngles;
            rotAngles.x = verticalRotationAngle;
            transform.Rotate(rotAngles);
            //transform.RotateAround(rot.transform.position, rot.transform.right, -verticalRotationAngle);
            //Debug.LogFormat("rotating to vert angle {0}", verticalRotationAngle);
        }
        /*
        else
        {
            var rotTransform = rotationAnchorObject.transform;
            var anchor = rotTransform;
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
            transform.LookAt(DefaultTarget.transform.position);
        }
        */
    }
}
