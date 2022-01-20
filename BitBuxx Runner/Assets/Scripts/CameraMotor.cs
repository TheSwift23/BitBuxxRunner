using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    private Transform lookAt;
    private Vector3 startOffset;
    private Vector3 moveVector;

    private float transition = 0.0f; 
    private float animationDuration = 3.0f; // How long animation take in seconds. 
    private Vector3 animationOffset = new Vector3(0, 5, 5); 

    void Start()
    {
        lookAt = GameObject.FindGameObjectWithTag("Player").transform; //Grabs player transform and stores it in lookAt. 
        startOffset = transform.position - lookAt.position;     
    }


    void LateUpdate()
    {
        moveVector = lookAt.position + startOffset;
        //x
        moveVector.x = 0;
        //y
        moveVector.y = Mathf.Clamp(moveVector.y, 3, 5); 

        if(transition > 1.0f)
        {
        transform.position = moveVector;
        }
        else
        {
            //Animation at the start. 
            transform.position = Vector3.Lerp(moveVector + animationOffset, moveVector, transition);
            transition += Time.deltaTime * 1 / animationDuration;
            transform.LookAt(lookAt.position + Vector3.up); // takes camera rotation and looks at player. 
        }

    }
}
