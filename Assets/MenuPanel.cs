using UnityEngine;
using System.Collections;

public class MenuPanel : MonoBehaviour {
 
    void Awake()
    {
        var myRect = GetComponent<RectTransform>();
        myRect.offsetMax = myRect.offsetMin = Vector2.zero;

    }
}
