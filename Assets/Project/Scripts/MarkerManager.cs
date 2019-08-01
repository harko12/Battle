using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerManager : SingletonMonoBehaviour<MarkerManager>
{
    public Marker MarkerPrefab;

    public static void PlaceMarker(Vector3 pos)
    {
        if (instance != null)
        {
            instance.Place(pos);
        }
    }

    public void Place(Vector3 pos)
    {
        var m = GameObject.Instantiate(MarkerPrefab, pos, Quaternion.identity, transform);
    }


}
