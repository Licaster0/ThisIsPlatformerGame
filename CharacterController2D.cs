using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class CharacterController2D : MonoBehaviour
{
    // === [FIELDS & COMPONENT REFERENCES] ===
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    /*
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck; // Ayakların olduğu pozisyon
    [SerializeField] private float groundCheckRadius = 0.2f; // Çemberin yarıçapı
    [SerializeField] private LayerMask groundLayer; // Zemin katmanı
    */
    [Header("Animator & Particles")]
    [SerializeField] public Animator animator;
    [SerializeField] public ParticleSystem dustParticle;
    [SerializeField] public ParticleSystem jumpParticle;
    [SerializeField] public ParticleSystem flipEffect;

    [Header("Effects & Prefabs")]
    [SerializeField] private GameObject deathEffectPrefab;
    [SerializeField] private GameObject restartEffectPrefab;
    [SerializeField] private GameObject collectedEffectPrefab;

    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI sign1Text;
    [SerializeField] private GameObject dialogBox;

    [Header("Gameplay Variables")]
    [SerializeField] private int collectedFruit = 0;
    private int totalFruits;




    // === [PRIVATE FIELDS] ===
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Collider2D characterCollider;
    private Vector3 originalPosition;

    // === [STATE MACHINE] ===
    private State currentState;
    public IdleState IdleState { get; private set; }
    public RunState RunState { get; private set; }
    public JumpState JumpState { get; private set; }
    public DoubleJumpState DoubleJumpState { get; private set; }

    // === [PROPERTIES] ===
    public bool IsTouchingWall { get; private set; }

    public float GetVerticalVelocity()
    {
        return rb.velocity.y; // Rigidbody2D'nin dikey h�z�n� d�nd�r�r
    }
    // === [UNITY METHODS] ===
    private void Start()
    {
        // Component references
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        characterCollider = GetComponent<Collider2D>();

        // Initialize states
        IdleState = new IdleState(this);
        RunState = new RunState(this);
        JumpState = new JumpState(this);
        DoubleJumpState = new DoubleJumpState(this);

        // Cache initial position
        originalPosition = transform.position;

        // Calculate total fruits
        totalFruits = GameObject.FindGameObjectsWithTag("Fruits").Length;

        // Set initial state
        TransitionToState(IdleState);
    }

    private void Update()
    {
        // Update current state
        currentState.UpdateState();

        // Handle dust particle animation
        HandleDustEffect();
    }

    private void FixedUpdate()
    {
        HandleSpriteFlip();
        UpdateAnimatorIdleState();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("finish"))
        {
            HandleFinishTrigger();
        }
        else if (collision.CompareTag("Traps"))
        {
            StartCoroutine(HandleDeath());
        }
        else if (collision.name == "Sign1")
        {
            HandleSignTriggerEnter();
        }
        else if (collision.CompareTag("Fruits"))
        {
            HandleFruitCollection(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.name == "Sign1")
        {
            HandleSignTriggerExit();
        }
    }

    // === [MOVEMENT METHODS] ===
    public void Move(float direction)
    {
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    public void Jump(float forceMultiplier = 1f)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce * forceMultiplier);
    }

    public bool IsGrounded()
    {
        return Mathf.Approximately(rb.velocity.y, 0f);
    }

    // === [TRIGGER HANDLERS] ===
    private void HandleFinishTrigger()
    {
        if (collectedFruit == totalFruits)
        {
            LoadNextScene();
        }
        else
        {
            Debug.Log("Daha fazla meyve toplamalısın!");
        }
    }

    private void HandleSignTriggerEnter()
    {
        dialogBox.SetActive(true);
        sign1Text.gameObject.SetActive(true);
    }

    private void HandleSignTriggerExit()
    {
        dialogBox.SetActive(false);
        sign1Text.gameObject.SetActive(false);
    }

    private void HandleFruitCollection(Collider2D collision)
    {
        collectedFruit++;
        Instantiate(collectedEffectPrefab, collision.transform.position, Quaternion.identity);
        Destroy(collision.gameObject);
    }

    // === [DEATH & RESPAWN METHODS] ===
    private IEnumerator HandleDeath()
    {
        PlayDeathEffect();

        // Disable sprite renderer
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(1f);

        // Respawn
        Respawn();
    }

    private void PlayDeathEffect()
    {
        Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        StartCoroutine(ScreenShake(0.5f, 0.1f));
    }

    private void Respawn()
    {
        transform.position = originalPosition;

        spriteRenderer.enabled = true;
        PlayRestartEffect();

        Debug.Log("Player has respawned!");
    }

    private void PlayRestartEffect()
    {
        Instantiate(restartEffectPrefab, transform.position, Quaternion.identity);
    }

    // === [CAMERA EFFECTS] ===
    public IEnumerator ScreenShake(float duration, float magnitude)
    {
        Vector3 originalCamPos = Camera.main.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            Camera.main.transform.localPosition = new Vector3(x, y, originalCamPos.z);
            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalCamPos;
    }

    // === [UTILITY METHODS] ===
    public void TransitionToState(State newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState.EnterState();
    }

    private void LoadNextScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(currentSceneIndex + 1);
        }
        else
        {
            Debug.Log("Son sahneye ulaşıldı.");
        }
    }

    private void HandleDustEffect()
    {
        bool isRunning = animator.GetBool("isRunning");

        if (isRunning && !dustParticle.isPlaying)
        {
            dustParticle.Play();
        }
        else if (!isRunning && dustParticle.isPlaying)
        {
            dustParticle.Stop();
        }
    }

    private void HandleSpriteFlip()
    {
        if (rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }

    private void UpdateAnimatorIdleState()
    {
        animator.SetBool("isIdle", Mathf.Abs(rb.velocity.x) < 0.1f && Mathf.Abs(rb.velocity.y) < 0.1f);
    }
}
