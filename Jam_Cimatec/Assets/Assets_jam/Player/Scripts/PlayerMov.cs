using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(PlayerStats), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 12f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;

    public Transform firePoint;
    public GameObject projectilePrefab;

    private Rigidbody2D rb;
    private PlayerStats stats;
    private PlayerInput input;

    private Vector2 moveInput;
    private Vector2 dashDirection;
    private bool canDash = true;
    private bool isDashing;
    private float fireTimer;
    [HideInInspector] public bool isBuilding = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stats = GetComponent<PlayerStats>();
        input = GetComponent<PlayerInput>();
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
        Vector2 mpos = Mouse.current.position.ReadValue();
        Vector3 world = Camera.main.ScreenToWorldPoint(mpos);
        Vector2 dir = (world - transform.position).normalized;
        rb.rotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;

        if (fireTimer > 0f) fireTimer -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isDashing)
            rb.linearVelocity = moveInput * moveSpeed;
    }

    void Shoot()
    {
        if (!isBuilding)
        {
            if (fireTimer > 0) return;
            GameObject p = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            if (p.TryGetComponent<Bullet>(out var b)) b.Init(stats.Damage.Value);
            fireTimer = 1f / stats.AttackSpeed.Value;
        }
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
}
