using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private CharacterController controller; // Character controller attached to the player. 
    private Vector3 direction; //We are using vector 3 to move the player. 
    [SerializeField] float forwardSpeed; // Speed of the player. 

    private int desiredLane = 1;
    [SerializeField] float laneDistance = 4f; //Distance between two lanes. 
    [SerializeField] float jumpForce;
    [SerializeField] float Gravity = -20f; 

    void Start()
    {
        controller = GetComponent<CharacterController>(); //Grabbing the character controller and storing it. 
    }

    void Update()
    {
        direction.z = forwardSpeed;
        

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            direction.y += Gravity * Time.deltaTime;
        }

        //Get inputs which lane to be in. 
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            desiredLane++; 
            if(desiredLane == 3)
            {
                desiredLane = 2; 
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            desiredLane--;
            if (desiredLane == -1)
            {
                desiredLane = 0;
            }
        }

        //Calculate where to be in the future. 
        Vector3 targetPosition = transform.position.z * transform.forward + transform.position.y * transform.up; 

        if(desiredLane == 0)
        {
            targetPosition += Vector3.left * laneDistance; 
        }else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * laneDistance; 
        }

        Vector3 diff = targetPosition - transform.position;
        Vector3 moveDir = diff.normalized * 25 * Time.deltaTime;

        // transform.position = Vector3.Lerp(transform.position,targetPosition, 100*Time.fixedDeltaTime); // How fast player switches between lanes.  
        if (transform.position == targetPosition)
        {
            return;
        }

        if (moveDir.sqrMagnitude < diff.sqrMagnitude)
        {
            controller.Move(moveDir);
        }
        else
        {
            controller.Move(diff);
        }

    }

    private void FixedUpdate()
    {
        controller.Move(direction * Time.fixedDeltaTime); // Character moves in a direction per frame. 
    }

    private void Jump()
    {
        direction.y = jumpForce; 
    }
}
