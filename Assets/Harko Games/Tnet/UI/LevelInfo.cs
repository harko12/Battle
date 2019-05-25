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
public class LevelInfo : ScriptableObject {

    public int MaxPlayers;
    public string RoomName;
    public string LevelName;

    public const string INPUT_MaxPlayers = "Max Players";
    public const string INPUT_RoomName = "Room Name";
    public const string INPUT_LevelName = "Level Name";
    public const string INPUT_TeamCount = "Team Count";

    public List<TeamDefinition> TeamDefinitions;

    public List<InputItem> GetInputItemList()
    {
        var list = new List<InputItem>();
        list.Add(new InputItem() { InputName = INPUT_RoomName, InputType = typeof(string).Name, InputValue = RoomName });
        list.Add(new InputItem() { InputName = INPUT_MaxPlayers, InputType = typeof(int).Name, InputValue = MaxPlayers });
        list.Add(new InputItem() { InputName = INPUT_LevelName, InputType = typeof(string).Name, InputValue = LevelName });
        list.Add(new InputItem() { InputName = INPUT_TeamCount, InputType = typeof(int).Name, InputValue = TeamDefinitions.Count });
        return list;
    }

}
