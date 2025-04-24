using UnityEngine;

public class EnemyShip : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float directionChangeInterval = 2f;
    [SerializeField] private float shootingInterval = 1.5f;
    
    //[SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePointParent;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private Transform firePoint;
    private Collider2D _collider;
    private bool asteroidResistance = true;//resiste ao primeiro asteroid

    private Vector2 _currentDirection;
    private float _directionTimer;
    private float _shootingTimer;
    private Camera _mainCamera;

    private void Awake()
    {
        _collider = GetComponent<Collider2D>();

        //disable collider on spawn
        _collider.enabled = false;
        asteroidResistance = true;

        _mainCamera = Camera.main;
        SetRandomDirection();

        Invoke(nameof(EnableCollider), 1f);
    }

    private void Update()
    {
        Move();
        Shoot();
    }

    private void Move()
    {
        _directionTimer += Time.deltaTime;

        if (_directionTimer >= directionChangeInterval)
        {
            SetRandomDirection();

            // Verifica se há asteroides na direção escolhida
            for (int i = 0; i < 10; i++)
            {
                if (!AsteroidOnWay())
                {
                    break;
                }
                SetRandomDirection();
            }

            _directionTimer = 0f;
        }

        transform.Translate(_currentDirection * moveSpeed * Time.deltaTime);
    }

    private void SetRandomDirection()
    {
        _currentDirection = new Vector2(
            Random.Range(-1f, 1f),
            Random.Range(-1f, 1f)
        ).normalized;
    }

    private bool AsteroidOnWay()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, _currentDirection, 1f);
        if (hit.collider != null && hit.collider.CompareTag("Asteroid"))
        {
            return true;
        }
        return false;
    }

    private void Shoot()
    {
        _shootingTimer += Time.deltaTime;

        if (_shootingTimer >= shootingInterval)
        {
            if(AsteroidOnWay())
            {
                // Em direção que se move
                firePointParent.transform.rotation = _currentDirection.x > 0 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, 180);
            }
            else
            {
                // Em direção aleatória
                firePointParent.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
            }

            // Pegar projétil do pool em vez de Instantiate
            GameObject projectile = ProjectilePool.Instance.GetProjectile();
            projectile.transform.position = firePoint.position;
            projectile.transform.rotation = firePoint.rotation;
            projectile.SetActive(true);

            _shootingTimer = 0f;

            EventManager.Instance?.TriggerEvent(EventType.ENEMY_SHOOT);
        }
    }

    private void EnableCollider()
    {
        _collider.enabled = true;
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(asteroidResistance && other.gameObject.CompareTag("Asteroid"))
        {
            asteroidResistance = false;
            _collider.enabled = false;
            Invoke(nameof(EnableCollider), 0.1f);
        }
        else if (other.gameObject.CompareTag("Projectile") || other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Asteroid"))
        {
            // Cria efeito de explosão
            if (explosionEffect)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);

            // Notifica a destruição do inimigo
            EventManager.Instance?.TriggerEvent(EventType.ENEMY_DESTROYED);
        }
        
    }
}