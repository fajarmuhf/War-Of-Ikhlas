using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine.SceneManagement;

/*
 * Match
 * - berisi id,players,items
 * - berisi publicMatch (public atau private match)
 * - berisi inMatch dalam permainan atau tidak
 * - berisi matchFull / match penuh atau tidak
 */

[System.Serializable]
public class Match
{
    public string matchId;
    public SyncListGameObject players = new SyncListGameObject();
    public SyncListGameObject items = new SyncListGameObject();
    public bool publicMatch;
    public bool inMatch;
    public bool matchFull;

    public Match(string matchId, GameObject player)
    {
        this.matchId = matchId;
        players.Add(player);
    }
    public Match() { }
}

[System.Serializable]
public class SyncListGameObject : SyncList<GameObject> { }

[System.Serializable]
public class SyncListMatch : SyncList<Match> { }

[System.Serializable]
public class SyncListString : SyncList<String> { }

/*
 * MatchMaker
 * - berisi mendapatkan random match id
 * - berisi server fungsi untuk Host,Join,Search dan Begin
 * - berisi server fungsi untuk disconnect player
 */

[System.Serializable]
public class Matchku
{
    public SyncListMatch matches = new SyncListMatch();
    public SyncListString matchIDs = new SyncListString();

    
}

public class MatchMaker : NetworkBehaviour
{

    public static MatchMaker instance;
    public Matchku matchku = new Matchku();
    
    [SerializeField] GameObject turnManagerPrefab;

    void Start()
    {
        Debug.Log("match ok");
        instance = this;

        if (isServer)
        {
            SceneManager.LoadScene("Homestead", LoadSceneMode.Additive);
        }
    }

    //fungsi untuk mendapatkan random untuk match id
    public static string getRandomMatchId()
    {
        string _id = string.Empty;
        for (int i = 0; i < 5; i++)
        {
            int random = UnityEngine.Random.Range(0, 36);
            if (random < 26)
            {
                _id += (char)(random + 65);
            }
            else
            {
                _id += (random - 26).ToString();
            }
        }

        return _id;
    }

    //fungsi host game
    public bool HostGame(string _matchId, GameObject _player, bool publicMatch, out int playerIndex)
    {
        playerIndex = -1;
        //jika matchid belum terdaftar
        if (!matchku.matchIDs.Contains(_matchId))
        {
            //membuat match id baru
            matchku.matchIDs.Add(_matchId);

            //membuat match baru dan ditambahkan ke list mathes
            Match matchBaru = new Match(_matchId, _player);
            matchBaru.publicMatch = publicMatch;
            matchku.matches.Add(matchBaru);
            Debug.Log("Match generated");
            playerIndex = 1;
            return true;
        }
        else
        {
            //jika matchid sudah terdaftar
            Debug.Log("Match already exist");
            return false;
        }
    }

    //fungsi search game
    public bool SearchGame(GameObject _player, out int playerIndex, out string matchId)
    {
        //inisialisasi playerIndex dan matchId
        playerIndex = -1;
        matchId = String.Empty;

        //membuat fungsi join game jika Match adalah public ,tidak penuh dan tidak sedang bermain 
        for (int i = 0; i < matchku.matches.Count; i++)
        {
            if (matchku.matches[i].publicMatch && !matchku.matches[i].matchFull && !matchku.matches[i].inMatch)
            {
                matchId = matchku.matches[i].matchId;
                if (JoinGame(matchId, _player, out playerIndex))
                {
                    return true;
                }
            }
        }

        return false;
    }

    //fungsi join game
    public bool JoinGame(string _matchId, GameObject _player, out int playerIndex)
    {
        //inisialisasi playerIndex
        playerIndex = -1;

        //jika matchid sudah terdaftar maka tambahkan pemain dan playerIndex
        if (matchku.matchIDs.Contains(_matchId))
        {
            for (int i = 0; i < matchku.matches.Count; i++)
            {
                if (matchku.matches[i].matchId == _matchId)
                {
                    matchku.matches[i].players.Add(_player);
                    playerIndex = matchku.matches[i].players.Count;
                    break;
                }
            }
            Debug.Log("Joined Match" + playerIndex);
            return true;
        }
        else
        {
            //jika matchid belum terdaftar
            Debug.Log("Match not exist");
            return false;
        }
    }

    //fungsi begin game
    public void BeginGame(string _matchId)
    {
        //membuat object turn manager
        GameObject newTurnManager = Instantiate(turnManagerPrefab);
        NetworkServer.Spawn(newTurnManager);
        newTurnManager.GetComponent<NetworkMatch>().matchId = _matchId.ToGuid();
        TurnManager turnManager = newTurnManager.GetComponent<TurnManager>();

        //jika matchid ditemukan maka permainan dimulai sesuai playernya
        for (int i = 0; i < matchku.matches.Count; i++)
        {
            if (matchku.matches[i].matchId == _matchId)
            {
                foreach (var player in matchku.matches[i].players)
                {
                    Player _player = player.GetComponent<Player>();
                    turnManager.AddPlayer(_player);
                    _player.StartGame();
                    matchku.matches[i].inMatch = true;
                }
                break;
            }
        }
    }

    //fungsi ketika player keluar
    public void PlayerDisconnected(Player _player, string _matchId)
    {
        //jika matchid ditemukan
        for (int i = 0; i < matchku.matches.Count; i++)
        {
            if (matchku.matches[i].matchId == _matchId)
            {
                //menghapus player yang disconnect dari matchku.matches
                var playerIndex = matchku.matches[i].players.IndexOf(_player.gameObject);
                matchku.matches[i].players.RemoveAt(playerIndex);

                //mengeser index player dari player yang disconnect
                for (int j = 0; j < matchku.matches[i].players.Count; j++)
                {
                    if (matchku.matches[i].players[j].GetComponent<Player>().playerIndex > _player.playerIndex)
                    {
                        matchku.matches[i].players[j].GetComponent<Player>().playerIndex -= 1;
                    }
                }

                Debug.Log("Player disconnect from lobby");

                //jika player kosong disuatu match
                if (matchku.matches[i].players.Count == 0)
                {
                    //hapus item-item dimatch dan hapus match
                    for (int j = 0; j < MatchMaker.instance.matchku.matches[i].items.Count; j++)
                        Destroy(MatchMaker.instance.matchku.matches[i].items[j]);
                    matchku.matches.RemoveAt(i);
                    matchku.matchIDs.Remove(_matchId);
                }

                break;
            }
        }
    }
}

public static class MatchExtensions
{
    //fungsi untuk membuat guid dari string
    public static Guid ToGuid(this string id)
    {
        MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
        byte[] inputBytes = Encoding.Default.GetBytes(id);
        byte[] hashBytes = provider.ComputeHash(inputBytes);

        return new Guid(hashBytes);
    }
}