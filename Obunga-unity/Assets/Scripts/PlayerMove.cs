using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [Header("Assignables")]
    public Transform orientation;
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
    float airTime;
    public LayerMask groundMask;
    bool isGrounded;
    bool canJump;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 1f;
    float crouchDrag = 9f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    public Transform playerScale;
    bool isCrouching;
    bool canUncrouch;
    bool canCrouch;
    bool aboveObstruction;
    RaycastHit obstructionHit;

    [Header("Sliding & Diving")]
    float slideForce = 25f;
    bool isSliding;

    [Header("Step Handling")]
    public Transform lowerRay;
    public Transform upperRay;
    public Transform downRay;
    public float stepSpeed;
    public float landPos;

    public float HoverSpringStrength;
    public float HoverSpringDamper;
    public float HoverHeight;


    [Header("Slope Stuff")]
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;

    [Header("Wallrunning")]
    public float wallrunForce;
    public float wallRunGravity;
    bool isWallRight, isWallLeft;
    bool isWallRunning;
    
    [Header("Camera")]
    public Camera cam;
    public float camTilt;
    public float WallRunCamTiltTime;
    public float camTiltTime;


    [Header("Footsteps")]
    public AudioSource footstepAudioSource;
    public AudioClip[] footstepClips;
    public AudioClip[] landingClips;
    public AudioClip slideClip;
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
        Debug.DrawRay(lowerRay.position, Vector3.forward, Color.green);
        Debug.DrawRay(upperRay.position, Vector3.forward, Color.red);
        Debug.DrawRay(downRay.position, Vector3.down, Color.yellow);
        MyInput();
        ControlDrag();
        CheckForWall();
        WallRunInput();
        CheckLanding();
        CheckAirTime();
        CameraTilting();
        Look(); 
        HandleFootsteps();
        
        Debug.DrawRay(transform.position, Vector3.up, Color.green);

        //above obstruction check
        aboveObstruction = Physics.Raycast(transform.position, Vector3.up, out obstructionHit, playerHeight * 3f);

        //slope stuff
        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        //ground check
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.15f, groundMask);


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

        if(isWallRunning)
        {
            canCrouch = false;
            canUncrouch = false;
            canJump = false;
        }
        else
        {
            canCrouch = true;
            canUncrouch = true;
            canJump = true;
        }
    }
    void FixedUpdate()
    {
        MovePlayer();
        HandleSteps();
    }


    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
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

            if (isSliding)
            {
                rb.drag = groundDrag;
            }

            if (isCrouching)
            {
                rb.drag = crouchDrag;
            }
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
        }
    }

    void CheckLanding()
    {
        if (rb.velocity.magnitude >= 1 && airTime >= 1f)
        {
            if(isGrounded)
            {
                GameObject Particles = Instantiate(landParticles, new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), Quaternion.Euler(90, 0, 0));
                Destroy(Particles, 2f);
            }
            
        }

        if (rb.velocity.magnitude >= 0.5f && airTime >= 0.25f)
        {
            if (isGrounded) footstepAudioSource.PlayOneShot(landingClips[Random.Range(0, landingClips.Length - 1)]);
        }
    }

    void Jump()
    {
        if(canJump)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }  
    }

    void Crouch()
    {
       if(canCrouch)
        {
            playerScale.localScale = new Vector3(1, 0.5f, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
            isCrouching = true;
            isSliding = false;

            if (rb.velocity.magnitude > 6f && isGrounded)
            {
                rb.AddForce(moveDirection * slideForce, ForceMode.VelocityChange);
                footstepAudioSource.PlayOneShot(slideClip);
                isCrouching = false;
                isSliding = true;
            }
        }
    }

    void Uncrouch()
    {
       if(canUncrouch)
        {
            playerScale.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.45f, transform.position.z);
            isCrouching = false;
            isSliding = false;
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

    void WallRunInput() 
    {
        //Wallrun
        if (Input.GetKey(KeyCode.D) && isWallRight && !isGrounded) StartWallrun();
        if (Input.GetKey(KeyCode.A) && isWallLeft && !isGrounded) StartWallrun();


        if (isWallRunning)
        {
            if (isWallRight && Input.GetKey(KeyCode.A))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
                rb.AddForce(transform.up * jumpForce * 3f); 
                rb.AddForce(-orientation.right * jumpForce * 2.5f);
            }
            if (isWallLeft && Input.GetKey(KeyCode.D))
            {
                rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.y);
                rb.AddForce(transform.up * jumpForce * 3f, ForceMode.Force);
                rb.AddForce(orientation.right * jumpForce * 2.5f, ForceMode.Force);
            }
        }
    }

    void StartWallrun()
    {
        rb.useGravity = false;
        isWallRunning = true;

        //slowly slide down
        rb.AddForce(Vector3.down * wallRunGravity, ForceMode.Force);

    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
    }

    void CheckForWall() 
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.7f);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.7f);

        //leave wall run
        if (!isWallLeft && !isWallRight) 
        {
            StopWallRun();
        }
    }

    void HandleFootsteps()
    {
        if (!isGrounded) return;

        if (rb.velocity.magnitude <= 0) return;

        if (isSliding) return;

        footstepTimer -= Time.deltaTime;

        if (rb.velocity.magnitude >= 6 && footstepTimer <= 0 && isGrounded)
        {
            footstepAudioSource.PlayOneShot(footstepClips[Random.Range(0, footstepClips.Length - 1)]);

            footstepTimer = GetCurrentOffset;
        }
    }

    void CameraTilting()
    {
        //wallrun camera tilt
        if (isWallRight && !isGrounded) tilt = Mathf.Lerp(tilt, camTilt, WallRunCamTiltTime * Time.deltaTime);
        if (isWallLeft && !isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, WallRunCamTiltTime * Time.deltaTime);
        if (!isWallRunning) tilt = Mathf.Lerp(tilt, 0, WallRunCamTiltTime * Time.deltaTime);

        //sliding
        if (isSliding) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime / 2);

        if (Input.GetKey(KeyCode.A) && isGrounded) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime / 2);

        if (Input.GetKey(KeyCode.D) && isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime / 2);

        tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
    }

    void HandleSteps()
    {
        /* RaycastHit LowerHit;
         RaycastHit UpperHit;
         RaycastHit DownHit;

         if (Physics.Raycast(lowerRay.position, Vector3.forward, out LowerHit, 0.1f))
         {

             if(!Physics.Raycast(upperRay.position, Vector3.forward, out UpperHit, 0.2f))
             {
                 if(Physics.Raycast(downRay.position, Vector3.down, out DownHit, 1f))
                 {
                     Vector3 landPos = DownHit.point + (Vector3.up * playerHeight);
                     transform.position = landPos;
                 }
             }
         } */

        Ray ray = new Ray()
        {
            origin = transform.position,
            direction = Vector3.down
        };

        LayerMask mask = ~LayerMask.GetMask("Player");
        if (Physics.Raycast(ray, out RaycastHit hit, HoverHeight, mask))
        {
            isGrounded = true;

            Debug.DrawLine(transform.position, hit.point, Color.yellow);
            float relVel = Vector3.Dot(ray.direction, rb.velocity);

            float x = hit.distance - HoverHeight;
            float springForce = (x * HoverSpringStrength) - (relVel * HoverSpringDamper);
            rb.AddForce(ray.direction * springForce);
        }
    }
}
