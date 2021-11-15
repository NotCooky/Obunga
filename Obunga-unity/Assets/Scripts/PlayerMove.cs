using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EZCameraShake;

public class PlayerMove : MonoBehaviour
{
    [Header("Assignables")]
    public Transform orientation;
    public Transform cam;
    public Animation playerLandAnimation;
    public ParticleSystem landParticles;

    [Header("Movement")]
    float moveSpeed = 6f;
    float inAirSpeed = 3f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Speed display")]
    Vector3 PreviousFramePosition = Vector3.zero; // Or whatever your initial position is
    public float Speed = 0f;

    //camera movement
    float mouseX;
    float mouseY;

    float multiplier = 0.01f;

    float xRotation;
    float yRotation;
    public float sensX;
    public float sensY;
    
    [Header("Multipliers")]
    public float movementMultiplier = 10f;
    public float airMultiplier = 5f;
    public float crouchingMultiplier = 5f;

    [Header("Sprinting")]
    float sprintingSpeed = 12f;
    float walkSpeed = 6f;
    bool isSprinting;

    [Header("Jumping & Land Detection")]
    float playerHeight = 2f;
    float jumpForce = 10f;
    float airTime;
    bool isGrounded;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 1f;
    float crouchDrag = 9f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    float standingheight = 2f;
    float crouchingHeight = 1f;
    bool isCrouching;
    float crouchSpeed = 3f;
    bool aboveObstruction;
    RaycastHit obstructionHit;

    [Header("Sliding")]
    float slideForce = 0.5f;
    bool isSliding;


    [Header("Slope Stuff")]
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;

    [Header("Wallrunning")]
    public LayerMask whatIsWall;
    public float wallrunForce, maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;
    
    [Header("Camera")]
    public Camera playerCamera;
    public float camTilt;
    public float camTiltTime;

    [Header("Headbobbing")]
    float walkBobSpeed = 7f;
    float walkBobAmount = 0.1f;
    float defaultYPos = 0;
    float timer;


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
        defaultYPos = cam.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        MyInput();
        ControlDrag();
        ControlSpeed();
        CheckForWall();
        WallRunInput();
        CheckLanding();
        CheckAirTime();
        HeadBob();
        Look();

        float movementPerFrame = Vector3.Distance (PreviousFramePosition, transform.position);
        Speed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);

        aboveObstruction = Physics.Raycast(transform.position, Vector3.up, out obstructionHit, playerHeight * 2f);
        
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if(Input.GetKey(KeyCode.LeftControl))
        {
            Crouch();
        }
        else
        {
            Uncrouch();
        } 

        if(aboveObstruction)
        {
            playerCol.height = 1.5f;
        }
        

        if(Speed >= 10 && isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            StartSlide();
        }
        else
        {
            StopSlide();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

    }

    void Look()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }
    

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void CheckAirTime()
    {
        if(isGrounded)
        {
            airTime = 0f;
        }
        else
        {
            airTime += Time.deltaTime;
        }
    }

    void CheckLanding()
    {
        if(airTime > 0)
        {
            if(isGrounded)
            {
                Debug.Log("landed");
                playerLandAnimation.Play();
                landParticles.Play();
            }
        }
    }

    void Crouch()
    {
        playerCol.height = Mathf.Lerp(playerCol.height, crouchingHeight, Time.deltaTime * crouchSpeed);
        isCrouching = true;
        
    }

    void Uncrouch()
    {
        playerCol.height = Mathf.Lerp(playerCol.height, standingheight, Time.deltaTime * crouchSpeed);
        isCrouching = false;
    }

    void StartSlide()
    {
        playerCol.height = Mathf.Lerp(playerCol.height, crouchingHeight, Time.deltaTime * crouchSpeed);
        rb.AddForce(transform.forward * slideForce, ForceMode.VelocityChange);
        isSliding = true;
    }

    void StopSlide()
    {
        playerCol.height = Mathf.Lerp(playerCol.height, standingheight, Time.deltaTime * crouchSpeed);
        isSliding = false;
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

        if(isGrounded && isCrouching)
        {
            rb.drag = crouchDrag;
        } 

        if(isSliding)
        {
            rb.drag = crouchDrag;
        }
    }

    void ControlSpeed()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintingSpeed, Time.deltaTime);
            isSprinting = true;
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime);
            isSprinting = false;
        }
    }

  
    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * Time.deltaTime * 40);
        MovePlayer();
    }

    void MovePlayer()
    {  
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }  
        else
        {
            rb.AddForce(moveDirection.normalized * inAirSpeed * airMultiplier, ForceMode.Acceleration);
        }  

        if (isGrounded && OnSlope())
        {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
    }

    void HeadBob()
    {
        if(!isGrounded)
        {
            return;
        }

        if(Mathf.Abs(moveDirection.x ) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * walkBobSpeed;
            cam.transform.localPosition = new Vector3
            (
                cam.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * walkBobAmount,
                cam.transform.localPosition.z
            );
        }
    }

    void WallRunInput() //make sure to call in void Update
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();
    }
    void StartWallrun()
    {
        rb.useGravity = false;
        isWallRunning = true;

        if (rb.velocity.magnitude <= maxWallSpeed)
        {
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
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }

    void CheckForWall() //make sure to call in void Update
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.65f, whatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.65f, whatIsWall);

        //leave wall run
        if (!isWallLeft && !isWallRight) 
        {
            StopWallRun();
        }
    }
}
