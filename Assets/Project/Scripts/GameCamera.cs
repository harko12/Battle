using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private Transform rotationAnchorObject;
    public float focalDistance;
    [SerializeField] private Vector3 translationOffset;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private float maxViewingAngle;
    [SerializeField] private float minViewingAngle;
//    [SerializeField] private float rotationSensitivity;

    public float VerticalRotationAngle { get; set; }

    private BattleGameSettings Settings;

    private void Awake()
    {
        Settings = BattleGameObjects.instance.settings;
    }

    public Transform GetTarget()
    {
        return target.transform;
    }

    public void Init(BattlePlayer p)
    {
        target = p.focalPoint;
        rotationAnchorObject = p.rotationPoint;
    }

    public void Release()
    {
        target = null;
        rotationAnchorObject = null;
    }

    private void FixedUpdate()
	{
        if (target != null)
        {
            var bpInput = BattlePlayerInput.instance;
            var t = GetTarget();
            if (t == null || bpInput == null)
            {
                return;
            }
//            var rotAdjust = Quaternion.FromToRotation(Vector3.up, t.up);

            
            var tOffset = translationOffset;
            var fOffset = followOffset;

            // Make the camera look at the target.
            //float yAngle = t.eulerAngles.y;
            //            var eu = new Vector3(0, yAngle, 0);
            var eu = t.eulerAngles; // rotAdjust * new Vector3(0, yAngle, 0);

            Quaternion rotation = Quaternion.Euler(eu);

            transform.position = t.position - (rotation * fOffset);
            transform.LookAt(t.position + tOffset);

            var verticalInput = bpInput.VerticalAngle;
            if (Settings.InvertMouse)
            {
                verticalInput *= -1;
            }
            var rotationSensitivity = Settings.MouseSensitivity;
            // Make the camera look up or down.
            VerticalRotationAngle = Mathf.Clamp(VerticalRotationAngle + verticalInput * rotationSensitivity, minViewingAngle, maxViewingAngle);
            transform.RotateAround(rotationAnchorObject.position, rotationAnchorObject.transform.right, -VerticalRotationAngle);
        }
        else
        {
            //Debug.LogError("no target set for camera");
/*
            var anchor = GetRotationAnchor().transform;
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
            transform.LookAt(DefaultTarget.transform.position);
            */
    }
        var euler = transform.rotation.eulerAngles;
        transform.rotation.eulerAngles.Set(euler.x, euler.y, 0);
    }
}
