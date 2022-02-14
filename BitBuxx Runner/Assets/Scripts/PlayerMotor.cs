using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    // Movement
    private CharacterController controller; 
    [SerializeField] float jumpForce = 4.0f;
    [SerializeField] float gravity = 12.0f;
    private float verticalVelocity;
    [SerializeField] int desiredLane = 1; // 0 = Left; 1 = Middle; 2 = Right 

    //Speed Modifier 
    private float originalSpeed = 7.0f; 
    [SerializeField] float speed;
    [SerializeField] float speedIncreaseLastTick;
    [SerializeField] float speedIncreaseTime = 2.5f;
    [SerializeField] float speedIncreaseAmount = 0.1f; 
   
    private const float LANE_DISTANCE = 3.0f;
    private const float TURN_SPEED = 0.05f;

    private bool isGameStarted = false; 

    //Animation
    private Animator anim; 

    private void Start()
    {
        speed = originalSpeed; 
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>(); 
    }

    private void Update()
    {
        if (!isGameStarted)
        {
            return; 
        }

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            GameManager.Instance.UpdateModifier(speed - originalSpeed); 
        }

        // Inputs on which lane to be in. 
        if (MobileInputs.Instance.SwipeLeft)
        {
            //Move Left
            MoveLane(false); 
        }
        if (MobileInputs.Instance.SwipeRight)
        {
            //Move Right
            MoveLane(true); 
        }

        //Calcuate where to be in the future
        Vector3 targetPosition = transform.position.z * Vector3.forward;
        if(desiredLane == 0)
        {
            targetPosition += Vector3.left * LANE_DISTANCE; 
        }else if(desiredLane == 2)
        {
            targetPosition += Vector3.right * LANE_DISTANCE;
        }

        //Calculate move delta 
        Vector3 moveVector = Vector3.zero;
        moveVector.x = (targetPosition - transform.position).normalized.x * speed;

        bool isGrounded = IsGrounded(); 

        // Calculate Y 
        if (IsGrounded()) // if grounded 
        {
            //verticalVelocity = -0.1f;
            anim.SetBool("Grounded", isGrounded);
            if (MobileInputs.Instance.SwipeUp)
            {
                //Jump
                anim.SetTrigger("Jump"); 
                verticalVelocity = jumpForce; 
            }else if (MobileInputs.Instance.SwipeDown)
            {
                //Slide
                StartSliding();
                Invoke("StopSliding", 1.0f); 
            }
        }
        else
        {
            verticalVelocity -= (gravity * Time.deltaTime);

            // Fast Falling 
            if (MobileInputs.Instance.SwipeDown)
            {
                verticalVelocity = -jumpForce; 
            }
        }



        moveVector.y = verticalVelocity;
        moveVector.z = speed;

        //Move Player 
        controller.Move(moveVector * Time.deltaTime);

        // Rotate Player 
        Vector3 dir = controller.velocity;
        if(dir != Vector3.zero)
        {
            dir.y = 0;
            transform.forward = Vector3.Lerp(transform.forward, dir, TURN_SPEED); 
        }
    }

    void StartSliding()
    {
        anim.SetBool("Sliding", true);
        controller.height /= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y / 2, controller.center.z); 
    }

    void StopSliding()
    {

        anim.SetBool("Sliding", false);
        controller.height *= 2;
        controller.center = new Vector3(controller.center.x, controller.center.y * 2, controller.center.z);
    }
    private void MoveLane(bool goingRight)
    {
        desiredLane += (goingRight) ? 1 : -1;
        desiredLane = Mathf.Clamp(desiredLane, 0, 2); 
    }

    private bool IsGrounded()
    {
        Ray groundRay = new Ray(new Vector3(controller.bounds.center.x, (controller.bounds.center.y - controller.bounds.extents.y) + 0.2f, controller.bounds.center.z), Vector3.down);
        Debug.DrawRay(groundRay.origin, groundRay.direction, Color.cyan, 1.0f);

        return (Physics.Raycast(groundRay, 0.2f + 0.1f));
    }

    public void StartGame()
    {
        isGameStarted = true;
        anim.SetTrigger("StartRunning"); 
    }

    void Crash()
    {
        anim.SetTrigger("Death");
        isGameStarted = false;
        GameManager.Instance.IsDead = true; 
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
          switch (hit.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break; 
        }
    }
}
