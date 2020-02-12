using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ2020
{
    public class playerController : MonoBehaviour
    {
        //Walk speed variable
        public float walkSpeed = 450f;
        public float runSpeed = 850f;
        public float jumpForce = 300f;

        public float playerhealth = 100f;
        public float maxHealth = 100f;

        public float playerStamina = 10f;
        public float maxStamina = 10f;
        public float minStamina = 2f;

        public float normalFOV = 60f;
        public float fastFOV = 80f;
        public float FOVZoomSpeed = 20f;
        public bool isLerping = false;

        public bool isRunning = false;

        //Rigid body
        Rigidbody rb;
        Vector3 moveDirection;

        public Transform groundCheck;
        public float groundDistance = 0.04f;
        public LayerMask groundMask;
        public StatusBars statusBars;
        public Camera vision;
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
                //if (isGrounded && verticalMovement < 0)
                //{
                    rb.AddForce(transform.up * jumpForce);
                //}
            }

            if(!isRunning)
            {
                playerStamina = Mathf.Clamp(playerStamina + Time.deltaTime, 0, maxStamina);
                if(Input.GetKeyDown("left shift") && playerStamina > minStamina) 
                {
                    isRunning = true;
                    //vision.fieldOfView += FOVAdjust;
                    if(!isLerping) StartCoroutine(LerpFOV());
                }
            }
            else 
            {
                playerStamina = Mathf.Clamp(playerStamina - Time.deltaTime, 0, maxStamina);
                if(Input.GetKeyUp("left shift") || playerStamina <= 0) 
                {
                    isRunning = false;
                    //vision.fieldOfView -= FOVAdjust;
                    if(!isLerping) StartCoroutine(LerpFOV());
                }
            }

            statusBars.SetStamina(playerStamina / maxStamina);
        }

        void FixedUpdate()
        {
            Move();
        }

        IEnumerator LerpFOV()
        {
            isLerping = true;
            while(vision.fieldOfView >= normalFOV && vision.fieldOfView <= fastFOV)
            {
                if(isRunning) vision.fieldOfView += Time.deltaTime * FOVZoomSpeed;
                else vision.fieldOfView -= Time.deltaTime * FOVZoomSpeed;

                yield return null;
            }
            if(isRunning) vision.fieldOfView = fastFOV;
            else vision.fieldOfView = normalFOV;
            isLerping = false;
        }

        void Move()
        {
            Vector3 yVelFix = new Vector3(0, rb.velocity.y, 0);
            if(isRunning) rb.velocity = moveDirection * runSpeed * Time.deltaTime;
            else rb.velocity = moveDirection * walkSpeed * Time.deltaTime;
            rb.velocity += yVelFix;
        }

        public void takeDamage(float Damage)
        {
            Debug.Log("PH-" + playerhealth + " Damage-" + Damage);

            AudioManager.s_instance.PlayHurt();
            playerhealth = Mathf.Max(0.0f, playerhealth - Damage);

            statusBars.SetHealth(playerhealth / maxHealth);

            if (playerhealth - Damage == 0)
            {
                SceneManager.LoadScene("CREDITS");
                Destroy(this.gameObject);
            }
        }
    }
}
