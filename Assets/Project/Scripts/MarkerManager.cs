using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : MonoBehaviour
{
    public static MarkerManager Instance;
    public Marker MarkerPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public void Place(Vector3 pos)
    {
        var m = GameObject.Instantiate(MarkerPrefab, pos, Quaternion.identity, transform);
    }


}
