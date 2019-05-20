using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject rotationAnchorObject;
    public GameObject GlobalRotationAnchor;
    public GameObject DefaultTarget;
    [SerializeField] private Vector3 translationOffset;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private float maxViewingAngle;
    [SerializeField] private float minViewingAngle;
    [SerializeField] private float rotationSensitivity;

    private float verticalRotationAngle;

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

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
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
            // Make the camera look at the target.
            float yAngle = t.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0, yAngle, 0);

            transform.position = t.position - (rotation * followOffset);
            transform.LookAt(t.position + translationOffset);

            // Make the camera look up or down.
            verticalRotationAngle = Mathf.Clamp(verticalRotationAngle + Input.GetAxis("Mouse Y") * rotationSensitivity, minViewingAngle, maxViewingAngle);
            var rot = GetRotationAnchor();
            transform.RotateAround(rot.transform.position, rot.transform.right, -verticalRotationAngle);
        }
        else
        {
            var anchor = GetRotationAnchor().transform;
            transform.position = anchor.position;
            transform.rotation = anchor.rotation;
            transform.LookAt(DefaultTarget.transform.position);
        }
    }
}
