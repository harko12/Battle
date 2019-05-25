using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputDialogManager : MonoBehaviour {
    public static InputDialogManager instance;
    public Transform CanvasTransform;
    private Hashtable CurrentDialogs = new Hashtable();

    public InputDialog CurrentDialog;
    
    public delegate void OkFunction();

    public void AddDialog(InputDialog dlg)
    {
        CurrentDialogs[dlg.name] = dlg;
    }

    public void RemoveDialog(string name)
    {
        if (CurrentDialogs.ContainsKey(name))
        {
            CurrentDialogs.Remove(name);
        }
    }

    public InputDialog GetDialog_disabled(string name)
    {
        GameObject go = null;
        if (!CurrentDialogs.ContainsKey(name))
        {
            go = GameObject.Instantiate(DialogPrefab) as GameObject;
            go.transform.SetParent(null);
            go.SetActive(false);
            CurrentDialogs[name] = go;
        }
        go = CurrentDialogs[name] as GameObject;
        var dlg = go.GetComponent<InputDialog>();

        return dlg;
    }

    public void OpenDialog_disabled(string name, List<InputItem> items)
    {
        var dlg = GetDialog_disabled(name);

        dlg.transform.SetParent(CanvasTransform);

        if (dlg.IsModal)
        {
            dlg.ModalBackground.enabled = true;
        }
        dlg.items = items;
        dlg.gameObject.SetActive(true);
    }

    public void CloseDialog_disabled(string name)
    {
        if (!CurrentDialogs.ContainsKey(name))
        {
            return; // no point continuing if there isn't a dialog
        }
        var go = GetDialog_disabled(name).gameObject;
        go.transform.SetParent(null);
        go.SetActive(false);
    }

    public GameObject DialogPrefab;

    void ShowDialog(bool show)
    {
        CurrentDialog.ModalBackground.enabled = CurrentDialog.IsModal;
        var grp = CurrentDialog.GetComponent<CanvasGroup>();
        grp.alpha = (show ? 1 : 0);
        grp.interactable = show;
        grp.blocksRaycasts = show;
    }
    
    public void OpenDialog(List<InputItem> items, UnityEngine.Events.UnityAction okFunc)
    {
        CurrentDialog.items = items;
        CurrentDialog.Init();
        CurrentDialog.OkButton.onClick.AddListener(okFunc);
        ShowDialog(true);
    }
    
    public void CloseDialog()
    {
        ShowDialog(false);
    }
    
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ShowDialog(false); // hide at startup
    }
    /***  **/
}
