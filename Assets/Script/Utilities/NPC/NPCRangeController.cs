using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRangeController : MonoBehaviour
{
    private bool npcAlreadyInteract;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.parent.GetComponent<NPCController>().isServer)
        {
            if (other.CompareTag("Player"))
            {
                other.GetComponent<Player>().npcInteract = transform.parent.gameObject;
                npcAlreadyInteract = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (transform.parent.GetComponent<NPCController>().isServer)
        {
            if (npcAlreadyInteract)
            {
                if (other.CompareTag("Player"))
                {
                    other.GetComponent<Player>().npcInteract = null;
                    npcAlreadyInteract = false;
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        npcAlreadyInteract = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
