using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [Header("Assignables")]
    public Transform orientation;
    public Animation playerLandAnimation;
    public ParticleSystem wooshLines;
    public GameObject landParticles;

    [Header("Movement")]
    float moveSpeed = 10f;
    float inAirSpeed = 6f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Cam movement")]
    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;
    public float sensX;
    public float sensY;
    
    [Header("Multipliers")]
    public float movementMultiplier = 15f;
    public float airMultiplier = 20f;
    public float crouchingMultiplier = 7.5f;

    [Header("Jumping & Land Detection")]
    float playerHeight = 2f;
    float jumpForce = 10f;
    float groundDistance = 0.4f;
    float airTime;
    public LayerMask groundMask;
    bool isGrounded;
    bool isInAir;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 1f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    public Transform playerScale;
    bool isCrouching;
    bool canUncrouch;
    bool canCrouch;
    bool aboveObstruction;
    RaycastHit obstructionHit;

    [Header("Sliding & Diving")]
    float slideForce = 40f;
    bool isSliding;


    [Header("Slope Stuff")]
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;

    [Header("Wallrunning")]
    public float wallrunForce, maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;
    
    [Header("Camera")]
    public Camera cam;
    public float camTilt;
    public float camTiltTime;

    [Header("Footsteps")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public AudioClip[] landingClips;
    float baseStepSpeed = 0.3f;
    float crouchStepMultiplier = 1.5f;
    float footstepTimer = 0f;
    float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultiplier : baseStepSpeed;

    public float tilt { get; private set; }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f))
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
        rb.freezeRotation = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {
        MyInput();
        ControlDrag();
        CheckForWall();
        WallRunInput();
        CheckLanding();
        CheckAirTime();
        Look();
        HandleFootsteps();
        

        //above obstruction check
        aboveObstruction = Physics.Raycast(transform.position, Vector3.up, out obstructionHit, playerHeight * 2f);

        //slope stuff
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            Uncrouch();
        }

        if (isCrouching)
        {
            isGrounded = Physics.CheckSphere(transform.position, 1f, groundMask);
        }
        else
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f, groundMask);
        }

        if(aboveObstruction)
        {
            canCrouch = false;
            canUncrouch = false;
        }
        else
        {
            canCrouch = true;
            canUncrouch = true;
        }
    }
    void FixedUpdate()
    {
        MovePlayer();

        if (isGrounded)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 2f, 0.3f);
        }
        else if (!isGrounded)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 0.5f, 0.3f);
        }
    }

    void Look()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        // cam.transform.LookAt(cam.transform.position + rb.velocity);
    }

    void MovePlayer()
    {  
        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }  
        else if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }  

        if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * inAirSpeed * airMultiplier, ForceMode.Acceleration);
        }
    }

    void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = airDrag;
        }
    }

    void CheckAirTime()
    { 
        if(isGrounded || OnSlope() || isWallRunning || isCrouching || isSliding || aboveObstruction)
        {
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
            isInAir = true;
        }
    }

    void CheckLanding()
    {
        if (rb.velocity.magnitude >= 4 && airTime >= 2f)
        {
            if(isGrounded)
            {
                GameObject Particles = Instantiate(landParticles, new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), Quaternion.Euler(90, 0, 0));
                Destroy(Particles, 2f);
            }
            
        }
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Crouch()
    {
       if(canCrouch)
        {
            playerScale.localScale = new Vector3(1, 0.5f, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            isCrouching = true;

            if (rb.velocity.magnitude > 6f && isGrounded)
            {
                rb.AddForce(moveDirection * slideForce, ForceMode.Impulse);
                wooshLines.Play();
            }
        }
    }

    void Uncrouch()
    {
       if(canUncrouch)
        {
            playerScale.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
            isCrouching = false;
        }  
    }

    void WallRunInput() 
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();


        if (isWallRunning)
        {
            if (isWallRight && Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.up * jumpForce * 3f); 
                rb.AddForce(-orientation.right * jumpForce * 10f);
            }
            if (isWallLeft && Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.up * jumpForce * 3f);
                rb.AddForce(orientation.right * jumpForce * 10f);
            }
        }
    }

    void StartWallrun()
    {
        rb.useGravity = false;
        isWallRunning = true;

            rb.AddForce(orientation.forward * wallrunForce * Time.deltaTime);

            //Make sure char sticks to wall
            if (isWallRight)
            {
                rb.AddForce(orientation.right * wallrunForce / 5 * Time.deltaTime);
                tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime);
            } 
            else if (isWallLeft)
            {
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
                tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime);
            }         
        
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }

    void CheckForWall() 
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.65f);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.65f);

        //leave wall run
        if (!isWallLeft && !isWallRight) 
        {
            StopWallRun();
        }
    }

    void HandleFootsteps()
    {
        if (!isGrounded) return;

        if(rb.velocity.magnitude <= 0) return;

        footstepTimer -= Time.deltaTime;

        if(rb.velocity.magnitude >= 2 && footstepTimer <= 0 && isGrounded)
        {
            footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length - 1)]);

            footstepTimer = GetCurrentOffset;
        }
    }

}
