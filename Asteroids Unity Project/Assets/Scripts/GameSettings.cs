using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Asteroids/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Player Settings")]
    public float playerRotationSpeed = 150f;
    public float playerMoveSpeed = 5f;
    public float playerMaxVelocity = 7f;
    public float playerDragFactor = 0.98f;
    public float fireRate = 0.2f;
    public int initialLives = 3;

    [Header("Asteroid Settings")]
    public float largeAsteroidSpeed = 2f;
    public float mediumAsteroidSpeed = 3f;
    public float smallAsteroidSpeed = 4f;
    public int initialAsteroids = 4;
    public int scorePerLargeAsteroid = 20;

    [Header("Projectile Settings")]
    public float projectileSpeed = 10f;
    public float projectileLifetime = 2f;
}