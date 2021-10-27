using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
                collision.gameObject.GetComponent<Enemy>().reduceHealth();
            }
        }*/
    }
}
