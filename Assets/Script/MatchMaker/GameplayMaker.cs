using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GameplayMaker : NetworkBehaviour
{
    public static GameplayMaker instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    public void LoadUnit(string matchId,NetworkConnection conn)
    {
        GameObject enemyClone = Instantiate(Resources.Load("Prefab/Enemy/Ifrit") as GameObject,new Vector3(-0.2570385f, -8.70766f,0),Quaternion.identity);
        
        enemyClone.GetComponent<NetworkMatch>().matchId = matchId.ToGuid();
        enemyClone.transform.position = new Vector3(-0.2570385f, -8.70766f,0);
        NetworkServer.Spawn(enemyClone,conn);
        enemyClone.GetComponent<Enemy>().setMatchId(matchId);
        
        //NPC
        GameObject npcClone = Instantiate(Resources.Load("Prefab/NPC/NPC_KakekTua") as GameObject);
        npcClone.GetComponent<NetworkMatch>().matchId = matchId.ToGuid();
        NetworkServer.Spawn(npcClone,conn);
    }

    public void DropItem(string matchId,NetworkConnection conn,Vector2 posisi)
    {
        GameObject itemClone = Instantiate(Resources.Load("Prefab/Item/Ayat") as GameObject);

        itemClone.transform.position = posisi;
        itemClone.GetComponent<NetworkMatch>().matchId = matchId.ToGuid();
        NetworkServer.Spawn(itemClone, conn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
