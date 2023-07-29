﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GameObject impactEffect;

    public int damageToGive = 50;

    public float speed = 7.5f;
    public Rigidbody2D theRb;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        theRb.velocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {


        // detect if hit enemy:
        // on the 'other' collider that we touched get the component enemy controller and deal damage
        // only if other object's unity tag is = enemy

        AudioManager.instance.PlaySFX(4);

        if (other.tag == "Enemy")
        {
            other.GetComponent<EnemyController>().DamageEnemy(damageToGive);
        } else
        {
            Instantiate(impactEffect, transform.position, transform.rotation);
        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

}
