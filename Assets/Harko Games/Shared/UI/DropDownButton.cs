using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropDownButton : MonoBehaviour
{
    public DropDown parentList;

    public int Value;
    public string Label;

    public void Init()
    {
        var myText = GetComponentInChildren<Text>();
        myText.text = Label;
    }

    public void SetValue()
    {
        parentList.myButtonText.text = Label;
        parentList.Value = Value;
        parentList.Close();
    }
}