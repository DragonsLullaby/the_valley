using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject gameManager;

    public CharacterController controller;
    public Transform groundCheck;

    float groundDistance = 0.4f;
    public LayerMask groundMask;

    public float movementSpeed;
    public float jumpHeight;
    public float gravity;

    Vector3 velocity;
    bool isGrounded;

    float x;
    float z;

    void Update()
    {
        if (gameManager.GetComponent<GameManager>().gamePaused == false)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
                velocity.y = -2f;

            if (gameManager.GetComponent<GameManager>().inventoryOpen == false)
            {
                x = Input.GetAxis("Horizontal");
                z = Input.GetAxis("Vertical");
                
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
            else
            {
                x = 0f;
                z = 0f;
            }

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * movementSpeed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);
        }
    }
}
