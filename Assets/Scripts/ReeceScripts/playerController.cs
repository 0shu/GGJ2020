using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ2020
{
    public class playerController : MonoBehaviour
    {
        //Walk speed variable
        public float walkSpeed;
        public float jumpForce;

        public float playerhealth;

        //Rigid body
        Rigidbody rb;
        Vector3 moveDirection;

        public Transform groundCheck;
        public float groundDistance = 0.04f;
        public LayerMask groundMask;
        bool isGrounded;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            float horizontalMovement = Input.GetAxisRaw("Horizontal");
            float verticalMovement = Input.GetAxisRaw("Vertical");

            moveDirection = (horizontalMovement * transform.right + verticalMovement * transform.forward).normalized;

            if (Input.GetKeyDown("space"))
            {
                if (isGrounded && verticalMovement < 0)
                {
                    rb.AddForce(transform.up * jumpForce);
                }
            }
        }

        void FixedUpdate()
        {
            Move();
        }

        void Move()
        {
            Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
            rb.velocity = moveDirection * walkSpeed * Time.deltaTime;
            rb.velocity += yVelFix;
        }

        public void takeDamage(float Damage)
        {
            Debug.Log("PH-" + playerhealth + " Damage-" + Damage);

            playerhealth = Mathf.Max(0.0f, playerhealth - Damage);
            if (playerhealth - Damage == 0)
            {
                SceneManager.LoadScene("CREDITS");
                Destroy(this.gameObject);
            }

        }
    }
}
