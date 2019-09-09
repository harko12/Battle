using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[ExecuteInEditMode]
public class BodyShapeCollider : MonoBehaviour
{
    public Transform RootTransform;


    // Update is called once per frame
    void Update()
    {
        transform.position = RootTransform.position;
    }
}
