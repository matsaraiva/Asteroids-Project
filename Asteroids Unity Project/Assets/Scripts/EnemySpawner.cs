using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxEnemies = 2;

    private float _spawnTimer;
    //lista de inimigos
    private List<GameObject> _enemies = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.AddListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.AddListener(EventType.ENEMY_DESTROYED, OnEnemyDestroyed);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.RemoveListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.RemoveListener(EventType.ENEMY_DESTROYED, OnEnemyDestroyed);
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.PLAYING) return;

        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= spawnInterval && _enemies.Count < maxEnemies)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
        }
    }

    private void SpawnEnemy()
    {
        Vector2 spawnPosition = GetEdgeSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);

        _enemies.Add(enemy);

        EventManager.Instance?.TriggerEvent(EventType.ENEMY_SPAWNED, enemy);
    }

    private Vector2 GetEdgeSpawnPosition()
    {
        Camera mainCamera = Camera.main;
        float randomValue = Random.value;

        Vector2 viewportPos = randomValue < 0.5f 
            ? new Vector2(Random.value, Random.Range(0, 2) * 1.1f)
            : new Vector2(Random.Range(0, 2) * 1.1f, Random.value);

        return mainCamera.ViewportToWorldPoint(viewportPos);
    }

    private void OnGameStarted(object data)
    {
        _spawnTimer = 0f;
        _enemies.Clear();
    }

    private void OnGameOver(object data)
    {
        _spawnTimer = 0f;
        foreach (var enemy in _enemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _enemies.Clear();
    }

    public void OnEnemyDestroyed(object data)
    {
        for (int i = _enemies.Count - 1; i >= 0; i--)
        {
            if (_enemies[i] == null)
            {
                _enemies.RemoveAt(i);
            }
        }
    }
}