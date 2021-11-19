using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    [Header("Assignables")]
    public Transform orientation;
    public Animation playerLandAnimation;
    public ParticleSystem landParticles;
    public ParticleSystem wooshLines;

    [Header("Movement")]
    float moveSpeed = 6f;
    float inAirSpeed = 3f;
    float horizontalMovement;
    float verticalMovement;

    [Header("Speed display")]
    Vector3 PreviousFramePosition = Vector3.zero; // Or whatever your initial position is
    public float speed;

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
    float jumpForce = 15f;
    float groundDistance = 0.4f;
    float airTime;
    public LayerMask groundMask;
    bool isGrounded;
    bool isInAir;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 1f;
    float crouchDrag = 9f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    public Transform playerScale;
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    Vector3 standingScale = new Vector3(1, 1, 1);
    float standingheight = 2f;
    float crouchingHeight = 1f;
    bool isCrouching;
    float crouchSpeed = 9f;
    bool aboveObstruction;
    RaycastHit obstructionHit;

    [Header("Sliding")]
    float slideForce = 0.1f;
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

    [Header("Vaulting")]
    bool isVaultable;
    
    [Header("Camera")]
    public Camera cam;
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
        CheckForWall();
        WallRunInput();
        CheckLanding();
        CheckAirTime();
        HeadBob();
        Look();


        //ground check
        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, 1, 0), groundDistance, groundMask);
        //above obstruction check
        aboveObstruction = Physics.Raycast(transform.position, Vector3.up, out obstructionHit, playerHeight * 2f);

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

        if (isInAir && !isGrounded)
        {
            playerCol.height = 0.4f;
        }
        else if(isGrounded)
        {
            playerCol.height = 2f;
        } 
        

        if(speed > 5 && isGrounded && Input.GetKey(KeyCode.LeftControl))
        {
            StartSlide();
        }
        else
        {
            StopSlide();
        } 
    }

    void FixedUpdate()
    {
        ControlSpeed();
        MovePlayer();

        //player velocity calculator
        float movementPerFrame = Vector3.Distance(PreviousFramePosition, transform.position);
        speed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;

        if (aboveObstruction)
        {
            playerScale.localScale = new Vector3(1, 0.5f, 1);
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
    

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void CheckAirTime()
    { 
        if(isGrounded || OnSlope() || isWallRunning)
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
        if(airTime > 0.5f)
        {
            if(isGrounded)
            {
                Debug.Log("landed");
                playerLandAnimation.Play();
            }
        }

        if(airTime >= 2.5f)
        {
            if(isGrounded)
            {
                Debug.Log("Land particles");
                landParticles.Play();
            }
            
        }
    }


    void Crouch()
    {
        playerScale.localScale = new Vector3(1, 0.5f, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        isCrouching = true;
        
    }

    void Uncrouch()
    {
        playerScale.localScale = new Vector3(1, 1, 1);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        isCrouching = false;
    }

    void StartSlide()
    {
        playerCol.height = Mathf.Lerp(playerCol.height, crouchingHeight, Time.deltaTime * crouchSpeed);
        rb.AddForce(orientation.forward * slideForce, ForceMode.VelocityChange);
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
    }

    void ControlSpeed()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintingSpeed, Time.deltaTime * 2);
            isSprinting = true;
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime * 2);
            isSprinting = false;
        }
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

    void WallRunInput() 
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();


        if (isWallRunning)
        {
            if (isWallRight && Input.GetKey(KeyCode.A))
            {
                rb.AddForce(transform.up * jumpForce * 3); 
                rb.AddForce(-orientation.right * jumpForce * 3.2f);
            }
            if (isWallLeft && Input.GetKey(KeyCode.D))
            {
                rb.AddForce(transform.up * jumpForce * 3);
                rb.AddForce(orientation.right * jumpForce * 3.2f);
            }
        }
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
}
