using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    [Header("Movement")]
    private CharacterController controller; 
    [SerializeField] float wallRunForce = 2.5f; 
    //[SerializeField] float gravity = 12.0f;
    //private bool canSlide = false; // have to create a timer for sliding so player cannot spam slide ;) 
    private float slidingTimer = 3.0f; 
    private float verticalVelocity;

    [Header("Jumping")]
    //Force fpr when player swipes down to fall faster. 
    [SerializeField] float fallForce = 8.0f; 

    //Force for player jump. 
    [SerializeField] float JumpForce = 4.0f;


    [SerializeField] int NumberOfJumpsAllowed = 2;
    [SerializeField] float CooldownBetweenJumps = 0f;
    [SerializeField] bool JumpsAllowedWhenGroundedOnly;
    [SerializeField] float MinimalDelayBetweenJumps = 0.02f;
    [SerializeField] float UngroundedDurationAfterJump = 0.2f;
    [SerializeField] float GroundDistanceTolerance = 0.05f;
    [SerializeField] bool ShouldResetPosition = true;
    [SerializeField] float ResetPositionSpeed = 0.5f;
    [SerializeField] int _numberOfJumpsLeft;
    [SerializeField] GameObject _ground;
    private Vector3 _initialPosition; 
    private float _distanceToTheGroundRaycastLength = 50f;
    private RigidBodyInterface _rigidbodyInterface; 
    bool _grounded; 
    public float DistanceToTheGround { get; protected set; }
    bool _jumping = false;
    float _lastJumpTime; 

    [Header("Speed")]
    private float originalSpeed = 7.0f; 
    static public float speed;
    [SerializeField] float MAX_SPEED;
    [SerializeField] float speedIncreaseLastTick;
    [SerializeField] float speedIncreaseTime = 2.5f;
    [SerializeField] float speedIncreaseAmount = 0.5f;
    private Transform playerOrigin;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip moneyPickUpSfx;
    private float volume = 0.5f;

    [Header("Lane Running")]
    [SerializeField] float LaneWidth = 3f;
    [SerializeField] int NumberOfLanes = 3;
    [SerializeField] float ChangingLaneSpeed = 1f;
    int _currentLane;
    bool _isMoving = false; 
    
    
    static public bool isGameStarted = false;
    private bool wallRunningLeft;
    private bool wallRunningRight;
    private bool wallRunning; 
    
    //Animation
    private Animator anim; 

    private void Awake()
    {
        _currentLane = NumberOfLanes / 2;
        if (NumberOfLanes % 2 == 1) { _currentLane++; }
    }

    private void Start()
    {
        _lastJumpTime = Time.time;
        _rigidbodyInterface = GetComponent<RigidBodyInterface>(); 
        _numberOfJumpsLeft = NumberOfJumpsAllowed;
        speed = originalSpeed; 
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>(); 
    }


    private void Update()
    {
        //Save Debug For Future Bugs That Might Need To Be Fixed. 
        //Debug.Log(speed); 

        if (!isGameStarted)
        {
            return; 
        }

        if(Time.time - speedIncreaseLastTick > speedIncreaseTime && speed < MAX_SPEED)
        {
            speedIncreaseLastTick = Time.time;
            speed += speedIncreaseAmount;
            if (speed > MAX_SPEED)
            {
                speed = MAX_SPEED;
            }
            GameManager.Instance.UpdateModifier(speed - originalSpeed); 
        }

        // Inputs on which lane to be in. 
        if (MobileInputs.Instance.SwipeLeft || Input.GetKeyDown(KeyCode.A))
        {
            //Move Left
            if (_currentLane <= 1) { return; }
            if (_isMoving) { return; }
            StartCoroutine(MoveTo(transform.position + Vector3.left * LaneWidth, ChangingLaneSpeed));
            _currentLane--;
            //MoveLane(false); 
        }

        if (MobileInputs.Instance.SwipeLeft && wallRunningLeft == true || Input.GetKeyDown(KeyCode.A) && wallRunningLeft == true)
        {
            wallRunning = true; 
        }

        if (MobileInputs.Instance.SwipeRight && wallRunningRight == true || Input.GetKeyDown(KeyCode.D) && wallRunningRight == true)
        {
            wallRunning = true;
        }

        if (MobileInputs.Instance.SwipeRight || Input.GetKeyDown(KeyCode.D))
        {
            //Move Right
            if (_currentLane == NumberOfLanes) { return; }
            if (_isMoving) { return; }
            StartCoroutine(MoveTo(transform.position + Vector3.right * LaneWidth, ChangingLaneSpeed));
            _currentLane++;
            //MoveLane(true); 
        }
       
        anim.SetBool("Grounded", _grounded);

        if (_grounded)
        {
            if ((Time.time - _lastJumpTime > MinimalDelayBetweenJumps)
                && (Time.time - _lastJumpTime > UngroundedDurationAfterJump))
            {
                _jumping = false;
                _numberOfJumpsLeft = NumberOfJumpsAllowed;
            }
        }

        if(MobileInputs.Instance.SwipeDown || Input.GetKeyDown(KeyCode.S) && _grounded)
        {
            StartSliding();
            Invoke("StopSliding", 1.0f); 
        }

        if (MobileInputs.Instance.SwipeDown || Input.GetKeyDown(KeyCode.S) && !_grounded)
        {
            FallDown(); 
        }

        if (MobileInputs.Instance.SwipeUp || Input.GetKeyDown(KeyCode.W) && _grounded && wallRunning == false)
        {
            anim.SetTrigger("Jump");
            Jump(); 
        }

        if (wallRunning == true)
        {
            StartWalllRunning();
        }

        ComputeDistanceToTheGround();
        ResetPosition(); 
    }

    public void Jump()
    {
        
        if (!EvaluateJumpConditions())
        {
            return;
        }

        PerformJump();
 
    }

    private void PerformJump()
    {
        
        _lastJumpTime = Time.time;
        // we jump and decrease the number of jumps left
        _numberOfJumpsLeft--;

        // if the character is falling down, we reset its velocity
        if (_rigidbodyInterface.Velocity.y < 0)
        {
            _rigidbodyInterface.Velocity = Vector3.zero;
        }

        // we make our character jump
        ApplyJumpForce();

        _lastJumpTime = Time.time;
        _jumping = true;
    }

    private void ApplyJumpForce()
    {
        _rigidbodyInterface.AddForce(Vector3.up * JumpForce);
    }

    private void FallDown()
    {
        _rigidbodyInterface.AddForce(Vector3.down * fallForce); 
    }

    private bool EvaluateJumpConditions()
    {
        // if the character is not grounded and is only allowed to jump when grounded, we do not jump
        if (JumpsAllowedWhenGroundedOnly && !_grounded)
        {
            return false;
        }

        // if the character doesn't have any jump left, we do not jump
        if (_numberOfJumpsLeft <= 0)
        {
            return false;
        }

        // if we're still in cooldown from the last jump AND this is not the first jump, we do not jump
        if ((Time.time - _lastJumpTime < CooldownBetweenJumps) && (_numberOfJumpsLeft != NumberOfJumpsAllowed))
        {
            return false;
        }
        return true;
    }

    void ComputeDistanceToTheGround()
    {
        DistanceToTheGround = -1;
        if (_rigidbodyInterface.Is3D)
        {
            // we cast a ray to the bottom to check if we're above ground and determine the distance
            RaycastHit raycast3D = Raycast3D(transform.position, Vector3.down, _distanceToTheGroundRaycastLength, 1 << LayerMask.NameToLayer("Ground"), Color.green, true);


            if (raycast3D.transform != null)
            {
                DistanceToTheGround = raycast3D.distance;
                _ground = raycast3D.collider.gameObject;
            }
            else
            {
                DistanceToTheGround = -1;
                _ground = null;
            }
            _grounded = DetermineIfGroudedConditionsAreMet();
        }
    }

    public void SetInitialPosition(Vector3 initialPosition)
    {
        _initialPosition = initialPosition;
    }

    public static RaycastHit Raycast3D(Vector3 rayOriginPoint, Vector3 rayDirection, float rayDistance, LayerMask mask, Color color, bool drawGizmo = false, QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.UseGlobal)
    {
        if (drawGizmo)
        {
            Debug.DrawRay(rayOriginPoint, rayDirection * rayDistance, color);
        }
        RaycastHit hit;
        Physics.Raycast(rayOriginPoint, rayDirection, out hit, rayDistance, mask, queryTriggerInteraction);
        return hit;
    }

    private bool DetermineIfGroudedConditionsAreMet()
    {
        if (DistanceToTheGround == -1)
        {
            return (false);
        }
        // if the distance to the ground is within the tolerated bounds, the character is grounded, otherwise it's not.
        if (DistanceToTheGround - GetPlayableCharacterBounds().extents.y < GroundDistanceTolerance)
        {
            return (true);
        }
        else
        {
            return (false);
        }
    }

    private Bounds GetPlayableCharacterBounds()
    {
        if (GetComponent<Collider>() != null)
        {
            return GetComponent<Collider>().bounds;
        }

        return GetComponent<Renderer>().bounds;
    }

    private void ResetPosition()
    {
        if (ShouldResetPosition)
        {
            if (_grounded)
            {
                _rigidbodyInterface.Velocity = new Vector3((_initialPosition.x - transform.position.x) * (ResetPositionSpeed), _rigidbodyInterface.Velocity.y, _rigidbodyInterface.Velocity.z);
            }
        }
    }

    IEnumerator MoveTo(Vector3 destination, float movementDuration)
    {
        // initialization
        float elapsedTime = 0f;
        Vector3 initialPosition = transform.position;
        _isMoving = true;

        float sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
        while (sqrRemainingDistance > float.Epsilon)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(initialPosition, destination, elapsedTime / movementDuration);
            sqrRemainingDistance = (transform.position - destination).sqrMagnitude;
            yield return null;
        }
        _isMoving = false;
    }

    private bool IsSliding()
    {
        return anim.GetBool("Sliding");
    }

    void StartWalllRunning()
    {
        anim.SetBool("WallRun", true);
        verticalVelocity = wallRunForce; 
       // wallRunning = true; // Might not delete see if when playtesting people find a bug with this being removed. 
    }

    void StopWallRunning()
    {
        anim.SetBool("WallRun", false);
        wallRunningLeft = false;
        wallRunningRight = false; // Wall running finally works!!! LETS GOOOOOOOO!!!!!!
        wallRunning = false; 
        verticalVelocity = -JumpForce; // Player needs to hit the ground faster when exiting wall run. 
    }
    
    void StartSliding()
    {
        anim.SetBool("Sliding", true);
    }

    void StopSliding()
    {
        anim.SetBool("Sliding", false);
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
        GameManager.Instance.OnDeath(); 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WallRunLeft")
        {
            wallRunningLeft = true; 
        }

        if(other.tag == "WallRunRight")
        {
            wallRunningRight = true; 
        }

        if(other.tag == "Coin")
        {
            audioSource.PlayOneShot(moneyPickUpSfx, volume);
        }

        switch (other.gameObject.tag)
        {
            case "Obstacle":
                Crash();
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "WallRunLeft")
        {
            wallRunningLeft = false;
            StopWallRunning(); // Figure out how to get player to lift in the air.
        }
        if (other.tag == "WallRunRight")
        {
            wallRunningRight = false;
            StopWallRunning(); // Figure out how to get player to lift in the air.
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
          
    }
}
