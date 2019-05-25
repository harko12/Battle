using UnityEngine;
using System.Collections;

public class MenuBase : MonoBehaviour {

    RectTransform myRect;


    // Use this for initialization
	void Start () {
        myRect = GetComponent<RectTransform>();
        myRect.offsetMax = myRect.offsetMin = Vector2.zero; // move the stuff to the center
	
	}
	
}
