using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Battle Game Settings", menuName ="Battle Game Settings")]
public class BattleGameSettings : InputItemSource
{
    [RangeInputItem(INPUT_MouseSensitivity, 0, 1, 15, 1, false)]
    public float MouseSensitivity;
    [InputItem(INPUT_InvertMouse, 1)]
    public bool InvertMouse;
    [RangeInputItem(INPUT_MasterVolume, 2, -30, 20, 1, false)]
    public float MasterVolume;
    [RangeInputItem(INPUT_SoundVolume, 3, -30, 20, 1, false)]
    public float SoundVolume;
    [RangeInputItem(INPUT_MusicVolume, 4, -30, 20, 1, false)]
    public float MusicVolume;

    public const string INPUT_MouseSensitivity = "Mouse Sensitivity";
    public const string INPUT_InvertMouse = "Invert Mouse Look";
    public const string INPUT_MasterVolume = "Master Volume";
    public const string INPUT_SoundVolume = "Sound Volume";
    public const string INPUT_MusicVolume = "Music Volume";

    public override List<InputItem> GetInputItemList()
    {
        var list = GetInputItemListFromProperties<BattleGameSettings>();
        return list;
    }

    public void UpdateSlider(Slider slider)
    {
        var textFields = slider.GetComponentsInChildren<Text>();
        // for now, hard coded to min(0) max(1) current(2)
        textFields[0].text = slider.minValue.ToString();
        textFields[1].text = slider.maxValue.ToString();
        textFields[2].text = slider.value.ToString();
    }

    public override void Updated()
    {
        base.Updated();
        BattleGameObjects.instance.gameEvents.OnSettingsUpdated.Invoke();
    }
}
