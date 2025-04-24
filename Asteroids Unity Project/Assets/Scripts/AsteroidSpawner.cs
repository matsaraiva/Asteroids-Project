using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public static AsteroidSpawner Instance { get; private set; }

    [Header("Spawn Settings")]
    [SerializeField] private float spawnBorderOffset = 1.1f;
    [SerializeField] private int initialAsteroids = 4;
    [SerializeField] private float spawnInterval = 3f;

    [System.Serializable]
    public class AsteroidPool
    {
        public AsteroidSize size;
        public GameObject prefab;
        public int poolSize;
    }

    [SerializeField] private AsteroidPool[] asteroidPools;

    private Dictionary<AsteroidSize, Queue<AsteroidController>> _poolDictionary;
    private Camera _mainCamera;
    private float _nextSpawnTime;
    private bool _isSpawning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _mainCamera = Camera.main;
        InitializePools();
        gameObject.SetActive(true);
    }

    public void ClearAllAsteroids()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void StartSpawning()
    {
        gameObject.SetActive(true);
        _isSpawning = true;
        _nextSpawnTime = Time.time;
        SpawnInitialAsteroids();
    }

    public void StopSpawning()
    {
        _isSpawning = false;
    }

    private void Update()
    {
        if (!_isSpawning) return;

        if (Time.time >= _nextSpawnTime)
        {
            SpawnAsteroid(AsteroidSize.LARGE);
            _nextSpawnTime = Time.time + spawnInterval;
        }
    }

    private void SpawnInitialAsteroids()
    {
        for (int i = 0; i < initialAsteroids; i++)
        {
            SpawnAsteroid(AsteroidSize.LARGE);
        }
    }

    private void SpawnAsteroid(AsteroidSize size)
    {
        Vector2 spawnPosition = GetEdgeSpawnPosition();
        AsteroidController asteroid = GetAsteroidFromPool(size);
        asteroid.transform.position = spawnPosition;
        asteroid.gameObject.SetActive(true);
    }

    private Vector2 GetEdgeSpawnPosition()
    {
        float randomValue = Random.value;
        Vector2 viewportPos;

        if (randomValue < 0.25f) // Top
            viewportPos = new Vector2(Random.value, spawnBorderOffset);
        else if (randomValue < 0.5f) // Right
            viewportPos = new Vector2(spawnBorderOffset, Random.value);
        else if (randomValue < 0.75f) // Bottom
            viewportPos = new Vector2(Random.value, -spawnBorderOffset * 0.1f);
        else // Left
            viewportPos = new Vector2(-spawnBorderOffset * 0.1f, Random.value);

        return _mainCamera.ViewportToWorldPoint(viewportPos);
    }

    private void InitializePools()
    {
        _poolDictionary = new Dictionary<AsteroidSize, Queue<AsteroidController>>();

        foreach (AsteroidPool pool in asteroidPools)
        {
            Queue<AsteroidController> objectPool = new Queue<AsteroidController>();

            for (int i = 0; i < pool.poolSize; i++)
            {
                GameObject obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, transform);
                obj.SetActive(false);
                AsteroidController asteroid = obj.GetComponent<AsteroidController>();
                asteroid.Initialize(pool.size);
                objectPool.Enqueue(asteroid);
            }

            _poolDictionary.Add(pool.size, objectPool);
        }
    }

    public AsteroidController GetAsteroidFromPool(AsteroidSize size)
    {
        if (_poolDictionary[size].Count == 0)
        {
            foreach (AsteroidPool pool in asteroidPools)
            {
                if (pool.size == size)
                {
                    GameObject obj = Instantiate(pool.prefab, Vector3.zero, Quaternion.identity, transform);
                    obj.SetActive(false);
                    AsteroidController newAsteroid = obj.GetComponent<AsteroidController>();
                    newAsteroid.Initialize(size);
                    return newAsteroid;
                }
            }
        }

        return _poolDictionary[size].Dequeue();
    }

    public void ReturnAsteroidToPool(AsteroidController asteroid)
    {
        asteroid.gameObject.SetActive(false);
        _poolDictionary[asteroid.Size].Enqueue(asteroid);
    }
}