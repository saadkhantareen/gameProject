using UnityEngine;

/// <summary>
/// Place this script on an empty GameObject called "BatterySpawner".
/// Assign your Battery prefab and then add Transform spawn points.
/// It will spawn ONE battery per spawn point at start, with optional respawn.
/// </summary>
public class BatterySpawner : MonoBehaviour
{
    [Header("Battery Prefab")]
    [Tooltip("Drag your Battery prefab here (must have PickupItem component with ItemType = Battery)")]
    public GameObject batteryPrefab;

    [Header("Spawn Points")]
    [Tooltip("Add empty GameObjects as children of BatterySpawner, OR assign them manually here")]
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    [Tooltip("How many batteries to spawn out of all spawn points (randomised)")]
    public int batteriesToSpawn = 4;

    [Tooltip("Should empty spots respawn a battery after a delay? (false = one-time only)")]
    public bool respawnBatteries = false;

    [Tooltip("How many seconds before a used spot respawns a battery")]
    public float respawnDelay = 60f;

    // Tracks which spots have a live battery
    private GameObject[] spawnedBatteries;

    void Start()
    {
        // If no spawn points assigned manually, grab all child transforms
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            spawnPoints = new Transform[transform.childCount];
            for (int i = 0; i < transform.childCount; i++)
                spawnPoints[i] = transform.GetChild(i);
        }

        if (batteryPrefab == null)
        {
            Debug.LogError("BatterySpawner: No battery prefab assigned!");
            return;
        }

        spawnedBatteries = new GameObject[spawnPoints.Length];

        SpawnInitialBatteries();
    }

    void SpawnInitialBatteries()
    {
        // Shuffle spawn points so batteries appear in random spots each playthrough
        int[] indices = ShuffledIndices(spawnPoints.Length);

        int count = Mathf.Min(batteriesToSpawn, spawnPoints.Length);

        for (int i = 0; i < count; i++)
        {
            int idx = indices[i];
            SpawnAt(idx);
        }

        Debug.Log($"BatterySpawner: Spawned {count} batteries.");
    }

    void SpawnAt(int index)
    {
        if (spawnPoints[index] == null) return;

        GameObject battery = Instantiate(
            batteryPrefab,
            spawnPoints[index].position,
            spawnPoints[index].rotation
        );

        spawnedBatteries[index] = battery;

        // Watch for when the player picks it up
        if (respawnBatteries)
            StartCoroutine(WatchAndRespawn(index));
    }

    System.Collections.IEnumerator WatchAndRespawn(int index)
    {
        // Wait until the battery GameObject is destroyed (picked up)
        while (spawnedBatteries[index] != null)
            yield return new WaitForSeconds(1f);

        // Wait respawn delay
        yield return new WaitForSeconds(respawnDelay);

        // Respawn
        SpawnAt(index);
        Debug.Log($"BatterySpawner: Respawned battery at spot {index}");
    }

    // Fisher-Yates shuffle of indices
    int[] ShuffledIndices(int length)
    {
        int[] arr = new int[length];
        for (int i = 0; i < length; i++) arr[i] = i;

        for (int i = length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }
}