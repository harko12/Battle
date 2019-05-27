using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class InputDialog : InputItemDisplay {

    public static InputDialog instance;

    void Start()
    {
        ShowDialog(false);
    }

    public static void ShowDialog(bool show)
    {
        instance.ModalBackground.enabled = instance.IsModal;
        var grp = instance.GetComponent<CanvasGroup>();
        grp.alpha = (show ? 1 : 0);
        grp.interactable = show;
        grp.blocksRaycasts = show;
    }
    

    public string DialogName;
    public bool IsModal;
    public Image ModalBackground;

    public UnityEngine.UI.Button OkButton;

    protected override void Awake()
    {
        base.Awake();
        instance = this;
    }

    public void OpenDialog(List<InputItem> itemList, UnityEngine.Events.UnityAction okFunc)
    {
        items = itemList;
        Init();
        OkButton.onClick.AddListener(okFunc);
        ShowDialog(true);
    }

    public void CloseDialog()
    {
        ShowDialog(false);
    }
}
