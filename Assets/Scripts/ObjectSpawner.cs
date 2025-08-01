using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [Header("Coin Settings")]
    public GameObject[] coinPrefabs;
    public int maxCoinsOnScreen = 25;

    [Header("Spawn Timing")]
    public float minSpawnInterval = 0.5f;  // Minimum seconds between spawns
    public float maxSpawnInterval = 1.5f;  // Maximum seconds between spawns

    [Header("Spawn Position")]
    public float spawnDistanceRight = 15f; // How far right of screen to spawn
    public float verticalRange = 3f;       // Y range above/below camera center

    [Header("Recycling")]
    public float despawnDistanceLeft = 10f; // How far left of screen before despawning

    [Header("References")]
    public Camera mainCamera;

    private List<GameObject> activeCoinPool = new List<GameObject>();
    private List<GameObject> inactiveCoinPool = new List<GameObject>();
    private Coroutine spawnCoroutine;

    void Start()
    {
        // Pre-instantiate coin pool
        InitializeCoinPool();

        // Start spawning coins
        spawnCoroutine = StartCoroutine(SpawnCoinRoutine());
    }

    void Update()
    {
        RecycleCoinsOffScreen();
    }

    void InitializeCoinPool()
    {
        if (coinPrefabs == null || coinPrefabs.Length == 0)
        {
            Debug.LogError("CoinSpawner: No coin prefabs assigned!");
            return;
        }

        for (int i = 0; i < maxCoinsOnScreen; i++)
        {
            GameObject prefab = GetRandomCoinPrefab();
            GameObject coin = Instantiate(prefab);
            coin.SetActive(false);
            inactiveCoinPool.Add(coin);
        }
    }

    IEnumerator SpawnCoinRoutine()
    {
        while (true)
        {
            // Wait for random interval
            float waitTime = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(waitTime);

            // Spawn coin if we have available coins in pool
            if (inactiveCoinPool.Count > 0 && activeCoinPool.Count < maxCoinsOnScreen)
            {
                SpawnCoin();
            }
        }
    }

    void SpawnCoin()
    {
        // Limpa possíveis referências nulas/destroídas na inactiveCoinPool
        inactiveCoinPool.RemoveAll(coin => coin == null);

        if (inactiveCoinPool.Count == 0)
        {
            Debug.LogWarning("No inactive coins available to spawn!");
            return;
        }

        // Pega o primeiro coin válido
        GameObject coin = inactiveCoinPool[0];
        inactiveCoinPool.RemoveAt(0);
        activeCoinPool.Add(coin);

        Vector3 spawnPosition = GetRandomSpawnPosition();
        coin.transform.position = spawnPosition;
        coin.SetActive(true);
    }


    Vector3 GetRandomSpawnPosition()
    {
        // Get camera bounds
        Vector3 cameraPos = mainCamera.transform.position;

        // Get right edge of screen in world coordinates
        Vector3 rightEdgeWorld = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, mainCamera.nearClipPlane));

        // Spawn position
        float x = rightEdgeWorld.x + spawnDistanceRight;
        float y = cameraPos.y + Random.Range(-verticalRange, verticalRange);
        float z = 0f; // 2D game

        return new Vector3(x, y, z);
    }

    void RecycleCoinsOffScreen()
    {
        // Get left edge of screen in world coordinates
        Vector3 leftEdgeWorld = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, mainCamera.nearClipPlane));
        float despawnX = leftEdgeWorld.x - despawnDistanceLeft;

        // Check active coins for recycling
        for (int i = activeCoinPool.Count - 1; i >= 0; i--)
        {
            GameObject coin = activeCoinPool[i];

            if (coin == null)
            {
                activeCoinPool.RemoveAt(i);
                continue;
            }

            // If coin is too far left, recycle it
            if (coin.transform.position.x < despawnX)
            {
                RecycleCoin(coin, i);
            }
        }
    }

    void RecycleCoin(GameObject coin, int activeIndex)
    {
        // Move from active to inactive pool
        activeCoinPool.RemoveAt(activeIndex);
        inactiveCoinPool.Add(coin);

        // Deactivate coin
        coin.SetActive(false);
    }

    // Public method to manually recycle a coin (e.g., when collected by player)
    public void CollectCoin(GameObject coin)
    {
        int index = activeCoinPool.IndexOf(coin);
        if (index >= 0)
        {
            RecycleCoin(coin, index);
        }
    }

    void OnDestroy()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    GameObject GetRandomCoinPrefab()
    {
        if (coinPrefabs == null || coinPrefabs.Length == 0) return null;
        return coinPrefabs[Random.Range(0, coinPrefabs.Length)];
    }

    // Optional: Gizmos for debugging spawn area
    void OnDrawGizmosSelected()
    {
        if (mainCamera == null) return;

        Vector3 cameraPos = mainCamera.transform.position;
        Vector3 rightEdge = mainCamera.ViewportToWorldPoint(new Vector3(1f, 0.5f, mainCamera.nearClipPlane));

        // Draw spawn line
        Gizmos.color = Color.yellow;
        float spawnX = rightEdge.x + spawnDistanceRight;
        Vector3 spawnTop = new Vector3(spawnX, cameraPos.y + verticalRange, 0);
        Vector3 spawnBottom = new Vector3(spawnX, cameraPos.y - verticalRange, 0);
        Gizmos.DrawLine(spawnTop, spawnBottom);

        // Draw despawn line
        Gizmos.color = Color.red;
        Vector3 leftEdge = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0.5f, mainCamera.nearClipPlane));
        float despawnX = leftEdge.x - despawnDistanceLeft;
        Vector3 despawnTop = new Vector3(despawnX, cameraPos.y + verticalRange, 0);
        Vector3 despawnBottom = new Vector3(despawnX, cameraPos.y - verticalRange, 0);
        Gizmos.DrawLine(despawnTop, despawnBottom);
    }
}