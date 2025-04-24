using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivesCounter : MonoBehaviour
{
    // Intacia ou destroi Objetos do tipo Live como filho desse, dependendo da quantidade de vidas

    [SerializeField] private GameObject livePrefab;

    private int lives = 3;

    private List<GameObject> livesList = new List<GameObject>();

    private void Start()
    {
        UpdateLivesDisplay();
    }

    private void UpdateLivesDisplay()
    {
        // Remove all existing lives
        foreach (GameObject live in livesList)
        {
            Destroy(live);
        }
        livesList.Clear();

        // Create new lives
        for (int i = 0; i < lives; i++)
        {
            GameObject live = Instantiate(livePrefab, transform);
            live.transform.localPosition = new Vector3(i * 1.5f, 0, 0); // Adjust position as needed
            livesList.Add(live);
        }
    }

    public void SetLives(int newLives)
    {
        lives = newLives;
        UpdateLivesDisplay();
    }

}
