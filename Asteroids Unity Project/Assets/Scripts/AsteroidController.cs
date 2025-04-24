using UnityEngine;

public enum AsteroidSize { LARGE, MEDIUM, SMALL }

public class AsteroidController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private AsteroidSize size = AsteroidSize.LARGE;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float minRandomForce = 0.5f;
    [SerializeField] private float maxRandomForce = 2f;
    [SerializeField] private int splitCount = 2;
    [SerializeField] private AsteroidSize smallerSize;

    public AsteroidSize Size => size;

    [Header("Prefabs")]
    [SerializeField] private GameObject smallerAsteroidPrefab;
    [SerializeField] private GameObject explosionEffect;

    private Rigidbody2D _rb;
    private Vector2 _moveDirection;
    private Collider2D _collider;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        ApplyInitialForce();
    }

    private void ApplyInitialForce()
    {
        // Direção aleatória com força aleatória
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        float randomForce = Random.Range(minRandomForce, maxRandomForce);
        _rb.AddForce(randomDirection * moveSpeed * randomForce, ForceMode2D.Impulse);

        // Rotação aleatória
        float torque = Random.Range(-50f, 50f);
        _rb.AddTorque(torque);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            DestroyAsteroid(collision.contacts[0].point);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            DestroyAsteroid(collision.contacts[0].point);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            DestroyAsteroid(collision.contacts[0].point);
        }
    }

    private void DestroyAsteroid(Vector2 hitPosition)
    {
        // Notifica o GameManager para adicionar pontos
        GameManager.Instance.AddScore(size);

        // Cria efeito de explosão
        if (explosionEffect)
            Instantiate(explosionEffect, hitPosition, Quaternion.identity);

        // Se não for o menor tamanho, divide em asteroides menores
        if (size != AsteroidSize.SMALL)
        {
            SplitAsteroid();
        }

        // Notifica a destruição do asteroide
        EventManager.Instance.TriggerEvent(EventType.ASTEROID_DESTROYED, size);

        // Retorna ao pool ou destrói
        AsteroidSpawner.Instance.ReturnAsteroidToPool(this);
    }

    private void SplitAsteroid()
    {
        for (int i = 0; i < splitCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * 0.5f;
            Vector2 spawnPosition = (Vector2)transform.position + spawnOffset;

            AsteroidController newAsteroid = AsteroidSpawner.Instance.GetAsteroidFromPool(smallerSize);
            newAsteroid.transform.position = spawnPosition;
            newAsteroid.gameObject.SetActive(true);
        }
    }

    public void Initialize(AsteroidSize newSize)
    {
        size = newSize;
        gameObject.SetActive(true);
        ApplyInitialForce();
    }
}