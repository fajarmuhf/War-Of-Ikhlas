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
        GameObject enemyClone = Instantiate(Resources.Load("Prefab/Enemy/Ifrit") as GameObject);

        Debug.Log("Spawn " + conn.connectionId);

        enemyClone.GetComponent<NetworkMatchChecker>().matchId = matchId.ToGuid();
        NetworkServer.Spawn(enemyClone,conn);
        enemyClone.transform.position = new Vector3(-0.2570385f, -8.70766f,0);
        enemyClone.GetComponent<Enemy>().setMatchId(matchId);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
