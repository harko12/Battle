using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using System.Linq;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class InputItemAttribute: Attribute
{
    public string Label;
    public int SortOrder;

    public InputItemAttribute(string label, int sortOrder)
    {
        this.Label = label;
        this.SortOrder = sortOrder;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class RangeInputItemAttribute : InputItemAttribute
{
    public int Min, Max, Step;
    public bool CoerceTo01;
    public RangeInputItemAttribute(string label, int sortOrder, int min, int max, int step, bool coerce) : base(label,sortOrder)
    {
        Min = min;
        Max = max;
        Step = step;
        CoerceTo01 = coerce;
    }

}

public abstract class InputItemSource : ScriptableObject
{
    /// <summary>
    /// Set this value to a unique key, if you want the player to be able to update these values
    /// </summary>
    public virtual string DataKey { get { return null; } }
    public bool Updatable { get { return string.IsNullOrEmpty(DataKey); } }

    public abstract List<InputItem> GetInputItemList();
    private void Awake()
    {
        if (Updatable)
        {
            var storedJson = PlayerPrefs.GetString(DataKey);
            if (storedJson == null) { return; }
            JsonUtility.FromJsonOverwrite(storedJson, this);
        }
    }

    public void SetValues(List<InputItem> values)
    {
        var inputItemProps = values.ToDictionary(i => i.InputName, i => i);
        Type t = GetType();

        foreach (var field in t.GetFields())
        {
            var propAttrs = field.GetCustomAttributes(typeof(InputItemAttribute), true);
            foreach (var attr in propAttrs)
            {
                var item = (InputItemAttribute)attr;
                var val = inputItemProps[item.Label];
                if (val.ReadOnly)
                {
                    continue;
                }
                field.SetValue(this, val.InputValue);
            }
        }
        if (Updatable)
        {
            PlayerPrefs.SetString(DataKey, JsonUtility.ToJson(this));
        }
    }

    public List<InputItem> GetInputItemListFromProperties<T>()
    {
        var result = new List<InputItem>();
        Type t = typeof(T);

        foreach (var field in t.GetFields())
        {
            var propAttrs = field.GetCustomAttributes(typeof(InputItemAttribute), true);
            foreach (var attr in propAttrs)
            {
                var item = (InputItemAttribute)attr;
                var entry = new InputItem()
                {
                    InputName = item.Label,
                    InputType = field.FieldType.Name,
                    InputValue = field.GetValue(this)
                };
                if (attr is RangeInputItemAttribute)
                {
                    var rangeAttr = (RangeInputItemAttribute)attr;
                    if (rangeAttr != null)
                    {
                        entry.Max = rangeAttr.Max;
                        entry.Min = rangeAttr.Min;
                        entry.CoerceTo01 = rangeAttr.CoerceTo01;
                        entry.Step = rangeAttr.Step;
                        entry.ShowSlider = true;
                    }
                }
                result.Add(entry);
            }
        }
        return result;
    }

    public virtual void Updated()
    {

    }
}
