using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NPCController : NetworkBehaviour
{
    [SyncVar] public int npcId;
    [SyncVar] public TextAssetValue dialogNpc;
    [SyncVar] public int introQuest;
    // Start is called before the first frame update
    void Start()
    {
        introQuest = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
