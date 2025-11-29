using System.Collections;
using System.Collections.Generic;
using MessagePack;
using UnityEngine;

[MessagePackObject(true)]
public class BattleData
{
    public int[] EnemyIds { get; set; }
    public string BGM { get; set; }
    public string EncounterMessage { get; set; }
    public KeyValuePair<string, bool>[] WinFlags { get; set; }
    public KeyValuePair<string, bool>[] LoseFlags { get; set; }
}
