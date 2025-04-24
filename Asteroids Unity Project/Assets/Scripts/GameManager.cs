using UnityEngine;

public enum GameState { MENU, PLAYING, GAME_OVER }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player Settings")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnProtectionPrefab;
    [SerializeField] private int initialLives = 3;

    [Header("Game Settings")]
    [SerializeField] private int scorePerLargeAsteroid = 20;

    private GameObject spawnProtection;
    private GameObject currentPlayer;
    private int _currentScore;
    private int _currentLives;
    private GameState _currentState;

    public int CurrentScore => _currentScore;
    public int CurrentLives => _currentLives;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeGame();
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if (EventManager.Instance == null) return;

        // Eventos de controle do jogo
        EventManager.Instance.AddListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.AddListener(EventType.GAME_OVER, OnGameOver);

        // Eventos do player
        EventManager.Instance.AddListener(EventType.PLAYER_DIED, OnPlayerDied);
        EventManager.Instance.AddListener(EventType.PLAYER_RESPAWN, OnPlayerRespawn);

        // Eventos de progresso
        EventManager.Instance.AddListener(EventType.SCORE_UPDATED, OnScoreUpdated);
        EventManager.Instance.AddListener(EventType.LIVES_UPDATED, OnLivesUpdated);

        // Evento de destruição de asteroides
        EventManager.Instance.AddListener(EventType.ASTEROID_DESTROYED, OnAsteroidDestroyed);
    }

    private void UnsubscribeFromEvents()
    {
        if (EventManager.Instance == null) return;

        EventManager.Instance.RemoveListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.RemoveListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.RemoveListener(EventType.PLAYER_DIED, OnPlayerDied);
        EventManager.Instance.RemoveListener(EventType.PLAYER_RESPAWN, OnPlayerRespawn);
        EventManager.Instance.RemoveListener(EventType.SCORE_UPDATED, OnScoreUpdated);
        EventManager.Instance.RemoveListener(EventType.LIVES_UPDATED, OnLivesUpdated);
        EventManager.Instance.RemoveListener(EventType.ASTEROID_DESTROYED, OnAsteroidDestroyed);
    }


    private void InitializeGame()
    {
        _currentScore = 0;
        _currentLives = initialLives;
        _currentState = GameState.MENU;

        // Não desativamos o spawner aqui - só preparamos o jogo
        if (AsteroidSpawner.Instance != null)
        {
            AsteroidSpawner.Instance.StopSpawning(); // Para qualquer spawn ativo
            AsteroidSpawner.Instance.ClearAllAsteroids(); // Limpa asteroides existentes
        }

        EventManager.Instance.TriggerEvent(EventType.GAME_INITIALIZED);
        EventManager.Instance.TriggerEvent(EventType.SCORE_UPDATED, _currentScore);
        EventManager.Instance.TriggerEvent(EventType.LIVES_UPDATED, _currentLives);
    }

    public void StartGame()
    {
        _currentState = GameState.PLAYING;

        // Ativa o spawner corretamente
        if (AsteroidSpawner.Instance != null)
        {
            AsteroidSpawner.Instance.gameObject.SetActive(true);
            AsteroidSpawner.Instance.StartSpawning();
        }

        EventManager.Instance.TriggerEvent(EventType.GAME_STARTED);
    }

    // Implementação dos handlers
    public void OnGameStarted(object data)
    {
        _currentState = GameState.PLAYING;

        SpawnPlayer();
    }

    public void OnGameOver(object data)
    {
        _currentState = GameState.GAME_OVER;
        AsteroidSpawner.Instance?.StopSpawning();
    }

    public void OnPlayerDied(object data)
    {
        _currentLives--;
        EventManager.Instance?.TriggerEvent(EventType.LIVES_UPDATED, _currentLives);

        if (_currentLives <= 0)
        {
            EventManager.Instance?.TriggerEvent(EventType.GAME_OVER);
        }
        else
        {
            Invoke(nameof(TriggerRespawn), 0.5f);

            spawnProtection = Instantiate(spawnProtectionPrefab, Vector3.zero, Quaternion.identity);
        }
    }

    private void TriggerRespawn()
    {
        EventManager.Instance?.TriggerEvent(EventType.PLAYER_RESPAWN);
    }

    public void OnPlayerRespawn(object data)
    {
        SpawnPlayer();
    }

    public void OnScoreUpdated(object newScore)
    {
        _currentScore = (int)newScore;
    }

    public void OnLivesUpdated(object newLives)
    {
        _currentLives = (int)newLives;
    }

    public void OnAsteroidDestroyed(object asteroidSize)
    {
        AddScore((AsteroidSize)asteroidSize);
    }

    private void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("PlayerPrefab não atribuído no GameManager!");
            return;
        }

        if (currentPlayer != null)
            Destroy(currentPlayer);

        if (spawnProtection != null)
            Destroy(spawnProtection);

        currentPlayer = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        currentPlayer.SetActive(true);
    }

    public void AddScore(AsteroidSize size)
    {
        int points = size switch
        {
            AsteroidSize.LARGE => scorePerLargeAsteroid,
            AsteroidSize.MEDIUM => scorePerLargeAsteroid * 2,
            AsteroidSize.SMALL => scorePerLargeAsteroid * 3,
            _ => 0
        };

        _currentScore += points;
        EventManager.Instance.TriggerEvent(EventType.SCORE_UPDATED, _currentScore);
    }

    public void ReturnToMenu()
    {
        AsteroidSpawner.Instance.StopSpawning();

        if (currentPlayer != null)
            Destroy(currentPlayer);

        InitializeGame();
    }
}