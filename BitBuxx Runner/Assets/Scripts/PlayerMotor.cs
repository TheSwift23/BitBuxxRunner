using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller; // Grabbing PlayerController from the player. 
    private Vector3 moveVector; 
    [SerializeField] float speed = 5.0f; // Player speed (meters per second). 
    [SerializeField] float gravity = 20f;

    [SerializeField] float jumpForce; 

    private float animationDuration = 3.0f;

    void Start()
    {
        controller = GetComponent<CharacterController>(); // Gets CharacterController from the Player. 
    }


    void Update()
    { 

        if(Time.time < animationDuration)
        {
            controller.Move(Vector3.forward * speed * Time.deltaTime);
            return; 
        }

        if (controller.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                Jump();
            }
        }
        else
        {
            moveVector.y -= gravity * Time.deltaTime;
        }
 
        //x
        moveVector.x = Input.GetAxisRaw("Horizontal") * speed; // MoveVector x moves character when horizontal buttons are pressed. 
        //z
        moveVector.z = speed; // Character moves forward. 

        
    }

    private void FixedUpdate()
    {
        controller.Move(moveVector * Time.fixedDeltaTime); // Character moves forward every second. 

    }

    private void Jump()
    {
        moveVector.y = jumpForce; 
    }
}
