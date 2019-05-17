using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour {
    [SerializeField] private GameObject target;
    [SerializeField] private GameObject rotationAnchorObject;
    [SerializeField] private Vector3 translationOffset;
    [SerializeField] private Vector3 followOffset;
    [SerializeField] private float maxViewingAngle;
    [SerializeField] private float minViewingAngle;
    [SerializeField] private float rotationSensitivity;

    private float verticalRotationAngle;

    public Transform GetRotationAnchor()
    {
        return rotationAnchorObject.transform;
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
        if (target == null) { return; }
        // Make the camera look at the target.
        float yAngle = target.transform.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, yAngle, 0);

        transform.position = target.transform.position - (rotation * followOffset);
        transform.LookAt(target.transform.position + translationOffset);

        // Make the camera look up or down.
        verticalRotationAngle = Mathf.Clamp(verticalRotationAngle + Input.GetAxis("Mouse Y") * rotationSensitivity, minViewingAngle, maxViewingAngle);

        transform.RotateAround(rotationAnchorObject.transform.position, rotationAnchorObject.transform.right, -verticalRotationAngle);
    }
}
