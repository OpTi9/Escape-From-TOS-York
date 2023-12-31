﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public static CameraController instance;

    public float moveSpeed;

    public Transform target;


    // big map
    public Camera mainCamera, bigMapCamera;
    private bool bigMapActive;



    private void Awake()
    {
        instance = this;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x, target.position.y, transform.position.z), moveSpeed * Time.deltaTime);
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            if (!bigMapActive)
            {
                ActivateBigMap();
            }
            else
            {
                DeactivateBigMap();
            }
        }
        
    }

    public void ChangeTarget(Transform newTarget)
    {
        target = newTarget;
    }


    public void ActivateBigMap()
    {

        if (!LevelManager.instance.isPaused) // check if its not paused
        {
            bigMapActive = true;
            bigMapCamera.enabled = true;
            mainCamera.enabled = false;

            // stop moving
            PlayerController.instance.canMove = false;
            Time.timeScale = 0f;

            UIController.instance.mapDisplay.SetActive(false);
            UIController.instance.bigMapText.SetActive(true);
        }


    }

    public void DeactivateBigMap()
    {
        if (!LevelManager.instance.isPaused)
        {
            bigMapActive = false;
            bigMapCamera.enabled = false;
            mainCamera.enabled = true;

            PlayerController.instance.canMove = true;
            Time.timeScale = 1f;
            UIController.instance.mapDisplay.SetActive(true);
            UIController.instance.bigMapText.SetActive(false);
        }

    }

}
