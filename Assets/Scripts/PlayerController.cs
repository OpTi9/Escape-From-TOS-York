using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // reference to always locate our player for enemies etc
    public static PlayerController instance;

    // public = visible in unity ui
    public float moveSpeed;
    private Vector2 moveInput;

    // we will script moving using rigidbody to awoid jittering near the walls
    public Rigidbody2D theRb;

    public Transform gunArm;

    private Camera theCam;

    public Animator anim;

  /*  public GameObject bulletToFire;

    public Transform firePoint;

    // to autofire if holding lmb
    public float timeBetweenShots;
    private float shotCounter;
  */

    // transparency when hit by enemy
    public SpriteRenderer bodySR;


    // dash mechanic
    private float activeMoveSpeed;
    public float dashSpeed = 8f, dashLength = .5f, dashCoolDown = 1f, dashInvincibility = .5f;

    [HideInInspector]
    public float dashCounter;

    private float dashCoolCounter;


    // stop moving at the end of lvl
    [HideInInspector]
    public bool canMove = true;



    // starts as soon as object starts existing (before start function, or reactivate object
    private void Awake()
    {
        // define our static object so enemies always see the player distance
        instance = this;
    }




    // Start is called before the first frame update
    void Start()
    {
        // using camera.main is not recommended because uses too much memory so we create a reference one time at the start
        theCam = Camera.main;

        activeMoveSpeed = moveSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if (canMove && !LevelManager.instance.isPaused) // disable this to bullet time)   // so we can disable movement at the end of lvl
        {

            moveInput.x = Input.GetAxisRaw("Horizontal");
            moveInput.y = Input.GetAxisRaw("Vertical");
            moveInput.Normalize();


            // if player presses move button add this value to current position
            // transform.position += new Vector3(moveInput.x * Time.deltaTime * moveSpeed, moveInput.y * Time.deltaTime * moveSpeed, 0f);
            // we will script moving using rigidbody to awoid jittering near the walls


            theRb.velocity = moveInput * activeMoveSpeed;



            Vector3 mousePosition = Input.mousePosition;


            // rather than convert moouse position into the world its better to 
            // get pos of player and convert that into an area on our camera and then get the difference between 2 points to get an angle
            // using camera.main is not recommended because uses too much memory
            //Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);


            Vector3 screenPoint = theCam.WorldToScreenPoint(transform.localPosition);

            // check if mouse pos to the right
            if (mousePosition.x < screenPoint.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
                gunArm.localScale = new Vector3(-1f, -1f, 1f);
            }
            else
            {
                transform.localScale = Vector3.one;
                gunArm.localScale = Vector3.one;
            }

            // rotate gun arm
            Vector2 offset = new Vector2(mousePosition.x - screenPoint.x, mousePosition.y - screenPoint.y);
            // calculate the angle based on offset
            float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
            gunArm.rotation = Quaternion.Euler(0, 0, angle);



        /*    if (Input.GetMouseButtonDown(0))
            {
                // create a copy of object and where to place it and with what rotation
                Instantiate(bulletToFire, firePoint.position, firePoint.rotation);
                shotCounter = timeBetweenShots;
                AudioManager.instance.PlaySFX(12);
            }

            // delete to disable auto fire when holding lmb
            if (Input.GetMouseButton(0))
            {
                shotCounter -= Time.deltaTime;

                if (shotCounter <= 0)
                {
                    Instantiate(bulletToFire, firePoint.position, firePoint.rotation);
                    shotCounter = timeBetweenShots;
                    AudioManager.instance.PlaySFX(12);
                }
            }
        */

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (dashCoolCounter <= 0 && dashCounter <= 0)
                {
                    activeMoveSpeed = dashSpeed;
                    dashCounter = dashLength;

                    anim.SetTrigger("Dash");
                    PlayerHealthController.instance.MakeInvincible(dashInvincibility);

                    AudioManager.instance.PlaySFX(8);
                }

            }


            if (dashCounter > 0)
            {
                dashCounter -= Time.deltaTime;

                if (dashCounter <= 0)
                {
                    activeMoveSpeed = moveSpeed;
                    dashCoolCounter = dashCoolDown;
                }
            }


            if (dashCoolCounter > 0)
            {
                dashCoolCounter -= Time.deltaTime;
            }




            // animation transition
            if (moveInput != Vector2.zero)
            {
                anim.SetBool("IsMoving", true);
            }
            else
            {
                anim.SetBool("IsMoving", false);
            }
        }
        else
        {
            theRb.velocity = Vector2.zero;
            anim.SetBool("IsMoving", false);
        }



    }
}
