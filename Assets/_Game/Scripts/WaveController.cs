using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class WaveController : MonoBehaviour
{
    [SerializeField] private List<GameObject> spawnPoints;
    [SerializeField] private List<Wave> waves;
    public UnityAction OnLevelClear;
    private List<GameObject> enemies = new ();
    private float timeToWait = 3f;
    private float waitTimer;
    private bool wait => waitTimer > 0;
    private bool waveOver => enemies.Count == 0;
    private bool spawn;
    private bool stop = false;
    void Start()
    {
        OnLevelClear += GameManager.Instance.NextLevel;
    }
    void Update()
    {
        if (stop)
            return;
        if(wait)
            Wait();
        else if (spawn)
            SpawnEnemies();
        else if(waveOver)
            NextWave();
    }

    private void NextWave()
    {
        if (waves.Count == 0)
        {
            LevelCleared();
            return;
        }

        waitTimer = timeToWait;
        spawn = true;

    }

    private void Wait()
    {
        waitTimer -= Time.deltaTime;
    }
    private void LevelCleared()
    {
        OnLevelClear?.Invoke();
        stop = true;
    }

    private void SpawnEnemies()
    {
        var curWave = waves[0].EnemiesToSpawn;
        foreach (var enemyType in curWave)
        {
            var enemyGo = GameManager.Instance.GetEnemyGo(enemyType.enemy);
            for (var i = 0; i < enemyType.amount; i++)
            {
                var enemy = Instantiate(enemyGo);
                enemy.transform.position = GetRandomSpawnPoint();
                enemy.GetComponent<CharacterBase>().OnDeath += KillEnemy;
                enemies.Add(enemy);
            }
        }
        waves.RemoveAt(0);
        spawn = false;
    }

    private void KillEnemy(GameObject go)
    {
        var enemyCtrl = go.GetComponent<CharacterBase>();
        enemyCtrl.OnDeath -= KillEnemy;
        enemies.Remove(go);
        enemyCtrl.Die();
    }
    public Vector3 GetRandomSpawnPoint()
    {
        int randomNum = Random.Range(0, spawnPoints.Count);
        return spawnPoints[randomNum].transform.position;
    }
}

[Serializable]
public class Wave
{
    public List<EnemyAmount> EnemiesToSpawn;
}

[Serializable]
public class EnemyAmount
{
    public EnemyName enemy;
    public int amount;
}
