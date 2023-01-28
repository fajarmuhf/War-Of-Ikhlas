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
    [SyncVar] public GameObject pathObject;
    [SyncVar] public Transform currentGoal;
    [SyncVar] public int currentPoint;
    [SyncVar] public float roundingDistance;


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
    void FixedUpdate()
    {
        if (isServer)
        {
            checkDistance();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        /*if (collision.gameObject.CompareTag("enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider,GetComponent<Collider2D>());
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }*/
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        /*if (collision.gameObject.CompareTag("enemy"))
        {
            Physics2D.IgnoreCollision(collision.collider,GetComponent<Collider2D>());
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }*/
    }

    public void checkDistance() {
        GameObject[] pemainTag = GameObject.FindGameObjectsWithTag("Player");
        int pemainDalamJangkauan = 0;
        List<Vector2> targets = new List<Vector2>();
        int pemainJangkauanSerang = 0;
        int pemainLuarJangkauan = 0;
        int jumPemain = 0;

        foreach (GameObject pemain in pemainTag)
        {
            if (pemain.GetComponent<Player>().MatchID == MatchID)
            {
                jumPemain++;
                Vector2 target = new Vector2(pemain.transform.position.x+pemain.GetComponent<BoxCollider2D>().offset.x, pemain.transform.position.y + pemain.GetComponent<BoxCollider2D>().offset.y);
                Vector2 current = new Vector2(transform.position.x+GetComponent<BoxCollider2D>().offset.x,transform.position.y + GetComponent<BoxCollider2D>().offset.y);
                if (Vector2.Distance(target,current) <= chaseRadius && Vector2.Distance(target, current) > attackRadius)
                {
                    if (currentState == PlayerState.idle || currentState == PlayerState.walk && currentState != PlayerState.stagger)
                    {
                        targets.Add(target);
                        pemainDalamJangkauan++;
                    }
                }
                else if(Vector2.Distance(target, current) > chaseRadius)
                {
                    pemainLuarJangkauan++;
                }
            }
        }
        if (pemainDalamJangkauan > 0)
        {
            Vector2 minTarget = Vector2.zero;
            Vector2 current = new Vector2(transform.position.x+GetComponent<BoxCollider2D>().offset.x,transform.position.y + GetComponent<BoxCollider2D>().offset.y);
            for (int i=0;i<targets.Count;i++)
            {
                if (i == 0)
                {
                    minTarget = targets[0];
                }
                else
                {
                    if (Vector2.Distance(current, targets[i]) < Vector2.Distance(current,minTarget))
                    {
                        minTarget = targets[i];
                    }
                }
            }
            
            Vector2 target = minTarget;
            
            Vector2 change = (target - current).normalized;
            Vector2 smoothVelocity = new Vector2(0, 0);
            float smoothTime = 0f;
            Vector2 moveAmount = Vector2.SmoothDamp(GetComponent<Rigidbody2D>().position,
                change * moveSpeed, ref smoothVelocity, smoothTime);
            GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position +
                                                     new Vector2(
                                                         transform.TransformDirection(moveAmount).x,
                                                         transform.TransformDirection(moveAmount).y) *
                                                     Time.fixedDeltaTime);
            ChangeState(PlayerState.walk);
        }

        if (pemainLuarJangkauan == jumPemain)
        {
            Vector2 target = pathObject.transform.GetChild(currentPoint).position;
            Vector2 current = new Vector2(transform.position.x + GetComponent<BoxCollider2D>().offset.x, transform.position.y + GetComponent<BoxCollider2D>().offset.y);

            if (Vector2.Distance(target, current) > roundingDistance)
            {
                Vector2 change = (target - current).normalized;
                Vector2 smoothVelocity = new Vector2(0, 0);
                float smoothTime = 0f;
                Vector2 moveAmount = Vector2.SmoothDamp(GetComponent<Rigidbody2D>().position, change * moveSpeed, ref smoothVelocity, smoothTime);
                GetComponent<Rigidbody2D>().MovePosition(GetComponent<Rigidbody2D>().position + new Vector2(transform.TransformDirection(moveAmount).x, transform.TransformDirection(moveAmount).y) * Time.fixedDeltaTime);
                ChangeState(PlayerState.walk);

            }
            else
            {
                ChangeGoal();
            }
        }
    }

    public void ChangeGoal()
    {
        int children = pathObject.transform.childCount;
        if (currentPoint == children - 1)
        {
            currentPoint = 0;
            currentGoal = pathObject.transform.GetChild(0);
        }
        else
        {
            currentPoint++;
            currentGoal = pathObject.transform.GetChild(currentPoint);
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
            GameplayMaker.instance.DropItem(MatchID,connectionToClient,transform.position);
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
            myRigidbody.isKinematic = false;
        }
    }
}
