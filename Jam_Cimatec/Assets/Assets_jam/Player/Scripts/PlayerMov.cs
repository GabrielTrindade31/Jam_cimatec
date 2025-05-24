using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerStats))]
[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
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
    [HideInInspector] public bool isBuilding = false;

    void Awake()
    {
         rb        = GetComponent<Rigidbody2D>();
        stats     = GetComponent<PlayerStats>();
        input     = GetComponent<PlayerInput>();
        animator  = GetComponent<Animator>();
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
        AimAtMouse();
        UpdateAnimation();
        if (fireTimer > 0f) fireTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    void AimAtMouse()
    {
        Vector2 mouseScreen = Mouse.current.position.ReadValue();
        Vector3 world      = Camera.main.ScreenToWorldPoint(mouseScreen);
        Vector2 aimDir     = (world - transform.position).normalized;

        firePoint.up = aimDir;
    }
    void Shoot()
    {
        if (!isBuilding)
        {
            if (fireTimer > 0f) return;
            fireTimer = 1f / stats.AttackSpeed.Value;
            animator.SetBool("Attack", true);
        }

    }
    public void SpawnProjectile()
    {
        var p = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (p.TryGetComponent<Bullet>(out var b))
            b.Init(stats.Damage.Value);
        animator.SetBool("Attack", false);
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
        yield return new WaitForSeconds(dashDuration);
        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
     void UpdateAnimation()
    {
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
        else if(animator.GetBool("Attack") == false)
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
}
