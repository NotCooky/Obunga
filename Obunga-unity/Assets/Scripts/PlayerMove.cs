using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // movement
    float moveSpeed = 6f;
    float crouchSpeed = 2f;
    
    float jumpForce = 5f;
    public float movementMultiplier = 10f;

    float horizontalMovement;
    float verticalMovement;

    //jumping???!?!?!
    float playerHeight = 2f;
    bool isGrounded;

    //Drag
    float groundDrag = 6f;
    float airDrag = 0f;
    float crouchDrag = 10f;

    //crouch
    //Vector3 crouchScale = new Vector3(1, 0.5f, 1);
   // Vector3 playerScale;
    float wantedheight;
    Vector3 moveDirection;
    bool isCrouching;

    Rigidbody rb;
    CapsuleCollider playercol;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
       // playerScale =  transform.localScale;
        playercol = GetComponent<CapsuleCollider>();
    }

    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight / 2 + 0.1f);
        MyInput();
        ControlDrag();

        

        playercol.height = Mathf.Lerp(playercol.height, wantedheight, Time.deltaTime * crouchSpeed);

        if(Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
        {
            Jump();
        }

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            Uncrouch();
        }

        
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = transform.forward * verticalMovement + transform.right * horizontalMovement;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Crouch()
    {
      // transform.localScale = crouchScale;
      // transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
       wantedheight = 1f;
       isCrouching = true;
        
    }

    void Uncrouch()
    {
       // transform.localScale = playerScale;
       // transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        wantedheight = 2f;
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

        if(isCrouching)
        {
            rb.drag = crouchDrag;
        }
    }

  
    void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        if (isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }     
    }
}