using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Battle Game Settings", menuName ="Battle Game Settings")]
public class BattleGameSettings : InputItemSource
{
    [InputItem(INPUT_MouseSensitivity, 0)]
    public float MouseSensitivity;
    [InputItem(INPUT_InvertMouse, 1)]
    public bool InvertMouse;

    public const string INPUT_MouseSensitivity = "Mouse Sensitivity";
    public const string INPUT_InvertMouse = "Invert Mouse Look";

    public override List<InputItem> GetInputItemList()
    {
        var list = GetInputItemListFromProperties<BattleGameSettings>();
        return list;
    }
}
