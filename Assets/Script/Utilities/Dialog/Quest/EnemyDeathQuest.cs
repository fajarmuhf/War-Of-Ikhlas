using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy Death Quest", menuName = "Quest/Enemy Death Quest")]
public class EnemyDeathQuest : ScriptableObject
{
    public string nameEnemy;
    public int deathRequestEnemy;
    public int deathEnemy;
}
