using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideSpawner : MonoBehaviour
{
    //How far should buildings be behind the player before pushing back infront of him. 
    private const float DISTANCE_TO_RESPAWN = 55.0f;

    //Additional paralax effect. 
    public float scrollSpeed = -4.5f;
    public float totalLength; 
    public bool IsScrolling { set; get; }

    private float scrollLocation;
    private Transform playerTransform;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        //IsScrolling = true; //used for testing
    }

    //Moves the buildings infront of the player.
    private void Update()
    {
        if (!IsScrolling)
            return;
        scrollLocation += scrollSpeed * Time.deltaTime;
        Vector3 newLocation = (playerTransform.position.z + scrollLocation) * Vector3.forward;
        transform.position = newLocation; 

        if(transform.GetChild(0).transform.position.z < playerTransform.position.z - DISTANCE_TO_RESPAWN)
        {
            transform.GetChild(0).localPosition += Vector3.forward * totalLength;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);

            transform.GetChild(0).localPosition += Vector3.forward * totalLength;
            transform.GetChild(0).SetSiblingIndex(transform.childCount);
        }
    }
}
