using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using EZCameraShake;

public class PlayerMove : MonoBehaviour
{
    //assignables
    public Transform orientation;
    public Transform cam;

    //movement
    float moveSpeed = 6f;
    float inAirSpeed = 3f;
    float horizontalMovement;
    float verticalMovement;

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
    
    //multipliers
    public float movementMultiplier = 10f;
    public float airMultiplier = 5f;
    public float crouchingMultiplier = 5f;

    //sprinting
    float sprintingSpeed = 12f;
    float walkSpeed = 6f;

    //jumping
    float playerHeight = 2f;
    float jumpForce = 10f;
    bool isGrounded;

    //Drag
    float groundDrag = 6f;
    float airDrag = 1f;
    float crouchDrag = 9f;

    //crouch
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    Vector3 standingScale = new Vector3(1, 1, 1);
    public Transform playerScale;
    bool isCrouching;

    //slope stuff
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;

    //Wallrunning
    public LayerMask whatIsWall;
    public float wallrunForce, maxWallrunTime, maxWallSpeed;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    public float maxWallRunCameraTilt, wallRunCameraTilt;

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
        ControlSpeed();
        CheckForWall();
        WallRunInput();
        Look();

        float movementPerFrame = Vector3.Distance (PreviousFramePosition, transform.position);
        Speed = movementPerFrame / Time.deltaTime;
        PreviousFramePosition = transform.position;

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
        
        if(Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Jump();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            Crouch();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Uncrouch();
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

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRunCameraTilt);
        transform.rotation = Quaternion.Euler(0, yRotation, 0);

        //While Wallrunning
        //Tilts camera in .5 second
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallRight)
        {
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 5;
        }
            
        if (Math.Abs(wallRunCameraTilt) < maxWallRunCameraTilt && isWallRunning && isWallLeft)
        {
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 5;
        }    

        //Tilts camera back again
        if (wallRunCameraTilt > 0 && !isWallRight && !isWallLeft)
        {
            wallRunCameraTilt -= Time.deltaTime * maxWallRunCameraTilt * 5;
        }

        if (wallRunCameraTilt < 0 && !isWallRight && !isWallLeft)
        {
            wallRunCameraTilt += Time.deltaTime * maxWallRunCameraTilt * 5;
        }

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

    void Crouch()
    {
       // playerScale.localScale = crouchScale;
        playerScale.localScale = Vector3.MoveTowards(standingScale, crouchScale, Time.deltaTime * 7);
        isCrouching = true;
        
    }

    void Uncrouch()
    {
        //playerScale.localScale = standingScale;
        playerScale.localScale = Vector3.MoveTowards(crouchScale, standingScale, Time.deltaTime * 7);
        isCrouching = false;
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
            moveSpeed = Mathf.Lerp(moveSpeed, sprintingSpeed, Time.deltaTime);
        }
        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, Time.deltaTime);
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

   private void WallRunInput() //make sure to call in void Update
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft) StartWallrun();
    }
    private void StartWallrun()
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
            } 
            else
            {
                rb.AddForce(-orientation.right * wallrunForce / 5 * Time.deltaTime);
            }         
        }
    }
    private void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
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
