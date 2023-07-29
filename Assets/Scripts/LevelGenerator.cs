﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShortcutManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    
    public GameObject layoutRoom;

    public Color startColor, endColor, shopColor;

    public int distanceToEnd;
    public bool includeShop;
    public int minDistanceToShop, maxDistanceToShop;

    public Transform generatorPoint;


    public enum Direction { up, right, down, left};
    public Direction selectedDirection;

    public float xOffset = 18f, yOffset = 10f;


    public LayerMask whatIsRoom;

    // store last room
    private GameObject endRoom, shopRoom;

    // all the rooms
    private List<GameObject> layoutRoomObjects = new List<GameObject>();


    public RoomPrefabs rooms;

    // store our gener outlines
    private List<GameObject> generatedOutlines = new List<GameObject>();


    // rooms centers to create

    public RoomCenter centerStart, centerEnd, centerShop; // add new rooms here
    public RoomCenter[] potentialCenters;



    // Start is called before the first frame update
    void Start()
    {
        Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation).GetComponent<SpriteRenderer>().color = startColor;
        selectedDirection = (Direction)Random.Range(0, 4);
        MoveGenerationPoint();

        for (int i = 0; i < distanceToEnd; i++)
        {


            GameObject newRoom = Instantiate(layoutRoom, generatorPoint.position, generatorPoint.rotation);

            // add it to the list
            layoutRoomObjects.Add(newRoom);

            // are we at the end of foor loop
            if (i + 1 == distanceToEnd)
            {

                newRoom.GetComponent<SpriteRenderer>().color = endColor;

                // if its end room take it off the rooms list because we dont need start and end rooms in the list
                layoutRoomObjects.RemoveAt(layoutRoomObjects.Count -1);

                endRoom = newRoom;

            }



            selectedDirection = (Direction)Random.Range(0, 4);
            MoveGenerationPoint();


            // prevent rooms overlapping
            while (Physics2D.OverlapCircle(generatorPoint.position, .2f, whatIsRoom))
            {
                MoveGenerationPoint();
            }


        }

        // include the shop
        if (includeShop)
        {
            int shopSelector = Random.Range(minDistanceToShop, maxDistanceToShop + 1);
            shopRoom = layoutRoomObjects[shopSelector];
            layoutRoomObjects.RemoveAt(shopSelector); // dont generate random room in this place
            shopRoom.GetComponent<SpriteRenderer>().color = shopColor;
        }



        // create room outlines

        // start room
        CreateRoomOutline(Vector3.zero);

        foreach (GameObject room in layoutRoomObjects)
        {
            CreateRoomOutline(room.transform.position);
        }

        CreateRoomOutline(endRoom.transform.position);

        // adding new room--------------------------------------------------------------------
        if (includeShop)
        {
            CreateRoomOutline(shopRoom.transform.position);
        }




        // 75 adding centers to generation
        foreach (GameObject outline in generatedOutlines)
        {

            bool generateCenter = true;

            // if its a start position
            if (outline.transform.position == Vector3.zero)
            {
                Instantiate(centerStart, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }

            if (outline.transform.position == endRoom.transform.position)
            {
                Instantiate(centerEnd, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                generateCenter = false;
            }


            if (includeShop)
            {
                if (outline.transform.position == shopRoom.transform.position)
                {
                    Instantiate(centerShop, outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
                    generateCenter = false;
                }
            }
            

            if (generateCenter)
            {
                int centerSelect = Random.Range(0, potentialCenters.Length);

                Instantiate(potentialCenters[centerSelect], outline.transform.position, transform.rotation).theRoom = outline.GetComponent<Room>();
            }
            


        }


    }

    // Update is called once per frame
    void Update()
    {

#if UNITY_EDITOR
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

#endif

    }

    public void MoveGenerationPoint()
    {

        switch (selectedDirection)
        {
            case Direction.up:
                generatorPoint.position += new Vector3(0f, yOffset, 0f);
                break;
            case Direction.right:
                generatorPoint.position += new Vector3(0f, -yOffset, 0f);
                break;
            case Direction.down:
                generatorPoint.position += new Vector3(xOffset, 0f, 0f);
                break;
            case Direction.left:
                generatorPoint.position += new Vector3(-xOffset, 0f, 0f);
                break;
            default:
                break;
        }

    }


    public void CreateRoomOutline(Vector3 roomPosition)
    {
        bool roomAbove = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, yOffset, 0f), .2f, whatIsRoom);
        bool roomBelow = Physics2D.OverlapCircle(roomPosition + new Vector3(0f, -yOffset, 0f), .2f, whatIsRoom);
        bool roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0f, 0f), .2f, whatIsRoom);
        bool roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0f, 0f), .2f, whatIsRoom);


        // determine how many exits we need
        int directionCount = 0;

        if (roomAbove)
        {
            // if theres room above
            directionCount++;
        }

        if (roomBelow)
        {
            
            directionCount++;
        }

        if (roomLeft)
        {
            
            directionCount++;
        }

        if (roomRight)
        {
            
            directionCount++;
        }

        // swtich to choose how we act depending on number of exits in the room

        switch (directionCount)
        {
            case 0:

                Debug.LogError("Weird. Found no rooms!");
                break;

                // if only one exit in the room:
            case 1:

                if (roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleUp, roomPosition, transform.rotation));
                }

                if (roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleDown, roomPosition, transform.rotation));
                }

                if (roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleLeft, roomPosition, transform.rotation));
                }

                if (roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.singleRight, roomPosition, transform.rotation));
                }

                break;


            case 2:

                if (roomAbove && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpDown, roomPosition, transform.rotation));
                }

                if (roomLeft && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleLeftRight, roomPosition, transform.rotation));
                }

                if (roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleUpRight, roomPosition, transform.rotation));
                }

                if (roomRight && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleRightDown, roomPosition, transform.rotation));
                }

                if (roomBelow && roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleDownLeft, roomPosition, transform.rotation));
                }

                if (roomLeft && roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.doubleLeftUp, roomPosition, transform.rotation));
                }

                break;


            case 3:

                if (roomAbove && roomRight && roomBelow)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleUpRightDown, roomPosition, transform.rotation));
                }

                if (roomRight && roomBelow && roomLeft)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleRightDownLeft, roomPosition, transform.rotation));
                }

                if (roomBelow && roomLeft && roomAbove)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleDownLeftUp, roomPosition, transform.rotation));
                }

                if (roomLeft && roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.tripleLeftUpRight, roomPosition, transform.rotation));
                }

                break;


            case 4:

                if (roomBelow && roomLeft && roomAbove && roomRight)
                {
                    generatedOutlines.Add(Instantiate(rooms.fourway, roomPosition, transform.rotation));
                }

                break;

        }



    }


}


[System.Serializable]
public class RoomPrefabs
{
    public GameObject 
        singleUp, singleDown, singleRight, singleLeft,
        doubleUpDown, doubleLeftRight, doubleUpRight, doubleRightDown, doubleDownLeft, doubleLeftUp,
        tripleUpRightDown, tripleRightDownLeft, tripleDownLeftUp, tripleLeftUpRight,
        fourway;
}