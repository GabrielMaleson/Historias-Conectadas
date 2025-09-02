using UnityEngine;
using System.Collections;

public class PopupScript : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] objectsToSpawn; // Array of prefabs to spawn from
    public Transform spawnPointA;       // First spawn boundary point
    public Transform spawnPointB;       // Second spawn boundary point

    [Header("Spawn Timing")]
    public float minSpawnTime = 2f;     // Minimum time between spawns
    public float maxSpawnTime = 5f;     // Maximum time between spawns

    private Coroutine spawnCoroutine;

    public void StartSpawning()
    {
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // Wait for a random time between min and max spawn times
            float waitTime = Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            // Spawn the object
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (objectsToSpawn == null || objectsToSpawn.Length == 0 || spawnPointA == null || spawnPointB == null)
        {
            Debug.LogWarning("Spawner not properly configured!");
            return;
        }

        // Select a random prefab from the array
        GameObject objectToSpawn = GetRandomPrefab();

        if (objectToSpawn == null)
        {
            Debug.LogWarning("Selected prefab is null!");
            return;
        }

        // Calculate random position between spawnPointA and spawnPointB
        Vector3 randomPosition = GetRandomSpawnPosition();

        // Instantiate the object
        Instantiate(objectToSpawn, randomPosition, Quaternion.identity);
    }

    GameObject GetRandomPrefab()
    {
        // Return a random prefab from the array
        int randomIndex = Random.Range(0, objectsToSpawn.Length);
        return objectsToSpawn[randomIndex];
    }

    Vector3 GetRandomSpawnPosition()
    {
        // Get random interpolation value between 0 and 1
        float t = Random.Range(0f, 1f);

        // Linearly interpolate between the two spawn points
        Vector3 randomPosition = Vector3.Lerp(spawnPointA.position, spawnPointB.position, t);

        return randomPosition;
    }

    // Optional: Visualize spawn area in the editor
    void OnDrawGizmosSelected()
    {
        if (spawnPointA != null && spawnPointB != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(spawnPointA.position, spawnPointB.position);
            Gizmos.DrawSphere(spawnPointA.position, 0.1f);
            Gizmos.DrawSphere(spawnPointB.position, 0.1f);
        }
    }
}