using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TurnManager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    List<Player> players = new List<Player>();

    public void AddPlayer(Player player)
    {
        players.Add(player);
    }
}
