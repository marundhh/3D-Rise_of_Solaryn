using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMinionData", menuName = "Minion/Minion Data")]
public class MinionData : ScriptableObject
{
    public string enemyID;
    public string displayName;
}
