using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // movement
    float moveSpeed = 6f;
<<<<<<< HEAD
    float crouchSpeed = 2f;
    
    float jumpForce = 5f;
=======
    float jumpForce = 9f;
>>>>>>> parent of 47b7cc1 (what.)
    public float movementMultiplier = 10f;
    float airMultiplier = 0.4f;

    float horizontalMovement;
    float verticalMovement;

    //jumping???!?!?!
    float playerHeight = 2f;
    bool isGrounded;

    //Drag
    float groundDrag = 6f;
    float airDrag = 2f;

<<<<<<< HEAD
    //crouch
    //Vector3 crouchScale = new Vector3(1, 0.5f, 1);
   // Vector3 playerScale;
    float wantedheight;
=======
>>>>>>> parent of 47b7cc1 (what.)
    Vector3 moveDirection;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
<<<<<<< HEAD
       // playerScale =  transform.localScale;
        playercol = GetComponent<CapsuleCollider>();
=======
>>>>>>> parent of 47b7cc1 (what.)
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
<<<<<<< HEAD

        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            Crouch();
        }

        if(Input.GetKeyUp(KeyCode.LeftControl))
        {
            Uncrouch();
        }

        
=======
>>>>>>> parent of 47b7cc1 (what.)
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

<<<<<<< HEAD
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

=======
>>>>>>> parent of 47b7cc1 (what.)
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
<<<<<<< HEAD

        if(isCrouching)
        {
            rb.drag = crouchDrag;
        }
=======
        
>>>>>>> parent of 47b7cc1 (what.)
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
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * airMultiplier, ForceMode.Acceleration);
        }
        
    }
}