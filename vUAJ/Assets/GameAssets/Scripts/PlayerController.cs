using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movingSpeed;
    public float jumpForce;
    private float moveInput;

    private bool facingRight = false;
    [HideInInspector]
    public bool deathState = false;

    private bool isGrounded;
    public Transform groundCheck;

    private Rigidbody2D rigidbody;
    private Animator animator;
    private GameManager gameManager;
    private NotificationManager notificationManager;

    private bool _inputIsActive = true;

    void Start()
    {
        notificationManager = NotificationManager.Instance;
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GameIsPaused) return;
        CheckGround();
    }

    public void ActivateInput()
    {
        _inputIsActive = true;
    }

    public void DeactivateInput()
    {
        _inputIsActive = false;
    }

    void Update()
    {
        if (GameManager.Instance.GameIsPaused) return;
        if (_inputIsActive)
        {
            if (Input.GetButton("Horizontal"))
            {
                moveInput = Input.GetAxis("Horizontal");
                Vector3 direction = transform.right * moveInput;
                transform.position = Vector3.MoveTowards(transform.position, transform.position + direction, movingSpeed * Time.deltaTime);
                animator.SetInteger("playerState", 1); // Turn on run animation
            }
            else
            {
                if (isGrounded) animator.SetInteger("playerState", 0); // Turn on idle animation
            }
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                rigidbody.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            }
        }
        
        if (!isGrounded)animator.SetInteger("playerState", 2); // Turn on jump animation

        if(facingRight == false && moveInput > 0)
        {
            Flip();
        }
        else if(facingRight == true && moveInput < 0)
        {
            Flip();
        }
    }

    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void CheckGround()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.transform.position, 0.2f);
        isGrounded = colliders.Length > 1;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            deathState = true; // Say to GameManager that player is dead
        }
        else
        {
            deathState = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Sign")
        {
            for (int i = 0; i < collision.gameObject.transform.childCount; i++)
            {
                collision.gameObject.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Coin")
        {
            gameManager.coinsCounter += 1;
            switch (gameManager.coinsCounter)
            {
                case 1:
                    {
                        notificationManager.SpawnNotification(
                            "¡Has conseguido 1 moneda! Solo te quedan 2, ¡ánimo!",
                            "coin2",
                            new Color(113f / 255f, 194f / 255f, 79f / 255f),
                            "Coin Bag Reward",                          
                            HapticFeedbackType.Heavy      
                        );
                        break;
                    }
                case 2:
                    {
                        notificationManager.SpawnNotification(
                            "¡Otra! Solo te queda 1",
                            "card-diamonds",
                            new Color(105f / 255f, 161f / 255f, 211f / 255f),
                            "Brightest Star 3",                          
                            HapticFeedbackType.Heavy       
                        );
                        break;
                    }
                case 3:
                    {
                        notificationManager.SpawnNotification(
                            "¡Las conseguiste todas! YIPIIII",
                            "badge",
                            new Color(223f / 255f, 96f / 255f, 208f / 255f),
                            "Discovery 1",                           
                            HapticFeedbackType.Heavy        
                        );
                        break;
                    }
            }
            Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "Sign")
        {
           
            for(int i = 0; i < other.gameObject.transform.childCount; i++)
            {
                if(other.gameObject.transform.GetChild(i).gameObject.name != "border") other.gameObject.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        else if (other.gameObject.tag == "Friend")
        {
            if (notificationManager.CurrentLevel == NotificationLevel.Assisted)
            {
                notificationManager.SpawnNotification(
                            "¡Estas delante de una persona, habla con él!",
                            "chat",
                            Color.white,
                            "Unlocked Secret",
                            HapticFeedbackType.Light
                );
            }
        }
    }
}
