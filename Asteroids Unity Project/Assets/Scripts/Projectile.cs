using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 2f;

    private Rigidbody2D _rb;
    private Vector2 _moveDirection;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        // Reseta a física
        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;
        _rb.Sleep();
        _rb.WakeUp();

        // Calcula direção antes de ativar
        _moveDirection = transform.up;

        // Ativa o objeto
        gameObject.SetActive(true);

        // Aplica movimento após ativação
        _rb.velocity = _moveDirection * speed;

        // Invoke para desativar após o tempo de vida
        Invoke(nameof(Disable), lifetime);
    }

    private void FixedUpdate()
    {
        // Mantém velocidade constante independente de framerate
        _rb.velocity = _moveDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // verifica se é player, asteroid ou projectile
        if (other.CompareTag("Player") || other.CompareTag("Asteroid") || other.CompareTag("Projectile"))
        {
            // Desativa o projétil
            Disable();
        }

    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        // Cancela Invoke
        CancelInvoke();
    }


}