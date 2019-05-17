using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="UI Values", fileName ="UI Values")]
public class UIValues : ScriptableObject
{
    public int ResourceCount;
    public string ToolMessage;
    public string WeaponStatus;
    public float PlayerHealth;
}
