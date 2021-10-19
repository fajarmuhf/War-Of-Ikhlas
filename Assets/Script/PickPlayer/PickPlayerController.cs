using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PickPlayerController : MonoBehaviour
{
    List<string> ikonPemain;
    int pilihPemain;
    // Start is called before the first frame update
    void Start()
    {
        ikonPemain = new List<string>();
        ikonPemain.Add("Pemain_tremel");
        ikonPemain.Add("Pemain_egyptianqueen");
        pilihPemain = 0;
    }

    public void GeserKiri()
    {
        pilihPemain--;
        if (pilihPemain < 0)
        {
            pilihPemain = ikonPemain.Count-1;
        }
        GantiPemain(pilihPemain);
    }

    public void GantiPemain(int pemain)
    {
        GameObject.Find("Pemain").GetComponent<Animator>().runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Image/"+ikonPemain[pemain]);
    }

    public void GeserKanan()
    {
        pilihPemain++;
        if(pilihPemain == ikonPemain.Count)
        {
            pilihPemain = 0;
        }
        GantiPemain(pilihPemain);
    }

    public void PickPemain()
    {
        NetworkManager networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        networkManager.networkAddress = "localhost";
        networkManager.StartClient();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
