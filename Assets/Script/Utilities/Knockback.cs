using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float thrust;
    public float knockTime;
    public float damage;
    public bool staystatus;

    // Start is called before the first frame update
    void Start()
    {
        staystatus = false;
    }

    // Update is called once per frame
    void Update()
    {

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Holas");
        try
        {
            if (transform.parent.name.Contains("Player"))
            {
                if (transform.parent.GetComponent<Player>().isServer)
                {
                    Debug.Log("Masuk HOLD");
                    KnockNow(collision);
                }
            }
        }
        catch (Exception e)
        {
               KnockNow(collision);
        }
    }
    public void KnockNow(Collider2D collision)
    {
        Debug.Log("Masuk HOLD2");
        if (collision.CompareTag("enemy"))
        {
            Debug.Log("Masuk HOLD3");
            Rigidbody2D hit = collision.GetComponent<Rigidbody2D>();
            if (hit != null) { 

                hit.isKinematic = false;
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                Debug.Log("Masuk HOLD4" + difference);
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (collision.CompareTag("enemy") )
                {
                    hit.GetComponent<Enemy>().currentState = PlayerState.stagger;
                    collision.GetComponent<Enemy>().Knock(hit, knockTime,damage);
                }
                /*if (collision.CompareTag("Player"))
                {
                    hit.GetComponent<Player>().currentState = PlayerState.stagger;
                    collision.GetComponent<Player>().Knock(hit, knockTime);
                }*/
            }
        }
    }
}
