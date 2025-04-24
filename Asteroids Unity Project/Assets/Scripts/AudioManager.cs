using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume = 1f;
    public bool loop = false;
    [HideInInspector] public AudioSource source;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private Sound[] sounds;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        EventManager.Instance.AddListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.AddListener(EventType.PLAYER_DIED, OnPlayerDied);
        EventManager.Instance.AddListener(EventType.ASTEROID_DESTROYED, OnAsteroidDestroyed);
        EventManager.Instance.AddListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.AddListener(EventType.PLAYER_SHOOT, OnProjectileFired);
        EventManager.Instance.AddListener(EventType.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.Instance.AddListener(EventType.ENEMY_SPAWNED, OnEnemySpawned);
        EventManager.Instance.AddListener(EventType.ENEMY_SHOOT, OnEnemyShoot);

    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(EventType.GAME_STARTED, OnGameStarted);
        EventManager.Instance.RemoveListener(EventType.PLAYER_DIED, OnPlayerDied);
        EventManager.Instance.RemoveListener(EventType.ASTEROID_DESTROYED, OnAsteroidDestroyed);
        EventManager.Instance.RemoveListener(EventType.GAME_OVER, OnGameOver);
        EventManager.Instance.RemoveListener(EventType.PLAYER_SHOOT, OnProjectileFired);
        EventManager.Instance.RemoveListener(EventType.ENEMY_DESTROYED, OnEnemyDestroyed);
        EventManager.Instance.RemoveListener(EventType.ENEMY_SPAWNED, OnEnemySpawned);
        EventManager.Instance.RemoveListener(EventType.ENEMY_SHOOT, OnEnemyShoot);
    }

    public void Play(string soundName)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        s.source.Play();
    }

    public void Stop(string soundName)
    {
        Sound s = System.Array.Find(sounds, sound => sound.name == soundName);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + soundName + " not found!");
            return;
        }
        s.source.Stop();
    }

    private void OnGameStarted(object data) => Play("GameStart");
    private void OnPlayerDied(object data) => Play("PlayerDeath");
    private void OnAsteroidDestroyed(object size) => Play("AsteroidExplosion");

    private void OnGameOver(object data) => Play("GameOver");

    private void OnProjectileFired(object data) => Play("ProjectileFired");

    private void OnEnemyDestroyed(object data) => Play("EnemyDestroyed");

    private void OnEnemySpawned(object data) => Play("EnemySpawned");

    private void OnEnemyShoot(object data) => Play("EnemyShoot");

}