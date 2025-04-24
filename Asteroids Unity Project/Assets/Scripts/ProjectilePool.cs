using System.Collections.Generic;
using UnityEngine;

public class ProjectilePool : MonoBehaviour
{
    public static ProjectilePool Instance;

    [Header("Configura��o")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private int minPoolSize = 10;

    private List<GameObject> projectilePool = new List<GameObject>();
    private int poolSize;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Inicializa o pool 
    private void InitializePool()
    {
        poolSize = minPoolSize;

        for (int i = 0; i < poolSize; i++)
        {
            CreateProjectile();
        }
    }

    // Cria um novo proj�til e adiciona ao pool
    private void CreateProjectile()
    {
        GameObject newProjectile = Instantiate(projectilePrefab);
        newProjectile.SetActive(false);
        projectilePool.Add(newProjectile);
    }

    // Pega um proj�til dispon�vel
    public GameObject GetProjectile()
    {
        // Primeiro tenta pegar um inativo
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeInHierarchy)
            {
                return projectilePool[i];
            }
        }

        // Se n�o achar, cria um novo
        GameObject newProjectile = Instantiate(projectilePrefab);
        projectilePool.Add(newProjectile);
        return newProjectile;
    }
}