using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float moveSpeed = 4f;
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float dashSpeed = 8f;

    private int count = 0;

    private float initialScaleX;
    private bool isOnGround;

    private Rigidbody2D rb;
    private Animator animator;
    
    private float horizontalInput;
    private bool dashInput = false;
    private bool damageTaken = false;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Guardamos la escala que le pusiste en el Inspector al empezar
        initialScaleX = transform.localScale.x;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Se activa con el componente Player Input (Action: Move)
    public void OnMove(InputValue value)
    {
        // En plataformas solo nos interesa el eje X (A/D)
        horizontalInput = value.Get<Vector2>().x;

        // Esto imprimirá los valores en la consola cada vez que presiones WASD
        Debug.Log("Movimiento detectado: " + horizontalInput);
    }

    // Accion de Dash
    public void OnSprint()
    {
        if (isOnGround)
        {
            dashInput = true;
            animator.SetBool("Dash", true);
            Invoke("Tick", 0.5f);
        }
    }

    // Accion de ataque
    public void OnAttack()
    {
        count += 1;
        animator.SetBool("Attacking", true);
        Debug.Log("Atacando");
        if (count > 1)
        {
            Delay();
        }
        else
        {
            Invoke("Tick", 0.6f);
        }
    }

    // Accion de salto
    void OnJump()
    {
        if (isOnGround)
        {
            animator.SetBool("Ground", false);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Revisa las colisiones del personaje con el juego o con enemigos
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = true;
            animator.SetBool("Ground", true);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Daño recibido");
            if (!isOnGround)
            {
                rb.AddForce(Vector2.up * 3, ForceMode2D.Impulse);
            }
            else
            {
                rb.AddForce(Vector2.up * 6, ForceMode2D.Impulse);
            }
            damageTaken = true;
            Invoke("Tick", 0.5f);
        }
    }

    // Revisa si el personaje ya no esta en suelo
    void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }

    // Delay creado para los ataques
    public void Delay()
    {
        Debug.Log("Segundo Ataque");
        animator.SetBool("FollowUp", true);
        Invoke("Tick", 0.6f);
    }

    // Delay creado para el dash y daño
    public void Tick()
    {
        animator.SetBool("Attacking", false);
        animator.SetBool("FollowUp", false);
        damageTaken = false;
        if (!isOnGround && !damageTaken)
        {
            animator.SetBool("Dash", false);
            Invoke("Tick", 0.1f);
        }
        else if(!isOnGround && damageTaken)
        {
            dashInput = false;
            animator.SetBool("Dash", false);
        }
        else if (isOnGround)
        {
            dashInput = false;
            animator.SetBool("Dash", false);
        }
        count = 0;
    }

    void FixedUpdate()
    {
        // Movimiento horizontal manteniendo la velocidad vertical (gravedad/salto)
        if (!damageTaken)
        {
            rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, rb.linearVelocity.y);
        }

        // Actualizar Animación (usamos Mathf.Abs para que sea siempre positivo)
        animator.SetFloat("Speed", Mathf.Abs(horizontalInput));

        // Girar el Sprite según la dirección
        if (!damageTaken && horizontalInput > 0)
            transform.localScale = new Vector3(initialScaleX, transform.localScale.y, transform.localScale.z); // Mira a la derecha
        else if (!damageTaken && horizontalInput < 0)
            transform.localScale = new Vector3(-initialScaleX, transform.localScale.y, transform.localScale.z);  // Mira a la izquierda

        // Movimiento de la mecanica de Dash
        if (!damageTaken && dashInput && horizontalInput != 0)
        {
            rb.linearVelocity = new Vector2(horizontalInput * dashSpeed, rb.linearVelocity.y);
        }
        else if (!damageTaken && dashInput && transform.localScale.x > 0)
        {
            rb.linearVelocity = new Vector2(1 * dashSpeed, rb.linearVelocity.y);
        }
        else if (!damageTaken && dashInput && transform.localScale.x < 0)
        {
            rb.linearVelocity = new Vector2(-1 * dashSpeed, rb.linearVelocity.y);
        }
        // Al tomar daño restringe el movimiento del jugador
        else if (damageTaken && dashInput)
        {
            rb.linearVelocity = new Vector2(0,  rb.linearVelocity.y);
            dashInput = false;
        }

        //Check de daño
        if (damageTaken && transform.localScale.x > 0)
        {
            rb.linearVelocity = new Vector2(-2, rb.linearVelocity.y);
            transform.localScale = new Vector3(initialScaleX, transform.localScale.y, transform.localScale.z);
        }
        else if (damageTaken && transform.localScale.x < 0)
        {
            rb.linearVelocity = new Vector2(2, rb.linearVelocity.y);
            transform.localScale = new Vector3(-initialScaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
