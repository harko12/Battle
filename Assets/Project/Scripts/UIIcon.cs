using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
    public RawImage IconImage;
    public RawImage SelectedImage;

    [SerializeField]
    private bool IsSelected = false;

    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Clicked()
    {

    }

    public void Selected(bool selected)
    {
        IsSelected = selected;
        SelectedImage.gameObject.SetActive(selected);
    }
}
