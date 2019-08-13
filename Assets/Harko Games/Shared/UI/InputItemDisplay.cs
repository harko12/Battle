using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine.UI;

public class InputItemDisplay : MonoBehaviour
{
    public Transform InputListRoot;
    public List<InputItem> items;
    public GameObject InputLinePrefab;

    List<GameObject> InputLines = new List<GameObject>();

    protected virtual void Awake()
    {
    }

    public void SetupSlider(Slider s, InputItem i)
    {
        s.gameObject.SetActive(true);
        s.maxValue = i.Max;
        s.minValue = i.Min;
        s.wholeNumbers = true;
        switch (i.InputType.ToString().ToUpper())
        {
            case "INT":
            case "INT32":
                s.value = (int)i.InputValue;
                break;
            case "FLOAT":
            case "SINGLE":
            case "DECIMAL":
                s.value = (float)i.InputValue;
                break;
        }
    }

    public void Init()
    {
        if (items == null)
        {
            return;
        }
        // get cached lines out of the way
        foreach (GameObject go in InputLines)
        {
            go.SetActive(false);
        }

        var lcv = 0;
        foreach (InputItem i in items)
        {
            GameObject line = null;
            if (InputLines.Count <= lcv)
            {
                line = GameObject.Instantiate(InputLinePrefab) as GameObject;
                line.transform.SetParent(InputListRoot);
                InputLines.Add(line);
            }
            line = InputLines[lcv];
            line.SetActive(true);
            var text = line.GetComponentInChildren<Text>();
            var input = line.GetComponentInChildren<InputField>();
            input.gameObject.SetActive(false);
            var checkbox = line.GetComponentInChildren<Toggle>();
            if (checkbox != null)
            {
                checkbox.gameObject.SetActive(false);
            }
            var slider = line.GetComponentInChildren<Slider>();
            if (slider != null)
            {
                slider.gameObject.SetActive(false);
            }
            text.text = i.InputName;
            switch (i.InputType.ToString().ToUpper())
            {
                case "STRING":
                    input.gameObject.SetActive(true);
                    input.contentType = InputField.ContentType.Alphanumeric;
                    input.text = string.Format("{0}", i.InputValue);
                    break;
                case "INT":
                case "INT32":
                case "FLOAT":
                case "SINGLE":
                case "DECIMAL":
                    if (i.ShowSlider)
                    {
                        SetupSlider(slider, i);
                    }
                    else
                    {
                        input.gameObject.SetActive(true);
                        input.text = string.Format("{0}", i.InputValue);
                        input.contentType = InputField.ContentType.DecimalNumber;
                    }
                    break;
                case "ENUM":
                    break;
                case "BOOLEAN":
                    if (checkbox != null)
                    {
                        checkbox.gameObject.SetActive(true);
                        checkbox.isOn = (bool)i.InputValue;
                    }
                    break;
                default:
                    Debug.LogError("InputDialog unable to parse type " + i.InputType.ToString().ToUpper());
                    break;

            }
            lcv++;
        }
    }

    public void AcceptValues()
    {
        var lcv = 0;
        foreach (InputItem i in items)
        {
            GameObject line = null;
            line = InputLines[lcv];
            var text = line.GetComponentInChildren<Text>();
            var input = line.GetComponentInChildren<InputField>();
            var checkbox = line.GetComponentInChildren<Toggle>();
            var slider = line.GetComponentInChildren<Slider>();
            text.text = i.InputName;
            var t = i.InputType.ToString().ToUpper();
            switch (t)
            {
                case "STRING":
                    i.InputValue = input.text;
                    break;
                case "INT":
                case "INT32":
                    int intVal;
                    if (slider != null && slider.isActiveAndEnabled)
                    {
                        intVal = Convert.ToInt32(slider.value);
                    }
                    else if (!int.TryParse(input.text, out intVal))
                    {
                        throw new Exception(string.Format("Unable to parse {0} to {1}", input.text, t));
                    }
                    i.InputValue = intVal;
                    break;
                case "FLOAT":
                case "SINGLE":
                    float fVal;
                    if (slider != null && slider.isActiveAndEnabled)
                    {
                        fVal = slider.value;
                        if (i.CoerceTo01)
                        {
                            fVal = fVal / slider.maxValue;
                        }
                    }
                    else if (!float.TryParse(input.text, out fVal))
                    {
                        throw new Exception(string.Format("Unable to parse {0} to {1}", input.text, t));
                    }
                    i.InputValue = fVal;
                    break;
                case "DECIMAL":
                    decimal dVal;
                    if (slider != null && slider.isActiveAndEnabled)
                    {
                        dVal = Convert.ToDecimal(slider.value);
                        if (i.CoerceTo01)
                        {
                            dVal = dVal / Convert.ToDecimal(slider.maxValue);
                        }
                    }
                    else if (!decimal.TryParse(input.text, out dVal))
                    {
                        throw new Exception(string.Format("Unable to parse {0} to {1}", input.text, t));
                    }
                    i.InputValue = dVal;
                    break;
                case "BOOLEAN":
                    i.InputValue = checkbox.isOn;
                    break;
                default:
                    Debug.LogError("InputDialog unable to parse type " + t);
                    break;
            }
            lcv++;
        }
    }

    public object GetValue(string inputName)
    {
        var item = items.Where(i => i.InputName == inputName).FirstOrDefault();
        if (item != null)
            return item.InputValue;
        else
            return null;
    }

    public string GetValueString(string inputName)
    {
        string result = "";
        var val = GetValue(inputName);
        result = (val != null ? val.ToString() : "");
        return result;
    }

    public int GetValueInt(string inputName)
    {
        var val = GetValue(inputName);
        return val == null ? 0 : (int)val;
    }
}

public class InputItem
{
    public string InputName { get; set; }
    public string InputType { get; set; }
    public object InputValue { get; set; }
    public bool ReadOnly { get; set; }
    public bool ShowSlider { get; set; }
    public int Max { get; set; }
    public int Min { get; set; }
    public int Step { get; set; }
    public bool CoerceTo01 { get; set; }

    public InputItem()
    {
        ReadOnly = false;
    }

}