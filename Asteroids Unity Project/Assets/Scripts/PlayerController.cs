using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float maxVelocity = 7f;
    [SerializeField] private float dragFactor = 0.98f;

    [Header("Shooting")]
    //[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 0.2f;

    private Rigidbody2D _rb;
    private float _nextFireTime;
    private bool _isActive = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.drag = 0.5f;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.AddListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.AddListener(EventType.PLAYER_RESPAWN, OnRespawn);
        EventManager.Instance.AddListener(EventType.PLAYER_DIED, OnPlayerDied);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.RemoveListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.RemoveListener(EventType.PLAYER_RESPAWN, OnRespawn);
        EventManager.Instance.RemoveListener(EventType.PLAYER_DIED, OnPlayerDied);
    }

    private void OnPlayerDied(object data)
    {
        _isActive = false;
    }

    private void Update()
    {
        if (!_isActive) return;

        HandleRotation();
        HandleThrust();
        HandleShooting();
        ApplyDrag();
    }

    private void HandleRotation()
    {
        float rotationInput = -Input.GetAxis("Horizontal");
        transform.Rotate(0, 0, rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void HandleThrust()
    {
        float thrustInput = Input.GetAxis("Vertical");
        if (thrustInput > 0)
        {
            Vector2 force = transform.up * (moveSpeed * thrustInput * Time.deltaTime * 1000);
            _rb.AddForce(force);
            _rb.velocity = Vector2.ClampMagnitude(_rb.velocity, maxVelocity);
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + fireRate;

            Fire();
            EventManager.Instance.TriggerEvent(EventType.PLAYER_SHOOT);
        }
    }

    private void Fire()
    {
        GameObject projectile = ProjectilePool.Instance.GetProjectile();

        // Configura posição/rotação (responsabilidade do Player)
        projectile.transform.position = firePoint.position;
        projectile.transform.rotation = firePoint.rotation;

        projectile.SetActive(true);
    }

    private void ApplyDrag()
    {
        if (Input.GetAxis("Vertical") <= 0)
        {
            _rb.velocity *= Mathf.Pow(dragFactor, Time.deltaTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Asteroid") || other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Enemy"))
        {
            // Cria efeito de explosão
            if (explosionEffect)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);
            
            gameObject.SetActive(false);
            EventManager.Instance.TriggerEvent(EventType.PLAYER_DIED);
        }
    }

    private void OnGameStarted(object data) => _isActive = true;
    private void OnGameOver(object data) => _isActive = false;

    private void OnRespawn(object data)
    {
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        _rb.velocity = Vector2.zero;
        gameObject.SetActive(true);
        _isActive = true;
    }
}