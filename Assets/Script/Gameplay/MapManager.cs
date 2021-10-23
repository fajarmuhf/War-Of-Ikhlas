using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("Homestead", LoadSceneMode.Additive);

        initGameplay();
    }

    public void initGameplay()
    {
        Player.localPlayer.mapLoaded();
    }

    public void AttackClick()
    {
        Player.localPlayer.Attack();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
