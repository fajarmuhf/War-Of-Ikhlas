using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float thrust;
    public float knockTime;
    public float damage;

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
        try
        {
            if (transform.parent.name.Contains("Player"))
            {
                if (transform.parent.GetComponent<Player>().isServer)
                {
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
        if (collision.CompareTag("enemy") || collision.CompareTag("Player"))
        {
            Rigidbody2D hit = collision.GetComponent<Rigidbody2D>();
            if (hit != null)
            {
                Vector2 difference = hit.transform.position - transform.position;
                difference = difference.normalized * thrust;
                hit.AddForce(difference, ForceMode2D.Impulse);
                if (collision.CompareTag("enemy") && collision.isTrigger )
                {
                    hit.GetComponent<Enemy>().currentState = PlayerState.stagger;
                    collision.GetComponent<Enemy>().Knock(hit, knockTime,damage);
                }
                if (collision.CompareTag("Player"))
                {
                    hit.GetComponent<Player>().currentState = PlayerState.stagger;
                    collision.GetComponent<Player>().Knock(hit, knockTime);
                }
            }
        }
    }
}
