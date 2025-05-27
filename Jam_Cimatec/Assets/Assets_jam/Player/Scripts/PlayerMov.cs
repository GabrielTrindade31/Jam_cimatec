using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    private AudioSource audioSource;
    [SerializeField] private AudioClip shootSfx;
    [SerializeField] private AudioClip dashSfx;
    private UpgradeUI upgrade;
    public float moveSpeed = 5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public Animator animator;
    public Transform firePoint;
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private PlayerStats stats;
    private PlayerInput input;
    private bool isShooting;

    private Vector2 moveInput;
    private Vector2 dashDirection;
    private bool canDash = true;
    private bool isDashing;
    private float fireTimer;
    private bool isAttacking;
    [HideInInspector] public bool isBuilding = false;
    [Header("Zona Segura")]
    public SafeZone safeZone;                // arraste seu componente SafeZone aqui
    public float damageOutsidePerSecond = 5f;
    private Vector2 lastAimDir;

    void Awake()
    {
        rb          = GetComponent<Rigidbody2D>();
        stats       = GetComponent<PlayerStats>();
        input       = GetComponent<PlayerInput>();
        animator    = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = shootSfx;
    }

    void OnEnable()
    {
        input.actions["Move"].performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled += ctx => moveInput = Vector2.zero;
        input.actions["Dash"].performed += _ => TryDash();
        input.actions["Fire"].performed += _ => Shoot();
    }

    void OnDisable()
    {
        input.actions["Move"].performed -= ctx => moveInput = ctx.ReadValue<Vector2>();
        input.actions["Move"].canceled -= ctx => moveInput = Vector2.zero;
        input.actions["Dash"].performed -= _ => TryDash();
        input.actions["Fire"].performed -= _ => Shoot();
    }

    void Update()
    {
        if (stats.isDead) return;
         AimAtMouse();

         if (!isAttacking)
             UpdateAnimation();

         if (fireTimer > 0f) fireTimer -= Time.deltaTime;
         CheckSafeZone();
    }

    void FixedUpdate()
    {
        if (stats.isDead) return;
        if (!isDashing)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    void AimAtMouse()
     {
         Vector2 mouseScreen = Mouse.current.position.ReadValue();
         Vector3 world       = Camera.main.ScreenToWorldPoint(mouseScreen);
         Vector2 aimDir      = (world - transform.position).normalized;

         firePoint.up = aimDir;

        lastAimDir = aimDir;
     }
    void Shoot()
    {
        if (isBuilding || upgrade.inMenu) return;
       if (fireTimer > 0f)        return;
       fireTimer = 1f / stats.AttackSpeed.Value;
        audioSource.PlayOneShot(shootSfx);
        isAttacking = true;
       // choose and play the correct attack animation
       if (Mathf.Abs(lastAimDir.x) > Mathf.Abs(lastAimDir.y))
       {
           if (lastAimDir.x > 0) animator.Play("AttackRight");
           else                   animator.Play("AttackLeft");
       }
       else
       {
           if (lastAimDir.y > 0) animator.Play("AttackUp");
           else                   animator.Play("AttackDown");
       }

    }
    public void SpawnProjectile()
    {
        var p = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (p.TryGetComponent<Bullet>(out var b))
            b.Init(stats.Damage.Value);
        isAttacking = false;
    }

    public void SetUpUpgradeUI(UpgradeUI x)
    {
        upgrade = x;
    }

    void TryDash()
    {
        if (canDash && moveInput != Vector2.zero)
        {
            dashDirection = moveInput.normalized;
            StartCoroutine(DashRoutine());
        }
    }

    IEnumerator DashRoutine()
    {
        canDash = false;
        isDashing = true;
        rb.linearVelocity = dashDirection * dashSpeed;
        audioSource.PlayOneShot(dashSfx);
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    void UpdateAnimation()
    {
        if (isAttacking) return;
        if (moveInput != Vector2.zero && animator.GetBool("Attack") == false)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0) animator.Play("WalkRight");
                else animator.Play("Walkingleft");
            }
            else
            {
                if (moveInput.y > 0) animator.Play("WalkUp");
                else animator.Play("WalkDown");
            }
        }
        else if (animator.GetBool("Attack") == false)
        {
            var mpos = Mouse.current.position.ReadValue();
            var world = Camera.main.ScreenToWorldPoint(mpos);
            var aimDir = world - transform.position;
            if (Mathf.Abs(aimDir.x) > Mathf.Abs(aimDir.y))
            {
                if (aimDir.x > 0) animator.Play("IdleRight");
                else animator.Play("IdleLeft");
            }
            else
            {
                if (aimDir.y > 0) animator.Play("IdleUp");
                else animator.Play("IdleDown");
            }
        }
    }
    void CheckSafeZone()
    {
        if (safeZone == null) return;

        float dist = Vector2.Distance(
            transform.position,
            safeZone.transform.position
        );

        if (dist > safeZone.radius)
        {
            // está fora da safe zone: aplica dano contínuo
            stats.TakeDamage(damageOutsidePerSecond * Time.deltaTime);
        }
    }
    public void FlashRed()
    {
        StartCoroutine(FlashCoroutine());
    }
    IEnumerator FlashCoroutine()
    {
        // pega todos os sprites no jogador (e crianças)
        var rends = GetComponentsInChildren<SpriteRenderer>();
        // quantas vezes pisca
        int flashes = 2;
        // duração de cada cor
        float onTime  = 0.5f;
        float offTime = 0.5f;

        for (int i = 0; i < flashes; i++)
        {
            // vermelho
            foreach (var r in rends)
                r.color = Color.red;
            yield return new WaitForSeconds(onTime);

            // volta ao normal
            foreach (var r in rends)
                r.color = Color.white;
            yield return new WaitForSeconds(offTime);
        }
    }
}
