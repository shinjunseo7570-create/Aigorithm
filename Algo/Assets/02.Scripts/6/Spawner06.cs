using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner06 : MonoBehaviour
{
    public Transform[] spawnPoint;
    public RoundData06[] rounds;

    public PoolManager poolManager;
    public float limitTime = 10f;
    public float nowTime = 0;

    int currentRound = 0;

    float timer;
    public float spawnDelay = 2f;
    public bool roundEnd;
    int spawnedCount = 0;
    int aliveCount = 0;

    bool isSpawning = true;
    bool usingScene = true;

    void Awake()
    {
        roundEnd = false;
        Transform[] points = GetComponentsInChildren<Transform>();

        spawnPoint = new Transform[points.Length - 1];

        for (int i = 1; i < points.Length; i++)
        {
            spawnPoint[i - 1] = points[i];
        }
    }

    void OnEnable()
    {
        Enemy06.OnEnemyDead += HandleEnemyDead;
    }

    void OnDisable()
    {
        Enemy06.OnEnemyDead -= HandleEnemyDead;
    }

    void HandleEnemyDead(Enemy06 enemy)
    {
        aliveCount--;


        if (aliveCount <= 0)
        {
            currentRound++;
            ResetRoundState();
        }
    }

    void ResetRoundState()
    {
        spawnedCount = 0;
        isSpawning = true;
        timer = 0f;
    }

    void Update()
    {
        nowTime += Time.deltaTime;
        if (!(roundEnd))
        {
            if (nowTime > limitTime && !(roundEnd))
            {
                roundEnd = true;
                Fail();
            }



            if (rounds == null)
            {
                return;
            }
            if (currentRound >= rounds.Length && !(roundEnd))
            {
                roundEnd = true;
                Ending();
                return;
            }

            timer += Time.deltaTime;

            RoundData06 round = rounds[currentRound];

            if (round == null)
            {
                currentRound++;
                return;
            }
            // 1) 몹 소환 중
            if (isSpawning)
            {
                if (spawnedCount >= round.mobCount)
                {
                    isSpawning = false;
                    return;
                }

                if (timer >= spawnDelay)
                {
                    timer = 0f;
                    SpawnMob(round);
                }
            }
        }
        else if (nowTime >= limitTime + 10f && usingScene)
        {
            usingScene = false;
            LoadingSceneManager.LoadScene("Main");
        }
    }

    void SpawnMob(RoundData06 round)
    {
        GameObject enemyObj = poolManager.Get(round.mobSpawnData.spriteType);

        Enemy06 enemy = enemyObj.GetComponent<Enemy06>();

        int rand = Random.Range(0, spawnPoint.Length);

        Vector3 pos = spawnPoint[rand].position;

        enemyObj.transform.position = pos;

        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            Vector3 playerPos = GameManager.instance.player.transform.position;

            float minDistance = 2f;
            if (Vector3.Distance(pos, playerPos) < minDistance)
                return; 
        }






        enemy.isBoss = false;


        enemy.Init(round.mobSpawnData);

        aliveCount++;
        spawnedCount++;


        
    }


    void Ending()
    {
        Debug.Log($"Game Clear!");
        return;
    }
    void Fail()
    {
        PlayerInteract.stemina -= 10;
        Debug.Log($"Game Over...." + PlayerInteract.stemina);
        return;
    }
}



[System.Serializable]
public class RoundData06
{
    public SpawnData mobSpawnData;
    public int mobCount = 1;
}

