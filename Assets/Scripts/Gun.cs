using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{

    public GameObject bulletToFire;

    public Transform firePoint;

    // to autofire if holding lmb
    public float timeBetweenShots;
    private float shotCounter;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (PlayerController.instance.canMove && !LevelManager.instance.isPaused)
        {

            if (shotCounter > 0)
            {
                shotCounter -= Time.deltaTime;
            }
            else
            {


                if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
                {
                    // create a copy of object and where to place it and with what rotation
                    Instantiate(bulletToFire, firePoint.position, firePoint.rotation);
                    shotCounter = timeBetweenShots;
                    AudioManager.instance.PlaySFX(12);
                }


                // auto fire when holding lmb
              /*  if (Input.GetMouseButton(0))
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


            }


        }


    }
}
