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
    
    public void InventoryClick()
    {
        GameObject.Find("Canvas").transform.Find("Inventory Panel").gameObject.SetActive(!GameObject.Find("Canvas").transform.Find("Inventory Panel").gameObject.active);
    }

    public void InteractClick()
    {
        Player.localPlayer.Interact();
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
