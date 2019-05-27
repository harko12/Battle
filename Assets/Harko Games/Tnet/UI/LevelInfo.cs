using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[System.Serializable]
public class TeamDefinition
{
    public string TeamName;
    public Color TeamColor;
}

[CreateAssetMenu]
public class LevelInfo : InputItemSource {
    [InputItem(INPUT_MaxPlayers, 0)]
    public int MaxPlayers;
    [InputItem(INPUT_RoomName, 1)]
    public string RoomName;
    [InputItem(INPUT_LevelName, 2)]
    public string LevelName;

    public const string INPUT_MaxPlayers = "Max Players";
    public const string INPUT_RoomName = "Room Name";
    public const string INPUT_LevelName = "Level Name";
    public const string INPUT_TeamCount = "Team Count";

    public List<TeamDefinition> TeamDefinitions;

    public override List<InputItem> GetInputItemList()
    {
        var list = GetInputItemListFromProperties<LevelInfo>();
        list.Add(new InputItem() { InputName = INPUT_TeamCount, InputType = typeof(int).Name, InputValue = TeamDefinitions.Count, ReadOnly = true });
        return list;
    }
}
