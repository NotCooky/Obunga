using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Assignables")]
    public Transform orientation;
    public GameObject landParticles;

    [Header("Movement")]
    public float moveSpeed = 10f;
    public float inAirSpeed = 6f;
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
    public float movementMultiplier = 12.5f;
    public float airMultiplier = 13f;
    public float crouchingMultiplier = 7.5f;

    [Header("Jumping & Land Detection")]
    public float jumpForce = 10f;
    float playerHeight = 2f;
    float airTime;
    bool isGrounded;
    public LayerMask groundMask;

    [Header("Drag")]
    float groundDrag = 6f;
    float airDrag = 1f;
    float crouchDrag = 8f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    public Transform playerScale;
    bool isCrouching;

    [Header("Sliding & Diving")]
    float slideForce = 25f;
    bool isSliding;

    [Header("Vaulting")]
    public GameObject RayLower;
    public GameObject RayUpper;
    public float vaultStrength;

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
        CheckLanding();
        CheckAirTime();
        HandleFootsteps();
        Look(); 

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);

        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.15f);

        if(!isGrounded)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 1f, 0.1f);
        }
        else if(isGrounded && !isCrouching)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 2f, 0.1f);
        } 
    }
    void FixedUpdate()
    {
        MovePlayer();
        CameraTilting();
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;

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
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            if(OnSlope())
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            }
            else
            { 
                rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            } 
        }
        else
        {
            rb.AddForce(moveDirection.normalized * inAirSpeed * airMultiplier, ForceMode.Acceleration);
        }
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); 
    }

    void Crouch()
    { 
       isCrouching = true;
       playerCol.height = 1f;
       if(isGrounded)
       {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        }

        if (rb.velocity.magnitude > 6f && isGrounded)
       {
            rb.AddForce(moveDirection * slideForce, ForceMode.VelocityChange);
            footstepAudioSource.PlayOneShot(slideClip);
            isSliding = true;
       }
    }

    void Uncrouch()
    {
        playerCol.height = 2f;
        if (isGrounded)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        }
        isCrouching = false;
        isSliding = false; 
    }

    void Look()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        camHolder.transform.rotation = Quaternion.Euler(xRotation, yRotation, tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    } 

    void ControlDrag()
    {
        if(isGrounded)
        {
            rb.drag = groundDrag;

            if (isCrouching)
            {
                rb.drag = crouchDrag;
            }
        }
        else if(!isGrounded)
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
        if(isGrounded  || OnSlope() || isWallRunning || isCrouching || isSliding)
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
        if (rb.velocity.magnitude >= 1 && airTime >= 0.5f)
        {
            if(isGrounded || isGrounded && OnSlope())
            {
                GameObject Particles = Instantiate(landParticles, new Vector3(transform.position.x, transform.position.y - 0.7f, transform.position.z), Quaternion.Euler(90, 0, 0));
                Destroy(Particles, 2f);
                footstepTimer = GetCurrentOffset;
            }
            
        }

        if (rb.velocity.magnitude >= 0.5f && airTime >= 0.25f)
        {
            if (isGrounded || isGrounded && OnSlope()) footstepAudioSource.PlayOneShot(landingClips[Random.Range(0, landingClips.Length - 1)]);
        }
    }

    void CheckForWall() 
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.7f);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.7f);

        if (isWallRight && !isGrounded) Wallrun();
        if (isWallLeft && !isGrounded) Wallrun();

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
                rb.AddForce(transform.up * jumpForce + -orientation.right * jumpForce * 0.8f, ForceMode.Impulse);
            }

            if (isWallLeft && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce + orientation.right * jumpForce * 0.8f, ForceMode.Impulse);
            }
    }

    void StopWallRun()
    {
        isWallRunning = false;
        rb.useGravity = true;
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
        //this stinks like shit....

        if(isSliding) tilt = Mathf.Lerp(tilt, camTilt + 5, camTiltTime * Time.deltaTime / 2);

        if (Input.GetKey(KeyCode.A) && isGrounded) tilt = Mathf.Lerp(tilt, camTilt, camTiltTime * Time.deltaTime / 2);
        else tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);
        if (Input.GetKey(KeyCode.D) && isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, camTiltTime * Time.deltaTime / 2);
        else tilt = Mathf.Lerp(tilt, 0, camTiltTime * Time.deltaTime);

        if (isWallRunning)
        {
            //wallrun camera tilt
            if (isWallRight && !isGrounded) tilt = Mathf.Lerp(tilt, camTilt, WallRunCamTiltTime * Time.deltaTime);
            if (isWallLeft && !isGrounded) tilt = Mathf.Lerp(tilt, -camTilt, WallRunCamTiltTime * Time.deltaTime);
        }
        else return;
    }

}

/*  void Vault()
  {
      Debug.DrawRay(RayLower.transform.position, cam.transform.forward, Color.green);
      Debug.DrawRay(RayUpper.transform.position, cam.transform.forward, Color.red);
      Debug.DrawRay(RayUpper.transform.position + new Vector3(0, 0, 0.4f), Vector3.down, Color.blue);

      if (!isGrounded && !isWallRunning)
      {
          RaycastHit hitLower;
          RaycastHit hitUpper;
          RaycastHit hitDown; // to find the landing pos

          if (Physics.Raycast(RayLower.transform.position, cam.transform.forward, out hitLower, 0.125f))
          {

              if (!Physics.Raycast(RayUpper.transform.position, cam.transform.forward, out hitUpper, 0.125f) && Physics.Raycast(RayUpper.transform.position + new Vector3(0, 0, 0.4f), Vector3.down, out hitDown, 1.5f))
              {
                  if(transform.position.y <= hitDown.point.y)
                  {
                      transform.position = Vector3.Lerp(transform.position, hitDown.point, rb.velocity.magnitude * 0.25f);
                  }

              }
          }
      }
  }   */

//tato stinks like shit....
