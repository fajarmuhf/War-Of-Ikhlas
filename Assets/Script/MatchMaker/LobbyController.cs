using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    public static LobbyController instance;

    [Header("Host Join")]
    [SerializeField] InputField joinInput;
    [SerializeField] List<Selectable> lobbySelectable = new List<Selectable>();
    [SerializeField] Button joinButton;
    [SerializeField] Canvas lobbyCanvas;
    [SerializeField] Canvas searchCanvas;

    [Header("Lobby")]
    [SerializeField] Transform UIPlayerParrent;
    [SerializeField] GameObject UIPlayerPrefab;
    [SerializeField] Button beginButton;

    [SerializeField] Text matchIDText;
    [SerializeField] GameObject beginGameButton;

    GameObject playerLobbyUI;

    bool searching = false;

    //fungsi yang dijalankan awal permainan
    void Start()
    {
        instance = this;

        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(joinButton.gameObject);
    }

    public void HostPrivate()
    {
        joinInput.interactable = false;
        lobbySelectable.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(false);
    }

    public void HostPublic()
    {
        joinInput.interactable = false;
        lobbySelectable.ForEach(x => x.interactable = false);

        Player.localPlayer.HostGame(true);
    }

    public void HostSuccess(bool success, string _matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            playerLobbyUI = spawnPlayerPrefab(Player.localPlayer);
            Player.localPlayer.playerLobbyUI = playerLobbyUI;
            beginGameButton.SetActive(true);
            matchIDText.text = _matchID;
            //Player.localPlayer.SpawnToPoint();

            //EventSystem.current.SetSelectedGameObject(null);
            //EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
        else
        {
            joinInput.interactable = true;
            lobbySelectable.ForEach(x => x.interactable = true);

        }
    }

    public void Join()
    {
        joinInput.interactable = false;
        lobbySelectable.ForEach(x => x.interactable = false);

        Player.localPlayer.JoinGame(joinInput.text.ToUpper());
    }
    public void JoinSuccess(bool success, string _matchID)
    {
        if (success)
        {
            lobbyCanvas.enabled = true;
            beginGameButton.SetActive(false);
            playerLobbyUI = spawnPlayerPrefab(Player.localPlayer);
            Player.localPlayer.addLobi();
            Player.localPlayer.playerLobbyUI = playerLobbyUI;
            matchIDText.text = _matchID;
            //Player.localPlayer.SpawnToPoint();

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
        else
        {
            joinInput.interactable = true;
            lobbySelectable.ForEach(x => x.interactable = true);

        }
    }
    public void BeginGame()
    {
        Player.localPlayer.BeginGame();
    }
    public GameObject spawnPlayerPrefab(Player player)
    {
        GameObject newUIPlayer = Instantiate(UIPlayerPrefab, UIPlayerParrent);
        newUIPlayer.GetComponent<UIPlayer>().setPlayer(player);
        newUIPlayer.transform.SetSiblingIndex(player.playerIndex - 1);

        return newUIPlayer;
    }

    public void SearchGame()
    {
        Debug.Log("Seaching Game");
        searchCanvas.enabled = true;
        StartCoroutine(SearchingForGame());
    }

    IEnumerator SearchingForGame()
    {
        searching = true;
        float currentTime = 1;
        while (searching)
        {
            if (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
            }
            else
            {
                currentTime = 1;
                Player.localPlayer.SearchGame();
            }
            yield return null;
        }
    }
    public void SearchSuccess(bool success, string _matchID)
    {
        if (success)
        {
            searchCanvas.enabled = false;
            JoinSuccess(success, _matchID);
            searching = false;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(beginButton.gameObject);
        }
    }

    public void SearchCancel()
    {
        searchCanvas.enabled = false;
        searching = false;
        lobbySelectable.ForEach(x => x.interactable = true);
    }

    public void DisconnectLobby()
    {
        if (playerLobbyUI != null)
            Destroy(playerLobbyUI);
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerLobbyUI != null)
                Destroy(GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerLobbyUI);
        }
        Player.localPlayer.DisconnectGame(1);

        lobbyCanvas.enabled = false;
        lobbySelectable.ForEach(x => x.interactable = true);
        beginGameButton.SetActive(false);
    }
    public void ChoicePlayer(int _typePlayer)
    {
        Player.localPlayer.Choice(_typePlayer);
    }

}
