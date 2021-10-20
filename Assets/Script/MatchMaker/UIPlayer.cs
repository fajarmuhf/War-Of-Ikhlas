using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

/*
 * UI icon Player di Lobby
 * - berisi untuk merubah player index di UI icon
 */

public class UIPlayer : MonoBehaviour
{
    [SerializeField] Text text;

    Player player;

    public void setPlayer(Player player)
    {
        this.player = player;
        Debug.Log("Update to "+player.playerIndex.ToString());
        text.text = "Player " + player.playerIndex.ToString();
    }
}
