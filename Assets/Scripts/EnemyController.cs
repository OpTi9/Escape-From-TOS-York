using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Rigidbody2D theRB;
    public float moveSpeed;


    
    [Header("Chase Player")]
    public bool shouldChasePlayer;
    public float rangeToChasePlayer;
    private Vector3 moveDirection;


    [Header("Run Away")]
    public bool shouldRunAway;
    public float runAwayRange;

    [Header("Wandering")]
    public bool shouldWander; // for wandering enemy
    public float wanderLength, pauseLength; // two states = moving and stopping
    private float wanderCounter, pauseCounter;
    private Vector3 wanderDirection; // which direction to move


    // patrolling enemy

    [Header("Patrolling")]
    public bool shouldPatrol;
    public Transform[] patrolPoints;
    private int currentPatrolPoint;

    // shooting
    [Header("Shooting")]
    public bool shouldShoot;
    public GameObject bullet;
    public Transform firePoint;
    public float fireRate;
    private float fireCounter;
    public float shootRange;

    [Header("Variables")]
    public Animator anim;
    public SpriteRenderer theBody;
    public int health = 150;

    public GameObject[] deathSplatters;

    public GameObject hitEffect;

    [Header("Drops")]
    public bool shouldDropItem;
    public GameObject[] itemsToDrop;
    public float itemDropRate;

    // Start is called before the first frame update
    void Start()
    {
        if (shouldWander)
        {
            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {


        if(theBody.isVisible && PlayerController.instance.gameObject.activeInHierarchy)
        {

            moveDirection = Vector3.zero;

            // move to the player if distance
            if (Vector3.Distance(transform.position, PlayerController.instance.transform.position) < rangeToChasePlayer && shouldChasePlayer)
            {
                
                moveDirection = PlayerController.instance.transform.position - transform.position;
                
            } else
            {
                // WANDER----------------------------
                if (shouldWander)
                {


                    if (wanderCounter > 0)
                    {

                        wanderCounter -= Time.deltaTime;

                        // move the enemy
                        moveDirection = wanderDirection;


                        if (wanderCounter <= 0)
                        {
                            pauseCounter = Random.Range(pauseLength * .75f, pauseLength * 1.25f);
                        }

                    }


                    if (pauseCounter > 0)
                    {
                        pauseCounter -= Time.deltaTime;

                        if (pauseCounter <= 0)
                        {
                            wanderCounter = Random.Range(wanderLength * .75f, wanderLength * 1.25f);

                            wanderDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0f);

                        }

                    }


                }

                //PATROL-------------------------------------

                if (shouldPatrol)
                {
                    
                    moveDirection = patrolPoints[currentPatrolPoint].position - transform.position;
                    //Debug.Log(Vector3.Distance(patrolPoints[currentPatrolPoint].position, transform.position));
                    if ( Vector3.Distance(patrolPoints[currentPatrolPoint].position, transform.position ) <= 4f)
                    {
                        currentPatrolPoint = currentPatrolPoint + 1;

                        if (currentPatrolPoint >= patrolPoints.Length)
                        {
                            currentPatrolPoint = 0;
                        }

                    }

                }


            }



            // runaway if coward
            if (shouldRunAway && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < runAwayRange)
            {
                moveDirection = transform.position - PlayerController.instance.transform.position;
            }

            //else
            //{
            //moveDirection = Vector3.zero;
            //}





            moveDirection.Normalize();

            theRB.velocity = moveDirection * moveSpeed;


            if (shouldShoot && Vector3.Distance(transform.position, PlayerController.instance.transform.position) < shootRange)
            {
                fireCounter -= Time.deltaTime;
                if (fireCounter <= 0)
                {
                    fireCounter = fireRate;
                    Instantiate(bullet, firePoint.position, firePoint.rotation);
                    AudioManager.instance.PlaySFX(13);
                }
            }
        } else
        {
            theRB.velocity = Vector2.zero;
        }


        // animation transition
        if (moveDirection != Vector3.zero)
        {
            anim.SetBool("IsMoving", true);
        }
        else
        {
            anim.SetBool("IsMoving", false);
        }

    }


    public void DamageEnemy(int damage)
    {
        health -= damage;

        Instantiate(hitEffect, transform.position, transform.rotation);
        AudioManager.instance.PlaySFX(22);

        if (health <= 0)
        {
            Destroy(gameObject);
            AudioManager.instance.PlaySFX(21);

            int selectedSplatter = Random.Range(0, deathSplatters.Length);
            int rotation = Random.Range(0, 4);

            Instantiate(deathSplatters[selectedSplatter], transform.position, Quaternion.Euler(0f, 0f, rotation * 90f));
            //Instantiate(deathSplatter, transform.position, transform.rotation);

            // drop item

            if (shouldDropItem)
            {
                float dropChance = Random.Range(0f, 100f);

                if (dropChance < itemDropRate)
                {
                    int randomItem = Random.Range(0, itemsToDrop.Length);
                    Instantiate(itemsToDrop[randomItem], transform.position, transform.rotation);
                }

            }

        }
    }

}
