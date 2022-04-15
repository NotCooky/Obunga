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
    
    [Header("Multipliers")]
    public float movementMultiplier = 12.5f;
    public float airMultiplier = 13f;
    public float crouchingMultiplier = 7.5f;

    [Header("Jumping & Land Detection")]
    public float jumpForce = 10f;
    float playerHeight = 2f;
    float airTime;
    bool isGrounded;
    [Header("Drag")]
    float groundDrag = 10f;
    float airDrag = 1f;
    float crouchDrag = 12f;

    [Header("Crouching")]
    public CapsuleCollider playerCol;
    bool underObstruction;
    bool isCrouching;

    [Header("Slope Stuff")]
    Vector3 moveDirection;
    Vector3 slopeMoveDirection;
    RaycastHit slopeHit;
    Rigidbody rb;
    public float playerMaxSlopeAngle;
    public float slopeForce;

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

    float extraGravityForce = -350f;

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

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(playerCol.transform.position - new Vector3(0, 0.7f, 0), 0.4f);
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

        isGrounded = Physics.CheckSphere(playerCol.transform.position - new Vector3(0, 0.7f, 0), 0.4f, ~playerLayerMask);

        underObstruction = Physics.Raycast(transform.position, Vector3.up, playerHeight / 2 + 0.15f);

        if(isGrounded && !isCrouching)
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 2f, 0.1f);
            playerCol.center = Vector3.Lerp(playerCol.center, Vector3.zero, 0.1f);
        } 
        else
        {
            playerCol.height = Mathf.Lerp(playerCol.height, 1f, 0.1f);
            playerCol.center = Vector3.Lerp(playerCol.center, new Vector3(0, 0.5f, 0), 0.1f);
        }     
    }
    void FixedUpdate()
    {
        MovePlayer();
        CameraTilting();

        rb.AddForce(Vector3.up * extraGravityForce, ForceMode.Force);
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
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            if(OnSlope())
            {
                rb.AddForce(slopeMoveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
                Vector3 gravityForce = Physics.gravity - Vector3.Project(Physics.gravity, slopeHit.normal);

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
        if(isGrounded  || OnSlope() || isWallRunning || isCrouching)
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
        if (airTime >= 0.3f)
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
        isWallRight = Physics.Raycast(transform.position, orientation.right, 0.7f, ~playerLayerMask);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 0.7f, ~playerLayerMask);

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
                rb.AddForce(transform.up * jumpForce + -orientation.right * jumpForce / 2, ForceMode.Impulse);
            }

            if (isWallLeft && Input.GetKeyDown(KeyCode.Space))
            {
                rb.AddForce(transform.up * jumpForce + orientation.right * jumpForce / 2, ForceMode.Impulse);
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
        //this stinks like shit....
        if (isWallRunning)
        {
            //wallrun camera tilt
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