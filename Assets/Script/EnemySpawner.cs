using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance;

    [Header("Enemy Prefabs")]
    public GameObject[] enemyPrefabs;

    [Header("Map Tilemaps")]
    public Tilemap ground;
    public Tilemap wall;

    [Header("Spawn Settings")]
    public float spawnInterval = 2f;
    public int maxEnemies = 20;

    private float timer;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<Vector3> walkableTiles = new List<Vector3>();
    private Player player;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        CacheWalkableTiles();
    }

    private void Start()
    {
        // FIX: Use FindFirstObjectByType to stop the Yellow Warning
        player = FindFirstObjectByType<Player>();
    }

    private void Update()
    {
        if (player == null) return;

        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            timer = 0f;
            TrySpawn();
        }
    }

    private void TrySpawn()
    {
        if (activeEnemies.Count >= maxEnemies) return;
        List<Vector3> tilesOutsideView = GetTilesOutsideCamera();
        if (tilesOutsideView.Count == 0) return;

        Vector3 spawnPos = tilesOutsideView[Random.Range(0, tilesOutsideView.Count)];
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject spawned = Instantiate(prefab, spawnPos, Quaternion.identity);

        activeEnemies.Add(spawned);
        spawned.AddComponent<CleanupTracker>().Init(this);

        if (spawned != null)
        {
            // FIX: Use 'player.Level' (Capital L) to fix the Red Error
            int minLevel = player.Level;
            int maxLevel = player.Level + 1;
            int assignedLevel = Random.Range(minLevel, maxLevel + 1);
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
    }

    private void CacheWalkableTiles()
    {
        walkableTiles.Clear();
        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            if (!ground.HasTile(pos)) continue;
            if (wall != null && wall.HasTile(pos)) continue;
            walkableTiles.Add(ground.GetCellCenterWorld(pos));
        }
    }

    private List<Vector3> GetTilesOutsideCamera()
    {
        List<Vector3> list = new List<Vector3>();
        Camera cam = Camera.main;
        if (cam == null) return list;

        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;
        Vector3 camPos = cam.transform.position;
        float left = camPos.x - camWidth;
        float right = camPos.x + camWidth;
        float bottom = camPos.y - camHeight;
        float top = camPos.y + camHeight;

        foreach (var t in walkableTiles)
        {
            if (t.x < left || t.x > right || t.y < bottom || t.y > top)
                list.Add(t);
        }
        return list;
    }

    private class CleanupTracker : MonoBehaviour
    {
        private EnemySpawner spawner;
        public void Init(EnemySpawner spawner) { this.spawner = spawner; }
        private void OnDestroy() { if (spawner != null) spawner.RemoveEnemy(gameObject); }
    }
}