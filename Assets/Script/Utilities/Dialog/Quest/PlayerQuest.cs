using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Quest", menuName = "Quest/Player Quest")]
public class PlayerQuest : ScriptableObject
{
    public List<Quest> myQuest = new List<Quest>();
}
