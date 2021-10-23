using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;


public class Enemy : NetworkBehaviour
{
    [SyncVar] public string MatchID;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] enemyTag = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject musuh in enemyTag)
        {
            Physics2D.IgnoreCollision(musuh.GetComponent<Collider2D>(),GetComponent<Collider2D>());
        }
    }

    public void setMatchId(string matchId)
    {
        MatchID = matchId;

        ignoreDiffMatch();
    }

    public void ignoreDiffMatch()
    {
        GameObject[] pemainTag = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject pemain in pemainTag)
        {
            if (pemain.GetComponent<Player>().MatchID != MatchID)
            {
                Debug.Log("Ignore "+ pemain.GetComponent<Player>().MatchID);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(),pemain.GetComponent<Collider2D>(),true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void reduceHealth()
    {
        Destroy(this.gameObject);
    }
}
