using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettings")]
public class GameSettings : ScriptableObject
{
    public int WolfCount = 20;
    public int DiamondCount = 20;
    public int StartDelaySeconds = 3;
}
