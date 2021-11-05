using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest/Quest")]
public class Quest : ScriptableObject
{
    public int idQuest;
    public GameObject ownerQuest;
    public int levelQuest;
    public int takeQuest;
    public List<InventoryItem> rewardItem;
    public List<EnemyDeathQuest> enemyDeathQuests;
}
