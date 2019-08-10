using Battle;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject rotationAnchorObject;
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

    public Transform GetRotationAnchor()
    {
        return rotationAnchorObject.transform;
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
            var t = GetTarget();
            var bpInput = BattlePlayerInput.instance;
            if (t == null || bpInput == null)
            {
                return;
            }

            // Make the camera look at the target.
            float yAngle = t.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, yAngle, 0);

            transform.position = t.position - (rotation * followOffset);
            transform.LookAt(t.position + translationOffset);

            var verticalInput = bpInput.VerticalAngle;
            if (Settings.InvertMouse)
            {
                verticalInput *= -1;
            }
            var rotationSensitivity = Settings.MouseSensitivity;
            // Make the camera look up or down.
            VerticalRotationAngle = Mathf.Clamp(VerticalRotationAngle + verticalInput * rotationSensitivity, minViewingAngle, maxViewingAngle);
            var rot = GetRotationAnchor();
            transform.RotateAround(rot.transform.position, rot.transform.right, -VerticalRotationAngle);
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
