using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public enum EnemyState
{
    idle,
    walk,
    attack,
    stagger
}

public class Enemy : NetworkBehaviour
{
    [SyncVar] public string MatchID;
    [SyncVar] public string nameEnemy;
    [SyncVar] public float health;
    [SyncVar] public float maxHealth;
    [SyncVar] public float baseAttack;
    [SyncVar] public float moveSpeed;
    [SyncVar] public float attackSpeed;
    [SyncVar] public float chaseRadius;
    [SyncVar] public float attackRadius;
    [SyncVar] public Transform homePosition;
    [SyncVar] public PlayerState currentState;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] enemyTag = GameObject.FindGameObjectsWithTag("enemy");

        foreach (GameObject musuh in enemyTag)
        {
            Physics2D.IgnoreCollision(musuh.GetComponent<Collider2D>(),GetComponent<Collider2D>());
        }
        currentState = PlayerState.idle;
    }

    public void setMatchId(string matchId)
    {
        MatchID = matchId;

        ignoreDiffMatch();
    }

    public void ignoreDiffMatch()
    {
        GameObject[] pemainTag = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject pemain in pemainTag)
        {
            if (pemain.GetComponent<Player>().MatchID != MatchID)
            {
                Debug.Log("Ignore "+ pemain.GetComponent<Player>().MatchID);
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(),pemain.GetComponent<Collider2D>(),true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isServer)
        {
            checkDistance();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
    }

    public void checkDistance() {
        GameObject[] pemainTag = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject pemain in pemainTag)
        {
            if (pemain.GetComponent<Player>().MatchID == MatchID)
            {
                Vector2 target = new Vector2(pemain.transform.position.x+pemain.GetComponent<BoxCollider2D>().offset.x, pemain.transform.position.y + pemain.GetComponent<BoxCollider2D>().offset.y);
                Vector2 current = new Vector2(transform.position.x+GetComponent<BoxCollider2D>().offset.x,transform.position.y + GetComponent<BoxCollider2D>().offset.y);
                if (Vector2.Distance(target,current) <= chaseRadius && Vector2.Distance(target, current) > attackRadius)
                {
                    if (currentState == PlayerState.idle || currentState == PlayerState.walk && currentState != PlayerState.stagger)
                    {
                        Vector2 change = (target - current).normalized;
                        Vector2 smoothVelocity = new Vector2(0, 0);
                        float smoothTime = 0f;
                        Vector2 moveAmount = Vector2.SmoothDamp(GetComponent<Rigidbody2D>().position, change * moveSpeed, ref smoothVelocity, smoothTime);
                        GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + new Vector2(transform.TransformDirection(moveAmount).x, transform.TransformDirection(moveAmount).y) * Time.deltaTime);
                        ChangeState(PlayerState.walk);
                    }
                }
            }
        }
    }

    private void ChangeState(PlayerState newState)
    {
        if(currentState != newState)
        {
            currentState = newState;
        }
    }


    public void reduceHealth(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(this.gameObject);
            NetworkServer.Destroy(this.gameObject);
        }
    }

    public void Knock(Rigidbody2D myRigidbody, float knockTime,float damage)
    {
        StartCoroutine(KnockCo(myRigidbody,knockTime,damage));
    }


    IEnumerator KnockCo(Rigidbody2D myRigidbody,float knockTime,float damage)
    {
        if (myRigidbody != null)
        {
            yield return new WaitForSeconds(knockTime);
            myRigidbody.velocity = Vector2.zero;
            myRigidbody.GetComponent<Enemy>().currentState = PlayerState.idle;
            reduceHealth(damage);
        }
    }
}
