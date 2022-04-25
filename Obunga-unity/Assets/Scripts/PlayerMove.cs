using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region Singleton
    public static PlayerMove Instance;
    void Awake()
    {
        Instance = this;
    }
    #endregion
    [Header("Assignables")]
    public Transform orientation;
    public GameObject landParticles;
    public LayerMask playerLayerMask;

    [Header("Movement")]
    public float moveSpeed;
    public float inAirSpeed;
    float horizontalMovement;
    float verticalMovement;

    [Header("Cam movement")]
    float mouseX;
    float mouseY;
    float xRotation;
    float yRotation;
    float multiplier = 0.1f;
    public bool CanLook = true;

    [Range(0, 100)]
    public float sens;

    [Header("Jumping")]
    public bool canJump;
    public float jumpCooldown = 0.25f; // the cooldown limit
    public float currentJumpCooldown = 0f; //the actual cooldown
    public float jumpForce;
    float playerHeight = 2f;
    float airTime;

    [Header("Ground Detection")]
    public LayerMask whatIsGround;
    bool isGrounded;
    bool cancelGround;
    float surfaceAngle;
    float maxSlopeAngle = 60f;
    bool wishJump;
    Vector3 normalVector;

    [Header("Drag")]
    float groundDrag = 10f;
    float airDrag = 1f;

    [Header("Crouching")]
    CapsuleCollider playerCol;
    bool underObstruction;

    [Header("Slope Stuff")]
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;

    [Header("Wallrunning")]
    public float wallrunForce;
    public float wallRunGravity;
    public float wallrunSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    
    [Header("Camera")]
    public Transform camHolder;
    public float camTilt;
    public float WallRunCamTiltTime;
    public float camTiltTime;

    [Header("Footsteps")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public AudioClip[] landingClips;
    public AudioClip slideClip;
    float baseStepSpeed = 0.3f;
    float footstepTimer = 0f;
    float GetCurrentOffset => baseStepSpeed;

    float extraGravityForce = -200f;

    public float tilt { get; private set; }

    private bool OnSlope()
    {
        if (isGrounded && Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 1f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCol = GetComponentInChildren<CapsuleCollider>();
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        MyInput();
        ControlDrag();
        CheckForWall();
        //CheckLanding();
        CheckAirTime();
        HandleFootsteps();
        Look();

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        underObstruction = Physics.Raycast(transform.position, Vector3.up, playerHeight / 2 + 0.15f);

       /* if(isGrounded)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 2f, 0.1f);
            playerCol.center = Vector3.Lerp(playerCol.center, Vector3.zero, 0.1f);
        } 
        else
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 1f, 0.1f);
            playerCol.center = Vector3.Lerp(playerCol.center, new Vector3(0, 0.5f, 0), 0.1f);
        }     */
    }

    private void OnCollisionStay(Collision other)
    {
        //Gets the layer we collided with
        var layer = other.gameObject.layer;
        //Shifts 1 by the layer we collided with
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Loop through each contact point
        for (int i = 0; i < other.contactCount; i++)
        {
            Vector3 normal = other.GetContact(i).normal;

            if (IsFloor(normal))
            {
                isGrounded = true;
                cancelGround = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        float delay = 3f;
        if (!cancelGround)
        {
            cancelGround = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    public bool IsFloor(Vector3 v)
    {
        surfaceAngle = Vector3.Angle(v, Vector3.up);
        return surfaceAngle < maxSlopeAngle;
    }

    private void StopGrounded()
    {
        isGrounded = false;
    }

    void FixedUpdate()
    {
        MovePlayer();
        CameraTilting();

        if(wishJump)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.VelocityChange);
            wishJump = false;
        }

       rb.AddForce(Vector3.up * extraGravityForce, ForceMode.Force);
    }
    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

        if(currentJumpCooldown >= jumpCooldown)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
            currentJumpCooldown += Time.deltaTime; 
            currentJumpCooldown = Mathf.Clamp(currentJumpCooldown, 0f, jumpCooldown);
        }

        if (Input.GetKey(KeyCode.Space) && isGrounded && canJump)
        {
            Jump();
            currentJumpCooldown = 0f;
        }
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            if (OnSlope())
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed, ForceMode.Acceleration);
                //Vector3 gravityForce = Physics.gravity - Vector3.Project(Physics.gravity, slopeHit.normal);
                // rb.AddForce(-gravityForce, ForceMode.Acceleration);

            }
            else
            {
                rb.AddForce(moveDirection.normalized * moveSpeed, ForceMode.Acceleration);
            }
        }
        else
        {
            rb.AddForce(moveDirection.normalized * inAirSpeed, ForceMode.Acceleration);
        }
    }

    void Jump()
    {
        wishJump = true;
        print("jumped");
    }

    void Look()
    {
        if(CanLook == true)
        {
            mouseX = Input.GetAxisRaw("Mouse X");
            mouseY = Input.GetAxisRaw("Mouse Y");

            yRotation += mouseX * sens * multiplier;
            xRotation -= mouseY * sens * multiplier;

            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
            orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
        }
    } 

    void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        
        if(!isGrounded)
        {
            rb.drag = airDrag;
        }

        if(!isGrounded && isWallRunning)
        {
            rb.drag = airDrag * 2;
        }
    } 

    void CheckAirTime()
    { 
        if(isGrounded  || OnSlope() || isWallRunning)
        {
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }
    }

   /* void CheckLanding()
    {
        if (airTime >= 0.3f)
        {
            if(isGrounded || isGrounded && OnSlope())
            {
                GameObject Particles = Instantiate(landParticles, new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), Quaternion.Euler(90, 0, 0));
                Destroy(Particles, 2f);
                footstepTimer = GetCurrentOffset;
            }
            
        }

        if (rb.velocity.magnitude >= 0.5f && airTime >= 3f)
        {
            if (isGrounded || isGrounded && OnSlope()) footstepAudioSource.PlayOneShot(landingClips[Random.Range(0, landingClips.Length - 1)]);
        }
    } */

    void CheckForWall() 
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.7f, ~playerLayerMask);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.7f, ~playerLayerMask);

        if (isWallRight && !isGrounded || isWallLeft && !isGrounded) Wallrun();

        if (!isWallLeft && !isWallRight) 
        {
            StopWallRun();
        }
    }

    void Wallrun()
    {
        rb.useGravity = false;
        isWallRunning = true;

            //slowly slide down
            rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

            if (isWallRight && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce + -orientation.right, ForceMode.VelocityChange);
                rb.AddForce(transform.right * jumpForce + -orientation.right, ForceMode.VelocityChange);
        }

            if (isWallLeft && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce + orientation.right, ForceMode.VelocityChange);
            rb.AddForce(-transform.right * jumpForce + orientation.right, ForceMode.VelocityChange);
            }
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    void HandleFootsteps()
    {
        if (!isGrounded || rb.velocity.magnitude <= 0) return;

        footstepTimer -= Time.deltaTime;

        if (rb.velocity.magnitude >= 6 && footstepTimer <= 0 && isGrounded)
        {
            footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length - 1)]);

            footstepTimer = GetCurrentOffset;
        }
    }

    void CameraTilting()
    {
        //i hate
        if (isWallRunning)
        {
            if (isWallRight && !isGrounded) tilt = Mathf.Lerp(tilt, camTilt, WallRunCamTiltTime * Time.deltaTime);
            if (isWallLeft && !isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, WallRunCamTiltTime * Time.deltaTime);
        }
        else
        {
            if (Input.GetKey(KeyCode.A) && isGrounded) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime / 2);
            else tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
            if (Input.GetKey(KeyCode.D) && isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime / 2);
            else tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        }
    }

}
