using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

namespace UnityEngine.UI
{
    public class DropDown : MonoBehaviour
    {

        public Transform OptionRoot;

        public GameObject myButton;
        public Text myButtonText;

        public GameObject optionButtonPrefab;

        private List<GameObject> optionButtons = new List<GameObject>();

        public DropDownOption[] options;

        private bool isOpen = false;

        public void ToggleDropDown()
        {
            if (isOpen)
            {
                Close();
            }
            else
            {
                Open();
            }
        }

        public void Open()
        {
            int lcv = 0;
            foreach(DropDownOption o in options)
            {
                if (optionButtons.Count <= lcv)
                {
                    var newButton = GameObject.Instantiate(optionButtonPrefab) as GameObject;
                    newButton.transform.SetParent(OptionRoot);
                    newButton.name = string.Format("option {0}", lcv);
                    optionButtons.Add(newButton);
                }
                var btn = optionButtons[lcv];
                btn.SetActive(true);
                var ddBtnScript = btn.GetComponent<DropDownButton>();
                ddBtnScript.Label = o.Label;
                ddBtnScript.Value = o.Value;
                ddBtnScript.Init();
                lcv++;
            }
            isOpen = true;
        }

        public void Close()
        {
            foreach (var go in optionButtons)
            {
                go.SetActive(false);
            }
            isOpen = false;
        }

        private object mValue;

        public object Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;
            }
        }
    }

    [Serializable]
    public class DropDownOption
    {
        public int Value;
        public string Label;
    }
}
