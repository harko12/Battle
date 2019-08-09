using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Battle;
using UnityEngine.Events;

public class SlideMenu : InputItemDisplay
{
    Animator myAnim;
    public InputItemSource itemSource;
    public bool IsShowing = false;
    //[SerializeField]
    //public UnityEvent OnMenuClosed;

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
        itemSource.Updated();
        //OnMenuClosed.Invoke();
    }
}
