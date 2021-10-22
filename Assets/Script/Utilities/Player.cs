using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [Header("Object setting")]
    public static Player localPlayer;
    public static Transform localTransformPlayer;

    [Header("GameObject setting")]
    public Camera MainCamera;
    public NetworkMatchChecker networkMatchChecker;
    public GameObject pivot;
    public GameObject playerLobbyUI;
    public FixedJoystick joystick;
    public Animator animator;

    [Header("SyncVar setting")]
    [SyncVar] public string MatchID;
    [SyncVar] public int playerIndex;
    [SyncVar] public string interaksi;
    [SyncVar] public string directionH;
    [SyncVar] public string directionV;
    [SyncVar] public string directionAttack;
    [SyncVar] public string directionRot;
    [SyncVar] public string directionRot2;
    [SyncVar] public float InputJX;
    [SyncVar] public float InputJY;
    [SyncVar] public int playerType;

    [Header("Player Settings")]
    public float speed;

    // Pada saat mulai koneksi client
    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isLocalPlayer)
        {
            //inisialisasi Object Player
            localPlayer = this;
            localTransformPlayer = transform;
            if (PlayerPrefs.HasKey("TipePemain"))
            {
                Debug.Log("Tipe : "+PlayerPrefs.GetInt("TipePemain"));
                Choice(PlayerPrefs.GetInt("TipePemain"));
            }
            directionH = "nothing";
            directionV = "nothing";
            directionAttack = "nothing";
            InputJX = 0;
            InputJY = 0;
        }
        else
        {
            //inisialisasi GameObject UI Player di Lobby
            playerLobbyUI = LobbyController.instance.spawnPlayerPrefab(this);
        }
    }

    //pada saat client disconnect
    public override void OnStopClient()
    {
        base.OnStopClient();
        Debug.Log("Client stop");
        ClientDisconnect(0);
    }

    //pada saat server disconnect
    public override void OnStopServer()
    {
        base.OnStopServer();
        Debug.Log("Client stop from server");
        ServerDisconnect(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        Physics2D.IgnoreLayerCollision(6,6);
    }

    void LateUpdate()
    {
        //Jika player tidak punya autoritas maka keluar
        if (!hasAuthority) { return; }

        if (isLocalPlayer)
        {
            GameObject mainkamera = GameObject.Find("Main Camera");
            if(mainkamera.transform.position != transform.position)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, mainkamera.transform.position.z);
                mainkamera.transform.position = Vector3.Lerp(mainkamera.transform.position,targetPosition,0.1f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //dijalankan di server
        if (isServer)
        {
            if(directionAttack == "attack")
            {
                animator.SetBool("attacking", true);
                StartCoroutine(StopAttack());
            }
            if (directionH == "left" || directionH == "right" || directionV == "up" || directionV == "up")
            {
                if (directionH == "left")
                {
                    animator.SetFloat("moveX", InputJX);
                    animator.SetBool("moving", true);
                }
                if (directionH == "right")
                {
                    animator.SetFloat("moveX", InputJX);
                    animator.SetBool("moving", true);
                }
                if (directionV == "up")
                {
                    animator.SetFloat("moveY", InputJY);
                    animator.SetBool("moving", true);
                }
                if (directionV == "down")
                {
                    animator.SetFloat("moveY", InputJY);
                    animator.SetBool("moving", true);
                }
                Vector2 change = new Vector3(InputJX,InputJY);
                Vector2 smoothVelocity = new Vector2(0,0);
                float smoothTime = 0f;
                Vector2 moveAmount = Vector2.SmoothDamp(GetComponent<Rigidbody2D>().position, change * speed, ref smoothVelocity, smoothTime);
                GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + new Vector2(transform.TransformDirection(moveAmount).x, transform.TransformDirection(moveAmount).y) *Time.deltaTime);
            }
            else
            {
                animator.SetBool("moving", false);
            }
        }
        //Jika player tidak punya autoritas maka keluar
        if (!hasAuthority) { return; }

        if (isLocalPlayer)
        {
            try
            {
                joystick = GameObject.Find("Canvas").transform.Find("Fixed Joystick").GetComponent<FixedJoystick>();

                if (joystick != null)
                {
                    if (joystick.Horizontal < 0)
                    {
                        CmdMoveLeftSide(joystick.Horizontal);
                    }
                    if (joystick.Horizontal > 0)
                    {
                        CmdMoveRightSide(joystick.Horizontal);
                    }
                    if (joystick.Horizontal == 0)
                    {
                        CmdMoveReleaseH();
                    }

                    if (joystick.Vertical < 0)
                    {
                        CmdMoveDownSide(joystick.Vertical);
                    }
                    if (joystick.Vertical > 0)
                    {
                        CmdMoveUpSide(joystick.Vertical);
                    }
                    if (joystick.Vertical == 0)
                    {
                        CmdMoveReleaseV();
                    }
                }
            }catch (Exception e)
            {

            }
        }
    }

    public void Attack()
    {
        CmdAttack();
    }

    public IEnumerator StopAttack()
    {
        int animHash = Animator.StringToHash("Base Layer.attackDown");
        while (animator.GetCurrentAnimatorStateInfo(0).fullPathHash != animHash)
        {
            yield return null;
        }

        float counter = 0;
        float waitTime = animator.GetCurrentAnimatorStateInfo(0).length;

        //Now, Wait until the current state is done playing
        while (counter < (waitTime))
        {
            counter += Time.deltaTime;
            yield return null;
        }

        animator.SetBool("attacking", false);
        directionAttack = "nothing";
    }

    [Command]
    private void CmdAttack()
    {
        directionAttack = "attack";
    }

    [Command]
    private void CmdMoveReleaseH()
    {
        InputJX = 0;
        directionH = "nothing";
    }

    [Command]
    private void CmdMoveReleaseV()
    {
        InputJY = 0;
        directionV = "nothing";
    }

    [Command]
    private void CmdMoveLeftSide(float input)
    {
        InputJX = input;
        directionH = "left";
    }

    [Command]
    private void CmdMoveRightSide(float input)
    {
        InputJX = input;
        directionH = "right";
    }

    [Command]
    private void CmdMoveUpSide(float input)
    {
        InputJY = input;
        directionV = "up";
    }

    [Command]
    private void CmdMoveDownSide(float input)
    {
        InputJY = input;
        directionV = "down";
    }
    void Awake()
    {
        DontDestroyOnLoad(this);
        networkMatchChecker = GetComponent<NetworkMatchChecker>();
    }

    /*
     Host Game
     */
    public void HostGame(bool publicMatch)
    {
        string matchId = MatchMaker.getRandomMatchId();
        CmdHostGame(matchId, publicMatch);
    }
    [Command]
    void CmdHostGame(string matchId, bool publicMatch)
    {
        MatchID = matchId;
        if (MatchMaker.instance.HostGame(matchId, gameObject, publicMatch, out playerIndex))
        {
            Debug.Log("Game Hosted Successfully");
            networkMatchChecker.matchId = matchId.ToGuid();
            TargetHostGame(true, matchId, playerIndex);
            TargetHostGameAll(playerIndex);
        }
        else
        {
            Debug.Log("Game Hosted Failed");
            TargetHostGame(false, matchId, playerIndex);
            TargetHostGameAll(playerIndex);
        }
    }
    [TargetRpc]
    void TargetHostGame(bool _success, string _matchId, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchId;
        LobbyController.instance.HostSuccess(_success, _matchId);
    }
    [ClientRpc]
    void TargetHostGameAll(int _playerIndex)
    {
        name = "Player " + playerIndex;
    }
    /*
     Join Game
     */
    public void JoinGame(string inputId)
    {
        CmdJoinGame(inputId);
    }
    [Command]
    void CmdJoinGame(string matchId)
    {
        MatchID = matchId;
        if (MatchMaker.instance.JoinGame(matchId, gameObject, out playerIndex))
        {
            Debug.Log("Game Joined Successfully");
            networkMatchChecker.matchId = matchId.ToGuid();
            TargetJoinGame(true, matchId, playerIndex);
            TargetJoinGameAll(playerIndex);
        }
        else
        {
            Debug.Log("Game Joined Failed");
            TargetJoinGame(false, matchId, playerIndex);
            TargetJoinGameAll(playerIndex);
        }
    }
    [TargetRpc]
    void TargetJoinGame(bool _success, string _matchId, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchId;
        LobbyController.instance.JoinSuccess(_success, _matchId);
    }
    [ClientRpc]
    void TargetJoinGameAll(int _playerIndex)
    {
        name = "Player " + playerIndex;
    }
    /*
     Search Match
     */
    public void SearchGame()
    {
        CmdSearchGame();
    }
    [Command]
    void CmdSearchGame()
    {
        if (MatchMaker.instance.SearchGame(gameObject, out playerIndex, out MatchID))
        {
            Debug.Log("Game Search Successfully");
            networkMatchChecker.matchId = MatchID.ToGuid();
            TargetSearchGame(true, MatchID, playerIndex);
            TargetJoinGameAll(playerIndex);
        }
        else
        {
            Debug.Log("Game Search Failed");
            TargetSearchGame(false, MatchID, playerIndex);
            TargetJoinGameAll(playerIndex);
        }
    }
    [TargetRpc]
    public void TargetSearchGame(bool _success, string _matchId, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchId;
        LobbyController.instance.SearchSuccess(_success, _matchId);
    }

    /*
     Begin Game
     */
    public void BeginGame()
    {
        CmdBeginGame();
    }
    [Command]
    void CmdBeginGame()
    {
        for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].matchId == MatchID)
            {
                for (int j = 0; j < MatchMaker.instance.matches[i].players.Count; j++)
                {
                    if (MatchMaker.instance.matches[i].players[j].GetComponent<Player>().playerType == -1)
                    {
                        Debug.Log("Please choice player");
                        return;
                    }
                }
            }
        }

        MatchMaker.instance.BeginGame(MatchID);
        Debug.Log("Game Begin" + MatchID);


    }

    public void StartGame()
    {
        TargetBeginGame();
    }
    [TargetRpc]
    void TargetBeginGame()
    {
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().onlineScene = "Gameplay";
        GameObject.Find("NetworkManager").GetComponent<NetworkManager>().ServerChangeScene("Gameplay");
        //Cursor.lockState = CursorLockMode.Locked;
        //SceneManager.LoadScene(2,LoadSceneMode.Additive);
    }

    //fungsi pemilihan karakter diserver
    public void Choice(int _typePlayer)
    {
        CmdChoice(_typePlayer);
    }

    [Command]
    public void CmdChoice(int _typePlayer)
    {
        //Jika player sudah dipilih maka fungsi selesai
        /*for (int i = 0; i < MatchMaker.instance.matches.Count; i++)
        {
            if (MatchMaker.instance.matches[i].matchId == MatchID)
            {
                for (int j = 0; j < MatchMaker.instance.matches[i].players.Count; j++)
                {
                    if (MatchMaker.instance.matches[i].players[j].GetComponent<Player>().playerType == _typePlayer)
                    {
                        Debug.Log("Player already Choice");
                        return;
                    }
                }
            }
        }*/

        //pemilihan job player
        playerType = _typePlayer;
        //typeObject = new Karakter(_typePlayer);
        //typeObject.getCharacter().changeColor(this.gameObject);

        //kembali ke client dengan job yang dipilih
        TargetChangePlayer(playerType);
    }

    //mengubah job player disemua client
    [ClientRpc]
    public void TargetChangePlayer(int _playerType)
    {
        //typeObject.setCharacter(_playerType);
        //typeObject.getCharacter().Mulai();
        //typeObject.getCharacter().changeColor(this.gameObject);
    }

    /*
     Disconnect Match
     */

    public void DisconnectGame(int lobbyScene)
    {
        CmdDisconnectGame(lobbyScene);
    }
    [Command]
    public void CmdDisconnectGame(int lobbyScene)
    {
        ServerDisconnect(lobbyScene);
    }
    public void ServerDisconnect(int lobbyScene)
    {
        MatchMaker.instance.PlayerDisconnected(this, MatchID);
        networkMatchChecker.matchId = string.Empty.ToGuid();
        RpcDisconnectGame(lobbyScene);
    }
    [ClientRpc]
    public void RpcDisconnectGame(int lobbyScene)
    {
        ClientDisconnect(lobbyScene);
    }
    public void ClientDisconnect(int lobbyScene)
    {
        if (SceneManager.GetActiveScene().name != "Gameplay")
        {
            if (playerLobbyUI != null)
            {
                for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
                {
                    if (GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerIndex > playerIndex)
                    {
                        GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerIndex -= 1;
                        GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().name = "Player " + GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerIndex;
                        GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().playerLobbyUI.GetComponent<UIPlayer>().setPlayer(GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>());
                    }
                }
                Destroy(playerLobbyUI);
            }
            if (lobbyScene == 0)
            {
                //Destroy(GameObject.Find("ItemSpawn").gameObject);
                //Destroy(GameObject.Find("PlayersSpawn").gameObject);
                //Destroy(GameObject.Find("ObjectSpawn").gameObject);
            }
        }
    }
}
