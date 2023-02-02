using System;
using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    idle,
    walk,
    interact,
    attack,
    stagger
}

public class Player : NetworkBehaviour
{
    [Header("Object setting")]
    public static Player localPlayer;
    public static int indexPlayer;
    public static Transform localTransformPlayer;

    [Header("GameObject setting")]
    public Camera MainCamera;
    public NetworkMatch networkMatchChecker;
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
    [SyncVar] public float InputJX2;
    [SyncVar] public float InputJY2;
    [SyncVar] public int playerType;
    [SyncVar] public int mapLoad;
    [SyncVar] public PlayerState currentState;
    [SyncVar] public PlayerInventory playerInventory;
    [SyncVar] public GameObject npcInteract;
    [SyncVar] public PlayerQuest playerQuest;
    [SyncVar] public PlayerState tempState;
    [SyncVar] public int otheritung;
    [SyncVar] public int pertamaNama;


    NetworkMatch networkMatch;

    [Header("Player Settings")]
    public float speed;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public bool mulaiAttack;

    Guid netIDGuid;

    void Awake()
    {
        DontDestroyOnLoad(this);
        networkMatchChecker = GetComponent<NetworkMatch>();
        networkMatch = GetComponent<NetworkMatch>();
    }


    public override void OnStartServer()
    {
        netIDGuid = netId.ToString().ToGuid();
        networkMatch.matchId = netIDGuid;
    }

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
            //Player.localPlayer.otherPlayer.Add(this);
            //inisialisasi GameObject UI Player di Lobby
        }
    }
    public void mulaiLobi()
    {
        Debug.Log("mulai hola");
        cmdMulaiLobi();
    }
    [Command]
    public void cmdMulaiLobi()
    {
        for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
        {
            GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().addMulaiLobi(playerIndex, MatchID);
        }
    }
    [TargetRpc]
    public void addMulaiLobi(int index,string matchID)
    {
        if (isLocalPlayer)
        {
            Debug.Log("hola");
            otheritung = 2;
        }
    }
    public void addUIPlayer()
    {
        if (playerLobbyUI == null)
        {
            if (MatchID != "")
            {
                //Player.localPlayer.CmdaddUIPlayer();

                playerLobbyUI = LobbyController.instance.spawnPlayerPrefab(this);
                Player.localPlayer.CmdUI();
                Player.localPlayer.otheritung = 1;
            }
        }
    }
    [Command]
    public void CmdUI()
    {
        for (int i = 0; i < MatchMaker.instance.matches.matches.Count; i++)
        {
            Debug.Log(MatchMaker.instance.matches.matches[i].matchId + " = " + MatchID);
            if (MatchMaker.instance.matches.matches[i].matchId == MatchID)
            {
                for (int j = 0; j < MatchMaker.instance.matches.matches[i].players.Count; j++)
                {
                    if (MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().MatchID == MatchID)
                    {
                        int playerIndextmp = MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().playerIndex;
                        string matchIdtmp = MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().MatchID;

                        MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().playerIndex = 0;
                        MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().MatchID = "";
                        MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().playerIndex = playerIndextmp;
                        MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().MatchID = matchIdtmp;

                    }
                }
            }
        }
    }
    [TargetRpc]
    public void ClientaddUIPlayer(int _index,string _matchid)
    {
        Debug.Log("Player "+_index+" - "+_matchid);
        playerIndex = _index;
        MatchID = _matchid;
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
        Physics2D.IgnoreLayerCollision(6, 6);
        animator.SetFloat("moveX", 0);
        animator.SetFloat("moveY", -1);
        currentState = PlayerState.walk;
        tempState = PlayerState.walk;
        mulaiAttack = true;
        npcInteract = null;
        otheritung = 1;
        pertamaNama = 1;
        if (isServer)
        {
            playerInventory = ScriptableObject.CreateInstance<PlayerInventory>();
            playerQuest = ScriptableObject.CreateInstance<PlayerQuest>();
        }
    }
    public void initialize()
    {
        directionAttack = "nothing";
        currentState = tempState;
        mulaiAttack = true;
    }

    private void Update()
    {
        if (isClient)
        {
            if (!isLocalPlayer)
            {
                if (Player.localPlayer != null)
                {
                    if (Player.localPlayer.otheritung == 2)
                    {
                        addUIPlayer();
                    }
                }

                
            }
            if (MatchID != "" && pertamaNama == 1)
            {
                name = "Player " + playerIndex +" - "+MatchID;
                pertamaNama = 2;
            }
        }
    }

    void LateUpdate()
    {


        //Jika player tidak punya autoritas maka keluar
        if (!isOwned) { return; }

        if (isLocalPlayer)
        {
            GameObject mainkamera = GameObject.Find("Main Camera");
            if(mainkamera.transform.position != transform.position)
            {
                Vector3 targetPosition = new Vector3(transform.position.x, transform.position.y, mainkamera.transform.position.z);
                targetPosition.x = Mathf.Clamp(targetPosition.x,minPosition.x,maxPosition.x);
                targetPosition.y = Mathf.Clamp(targetPosition.y, minPosition.y, maxPosition.y);
                mainkamera.transform.position = Vector3.Lerp(mainkamera.transform.position,targetPosition,0.1f);
            }
        }
    }

    public void takeQuest(int levelQuest) {
        CmdTakeQuest(npcInteract.GetComponent<NPCController>().npcId,levelQuest);
    }

    [Command]
    public void CmdTakeQuest(int npcId,int levelQuest)
    {
        if (npcId == npcInteract.GetComponent<NPCController>().npcId)
        {

            //Bikin Story baru
            Story storyAku = new Story(npcInteract.GetComponent<NPCController>().dialogNpc.value.text);
            storyAku.variablesState["introQuest"] = npcInteract.GetComponent<NPCController>().introQuest;

            storyAku.BindExternalFunction("takeQuestNow", (string nama,string reward) => {
                //_audioController.Play(name);
                string[] subs = reward.Split(',');
                Quest newQuest = ScriptableObject.CreateInstance<Quest>();
                newQuest.idQuest = npcId;
                newQuest.ownerQuest = npcInteract;
                newQuest.levelQuest = levelQuest;
                newQuest.takeQuest = 1;
                newQuest.rewardItem = new List<InventoryItem>();
                foreach (var sub in subs)
                {
                    InventoryItem itemBaru = ScriptableObject.CreateInstance<InventoryItem>();
                    var subs2 = sub.Split('_');
                    InventoryItem thisItem = Resources.Load<InventoryItem>("ScriptableObjects/Item/Surah " + subs2[0] + "/Ayat " + subs2[1] + "/" + subs2[2] + "_" + subs2[3]);
                    itemBaru.itemId = thisItem.itemId;
                    itemBaru.itemName = thisItem.itemName;
                    itemBaru.itemDescription = thisItem.itemDescription;
                    itemBaru.itemImage = thisItem.itemImage;
                    itemBaru.itemImageIndex = thisItem.itemImageIndex;
                    itemBaru.numberHeld = thisItem.numberHeld;
                    itemBaru.usable = thisItem.usable;
                    itemBaru.usable = thisItem.unique;
                    itemBaru.thisEvent = thisItem.thisEvent;
                    newQuest.rewardItem.Add(itemBaru);
                    
                }

                playerQuest.myQuest.Add(newQuest);
                PlayerQuest temp = playerQuest;
                playerQuest = null;
                playerQuest = temp;
                Debug.Log("Quest berhasil diambil level " + name);
            });
            storyAku.BindExternalFunction("giveRewardQuest", (string name, string reward) => {

            });

            int jumlahOwnerQuest = 0;
            if (playerQuest.myQuest.Count > 0)
            {
                for (int i = 0; i < playerQuest.myQuest.Count; i++)
                {
                    if (playerQuest.myQuest[i].ownerQuest == npcInteract)
                    {
                        jumlahOwnerQuest++;
                    }
                }
            }
            if (jumlahOwnerQuest > 0)
            {
                storyAku.variablesState["takeQuest"] = 1;
            }
            else
            {
                storyAku.variablesState["takeQuest"] = 0;
            }

            while (storyAku.canContinue)
            {
                storyAku.Continue();
            }
            storyAku.ChooseChoiceIndex(0);

            while (storyAku.canContinue)
            {
                storyAku.Continue();
            }
            storyAku.ChooseChoiceIndex(0);

            while (storyAku.canContinue)
            {
                storyAku.Continue();
            }

        }
    }


    public void Interact()
    {
        CmdInteract();
    }

    [Command]
    public void CmdInteract()
    {
        if (npcInteract != null)
        {
            currentState = PlayerState.interact;
            chatWithNPC(npcInteract.GetComponent<NPCController>().dialogNpc.value.text);
        }
    }

    [TargetRpc]
    public void chatWithNPC(string dialogNpc)
    {
        TextAssetValue newText = ScriptableObject.CreateInstance<TextAssetValue>();
        TextAsset newTextAsset = new TextAsset(dialogNpc);
        newText.value = newTextAsset;
        GameObject.Find("Canvas").transform.Find("Dialog Panel").GetComponent<BranchingDialogController>().npcInteract = npcInteract;
        GameObject.Find("Canvas").transform.Find("Dialog Panel").GetComponent<BranchingDialogController>().playerInteract = gameObject;
        GameObject.Find("Canvas").transform.Find("Dialog Panel").GetComponent<BranchingDialogController>().dialogValue = newText;
        GameObject.Find("Canvas").transform.Find("Dialog Panel").gameObject.SetActive(true);
    }
    
    public void doneChat()
    {
        CmdDoneChat();
    }

    [Command]
    public void CmdDoneChat()
    {
        currentState = PlayerState.idle;
    }
    


    public void Knock(Rigidbody2D myRigidbody, float knockTime)
    {
        StartCoroutine(KnockCo(myRigidbody, knockTime));
    }


    IEnumerator KnockCo(Rigidbody2D myRigidbody, float knockTime)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            currentState = PlayerState.idle;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //dijalankan di server
        if (isServer)
        {
            
            if (directionAttack != "attack" && (currentState == PlayerState.walk || currentState == PlayerState.idle))
            {
                if (directionH == "left" || directionH == "right" || directionV == "up" || directionV == "down")
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
                    float x = 1 / Vector2.Distance(new Vector2(0,0),new Vector2(InputJX,InputJY));
                    float InputHX = InputJX * x;
                    float InputHY = InputJY * x;

                    Vector2 change = new Vector3(InputHX, InputHY);
                    Vector2 smoothVelocity = new Vector2(0, 0);
                    float smoothTime = 0f;
                    Vector2 moveAmount = Vector2.SmoothDamp(GetComponent<Rigidbody2D>().position, change * speed, ref smoothVelocity, smoothTime);
                    GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + change*speed * Time.fixedDeltaTime);

                }
                else
                {
                    animator.SetBool("moving", false);
                }
            }
            if (directionAttack == "attack" && currentState != PlayerState.attack && currentState != PlayerState.stagger)
            {
                if (mulaiAttack)
                {
                    Debug.Log(InputJX);
                    if(InputJX2 > InputJY2 && InputJX2 > 0 && InputJY2 > 0)
                    {
                        animator.SetFloat("moveX", 1);
                        animator.SetFloat("moveY", 0);
                    }
                    else if(InputJX2 < InputJY2 && InputJX2 > 0 && InputJY2 > 0)
                    {
                        animator.SetFloat("moveX", 0);
                        animator.SetFloat("moveY", 1);
                    }
                    else if (Mathf.Abs(InputJX2) < Mathf.Abs(InputJY2) && InputJX2 < 0 && InputJY2 > 0)
                    {
                        animator.SetFloat("moveX", 0);
                        animator.SetFloat("moveY", 1);
                    }
                    else if (Mathf.Abs(InputJX2) > Mathf.Abs(InputJY2) && InputJX2 < 0 && InputJY2 > 0)
                    {
                        animator.SetFloat("moveX", -1);
                        animator.SetFloat("moveY", 0);
                    }
                    else if (Mathf.Abs(InputJX2) > Mathf.Abs(InputJY2) && InputJX2 > 0 && InputJY2 < 0)
                    {
                        animator.SetFloat("moveX", 1);
                        animator.SetFloat("moveY", 0);
                    }
                    else if (Mathf.Abs(InputJX2) < Mathf.Abs(InputJY2) && InputJX2 > 0 && InputJY2 < 0)
                    {
                        animator.SetFloat("moveX", 0);
                        animator.SetFloat("moveY", -1);
                    }
                    else if (InputJX2 < InputJY2 && InputJX2 < 0 && InputJY2 < 0)
                    {
                        animator.SetFloat("moveX", -1);
                        animator.SetFloat("moveY", 0);
                    }
                    else if (InputJX2 > InputJY2 && InputJX2 < 0 && InputJY2 < 0)
                    {
                        animator.SetFloat("moveX", 0);
                        animator.SetFloat("moveY", -1);
                    }

                    mulaiAttack = false;
                    tempState = currentState;
                    currentState = PlayerState.attack;
                    animator.SetBool("attacking", true);
                    StartCoroutine(StopAttack());
                }
            }
        }

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

    public void mapLoaded()
    {
        CmdMapLoaded();
    }

    [Command]
    public void CmdMapLoaded()
    {
        mapLoad = 1;

        
        int loadCount = 0;
        int maxCount = 0;
        //if(true)
        for (int i = 0; i < MatchMaker.instance.matches.matches.Count; i++)
        {
            //if(true)
            if (MatchMaker.instance.matches.matches[i].matchId == MatchID)
            {
                maxCount = MatchMaker.instance.matches.matches[i].players.Count;
                //if(true)
                foreach (var player in MatchMaker.instance.matches.matches[i].players)
                {
                    //if(true)
                    if(player.GetComponent<Player>().mapLoad == 1)
                    {
                        loadCount++;
                    }
                }
            }
        }
        if(loadCount == maxCount) {
            Debug.Log("Load new Game");
            GameplayMaker.instance.LoadUnit(MatchID, connectionToClient);
            Destroy(GameObject.Find("TurnManager(Clone)").gameObject);
        }

    }

    public void Attack()
    {
        CmdAttack();
    }

    public void endAttack()
    {

        animator.SetBool("attacking", false);
        currentState = tempState;
    }

    public IEnumerator StopAttack()
    {
        int animHash = Animator.StringToHash("Base Layer.Attack");
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

    }

    [Command]
    private void CmdAttack()
    {
        if (directionAttack == "nothing")
        {
            directionAttack = "attack";
        }
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
        InputJX2 = input;
        directionH = "left";
    }

    [Command]
    private void CmdMoveRightSide(float input)
    {
        InputJX = input;
        InputJX2 = input;
        directionH = "right";
    }

    [Command]
    private void CmdMoveUpSide(float input)
    {
        InputJY = input;
        InputJY2 = input;
        directionV = "up";
    }

    [Command]
    private void CmdMoveDownSide(float input)
    {
        InputJY = input;
        InputJY2 = input;
        directionV = "down";
    }

    /*
     Host Game
     */
    public void HostGame(bool publicMatch)
    {
        string matchID = MatchMaker.getRandomMatchId();
        CmdHostGame(matchID, publicMatch);
    }

    [Command]
    void CmdHostGame(string _matchID, bool publicMatch)
    {
        MatchID = _matchID;
        if (MatchMaker.instance.HostGame(_matchID, gameObject, publicMatch, out playerIndex))
        {
            Debug.Log($"<color=green>Game hosted successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetHostGame(true, _matchID, playerIndex);
            TargetHostGameAll(playerIndex,_matchID);
        }
        else
        {
            Debug.Log($"<color=red>Game hosted failed</color>");
            TargetHostGame(false, _matchID, playerIndex);
            TargetHostGameAll(playerIndex, _matchID);
        }
    }

    [TargetRpc]
    void TargetHostGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchID;
        Debug.Log($"MatchID: {MatchID} == {_matchID}");
        LobbyController.instance.HostSuccess(success, _matchID);
    }
    [ClientRpc]
    void TargetHostGameAll(int _playerIndex,string _matchid)
    {
        playerIndex = _playerIndex;
        name = "Player " + playerIndex+" - "+_matchid;
        if(playerLobbyUI!=null)
        playerLobbyUI.GetComponent<UIPlayer>().setPlayer(this);
    }

    /* 
        JOIN MATCH
    */

    public void JoinGame(string _inputID)
    {
        CmdJoinGame(_inputID);
    }

    [Command]
    void CmdJoinGame(string _matchID)
    {
        MatchID = _matchID;
        if (MatchMaker.instance.JoinGame(_matchID, gameObject, out playerIndex))
        {
            Debug.Log($"<color=green>Game Joined successfully</color>");
            networkMatch.matchId = _matchID.ToGuid();
            TargetJoinGame(true, _matchID, playerIndex);
            for (int i = 0; i < GameObject.FindGameObjectsWithTag("Player").Length; i++)
            {
                GameObject.FindGameObjectsWithTag("Player")[i].GetComponent<Player>().TargetJoinGameAll(playerIndex, _matchID);
            }
            //Host
            if (isServer && playerLobbyUI != null)
            {
                playerLobbyUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"<color=red>Game Joined failed</color>");
            TargetJoinGame(false, _matchID, playerIndex);
            TargetJoinGameAll(playerIndex,_matchID);
        }
    }

    [TargetRpc]
    void TargetJoinGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchID;
        Debug.Log($"MatchID: {MatchID} == {_matchID}");
        LobbyController.instance.JoinSuccess(success, _matchID);
    }

    /*
     Join Game
     */
    [ClientRpc]
    void TargetJoinGameAll(int _playerIndex, string _matchID)
    {
        playerIndex = _playerIndex;
        Debug.Log("Join Player " + playerIndex);
        name = "Player " + playerIndex+" - "+ _matchID;
        playerLobbyUI.GetComponent<UIPlayer>().setPlayer(this);
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
            Debug.Log($"<color=green>Game Found Successfully</color>");
            networkMatch.matchId = MatchID.ToGuid();
            TargetSearchGame(true, MatchID, playerIndex);

            //Host
            if (isServer && playerLobbyUI != null)
            {
                playerLobbyUI.SetActive(true);
            }
        }
        else
        {
            Debug.Log($"<color=red>Game Search Failed</color>");
            TargetSearchGame(false, MatchID, playerIndex);
        }
    }

    [TargetRpc]
    void TargetSearchGame(bool success, string _matchID, int _playerIndex)
    {
        playerIndex = _playerIndex;
        MatchID = _matchID;
        Debug.Log($"MatchID: {MatchID} == {_matchID} | {success}");
        LobbyController.instance.SearchSuccess(success, _matchID);
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
        for (int i = 0; i < MatchMaker.instance.matches.matches.Count; i++)
        {
            if (MatchMaker.instance.matches.matches[i].matchId == MatchID)
            {
                for (int j = 0; j < MatchMaker.instance.matches.matches[i].players.Count; j++)
                {
                    if (MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().playerType == -1)
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
        for (int i = 0; i < MatchMaker.instance.matches.matches.Count; i++)
        {
            if (MatchMaker.instance.matches.matches[i].matchId == MatchID)
            {
                for (int j = 0; j < MatchMaker.instance.matches.matches[i].players.Count; j++)
                {
                    if (MatchMaker.instance.matches.matches[i].players[j].GetComponent<Player>().playerType == _typePlayer)
                    {
                        Debug.Log("Player already Choice");
                        return;
                    }
                }
            }
        }

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
                if (!isServer)
                {
                    Destroy(playerLobbyUI);
                }
                else
                {
                    playerLobbyUI.SetActive(false);
                }

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
