using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideMenu : InputItemDisplay
{
    Animator myAnim;
    public InputItemSource itemSource;
    public bool IsShowing = false;

    protected override void Awake()
    {
        base.Awake();
        myAnim = GetComponent<Animator>();
        if (itemSource != null)
        {
            items = itemSource.GetInputItemList();
            Init();
        }
    }

    public void Toggle()
    {
        IsShowing = !IsShowing;
        myAnim.SetTrigger("Toggle");
    }

    public void AcceptAndClose(SlideMenu prevMenu)
    {
        AcceptValues();
        itemSource.SetValues(items);
        UIController.instance.TogglePanel(prevMenu);
    }
}
