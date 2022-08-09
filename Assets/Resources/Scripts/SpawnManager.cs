using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region SINGLETON
    private static SpawnManager instance = null;
    public static SpawnManager Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    public enum EnemyType { Danger, Safe }
    public int numOfEnemyToSpawnAtStart;
    public int numOfEnemyToSpawnInDelay;
    public int maxDangerSpawn;
    public float spawnDelay;
    public Transform spawnArea;
    public Transform enemyParent;

    public List<Enemy> enemyBalls = new List<Enemy>();
    Dictionary<EnemyType, Enemy> otherEnemies = new Dictionary<EnemyType, Enemy>();
    public Sprite[] sprites;

    float spawnDelayCalculator;
    public static int dangerCount;

    public void LoadEnemies()
    {
        otherEnemies.Add(EnemyType.Danger, Resources.Load<Enemy>("Prefabs/Danger"));
        otherEnemies.Add(EnemyType.Safe, Resources.Load<Enemy>("Prefabs/Safe"));
        dangerCount = 0;
    }

    public void RefreshAll()
    {
        RefreshEnemies();
        RefreshEnemySpawning();
    }

    void RefreshEnemies()
    {
        for (int i = 0; i < enemyBalls.Count; i++)
            enemyBalls[i].RefreshUpdate();
    }

    void RefreshEnemySpawning()
    {
        spawnDelayCalculator += Time.deltaTime;
        if (spawnDelayCalculator >= spawnDelay)
        {
            SpawnNewEnemy();
            spawnDelayCalculator = 0;
        }
    }

    public void SpawnEnemiesOnStart()
    {
        for (int i = 0; i < numOfEnemyToSpawnAtStart; i++)
        {
            Enemy e = GameObject.Instantiate<Enemy>(otherEnemies[EnemyType.Safe]);
            Vector2 randomPos = new Vector2(Random.Range(-spawnArea.localScale.x / 2, spawnArea.localScale.x / 2), spawnArea.position.y);
            e.transform.position = randomPos;
            e.transform.SetParent(enemyParent);
            e.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            e.Initialize(false);
            e.SetSprite(sprites[GetRandomSpriteId()]);
            e.AddForceTowardsTarget();

            enemyBalls.Add(e);
        }
    }

    void SpawnNewEnemy()
    {
        for (int i = 0; i < numOfEnemyToSpawnAtStart; i++)
        {
            Enemy e;

            if (GetRandomInteger(0, 2) == 0)
            {
                e = GameObject.Instantiate<Enemy>(otherEnemies[EnemyType.Safe]);
                e.Initialize(false);
                e.SetSprite(sprites[GetRandomSpriteId()]);
            }
            else
            {
                e = GameObject.Instantiate<Enemy>(otherEnemies[EnemyType.Danger]);
                e.Initialize(true);
                e.SetSpriteColor(0);
                dangerCount++;
            }

            Vector2 randomPos = new Vector2(Random.Range(-spawnArea.localScale.x / 2, spawnArea.localScale.x / 2), spawnArea.position.y);
            e.transform.position = randomPos;
            e.transform.SetParent(enemyParent);
            e.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));

            e.AddForceTowardsTarget();

            enemyBalls.Add(e);
        }
    }

    public void RemoveEnemy(Enemy e)
    {
        enemyBalls.Remove(e);
    }

    int GetRandomSpriteId()
    {
        return Random.Range(0, sprites.Length - 1);
    }

    int GetRandomInteger(int start, int end)
    {
        int randomNumber = Random.Range(start, end);

        return dangerCount >= maxDangerSpawn ? 0 : randomNumber;
    }
}
