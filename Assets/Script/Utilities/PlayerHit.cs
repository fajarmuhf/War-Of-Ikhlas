using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    public bool boleh;
    // Start is called before the first frame update
    void Start()
    {
        boleh = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        /*if (transform.parent.GetComponent<Player>().isServer)
        {
            if (collision.gameObject.CompareTag("enemy") && collision.GetComponent<Enemy>().MatchID == transform.parent.gameObject.GetComponent<Player>().MatchID)
            {
                if (!boleh)
                {
                    collision.gameObject.GetComponent<Enemy>().reduceHealth(1);
                    boleh = true;
                }
            }
        }*/
    }
}
