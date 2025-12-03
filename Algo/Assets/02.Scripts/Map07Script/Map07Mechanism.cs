using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map07Mechanism : MonoBehaviour
{
    public Transform[] spawnPoint;
    public RoundData07[] rounds;

    public PoolManager07 poolManager;
    public Enemy07 enemyScript;
    public Mimic mimicScript;
    GameObject potal;

    int currentRound = 0;

    float timer;
    public float spawnDelay = 0f;
    public bool roundEnd;
    int spawnedCount = 0;
    int aliveCount = 0;
    bool isSpawning = true;
    bool usingScene = true;
    bool mimicSpawned;

    void Awake()
    {
        roundEnd = false;
        Transform[] points = GetComponentsInChildren<Transform>();
        mimicSpawned = false;
        potal = GameObject.FindWithTag("Potal");
        potal.SetActive(false);
        spawnPoint = new Transform[points.Length - 1];

        for (int i = 1; i < points.Length; i++)
        {
            spawnPoint[i - 1] = points[i];
        }
    }

    void OnEnable()
    {
        Enemy07.OnEnemyDead += HandleEnemyDead;
    }

    void OnDisable()
    {
        Enemy07.OnEnemyDead -= HandleEnemyDead;
    }

    void HandleEnemyDead(Enemy07 enemy07)
    {
        aliveCount--;
    }

  

    void Update()
    {
        if (!(roundEnd))
        {
            
            if (rounds == null)
            {
                return;
            }
            if ( spawnedCount!=0 && aliveCount == 0 && !(roundEnd))
            {
                roundEnd = true;
                Ending();
                return;
            }
            RoundData07 round = rounds[currentRound];
            timer += Time.deltaTime;

            if (round == null)
            {
                currentRound++; // 다음 라운드로 건너뛰기
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
            if (!mimicSpawned)
            {
                mimicSpawned = true;
                SpawnMimic(round);
            }
        }/*else if("플레이어가 이 맵을 나가는 방법을 정한 후 그 행위를 실행했다면")
        {
            usingScene = false;
            LoadingSceneManager.LoadScene("Main");
        }*/
    }

    void SpawnMob(RoundData07 round)
    {
        GameObject enemyObj = poolManager.Get(round.mob1SpawnData.spriteType);
        Enemy07 enemy07 = enemyObj.GetComponent<Enemy07>();
        mimicScript = enemyObj.GetComponent<Mimic>();
        Destroy(mimicScript);

        int rand = Random.Range(6, spawnPoint.Length);
        
        Vector3 pos = spawnPoint[rand].position;

        pos.x = Mathf.Clamp(pos.x, -8f, 8f);
        pos.y = Mathf.Clamp(pos.y, -4f, 4f);
        enemyObj.transform.position = pos;

        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            Vector3 playerPos = GameManager.instance.player.transform.position;

            float minDistance = 2f;
            if (Vector3.Distance(pos, playerPos) < minDistance)
                return; // 몬스터 소환 취소 (여전히 spawnedCount는 증가하지 않음)
        }

        enemy07.Init(round.mob1SpawnData);
        spawnedCount++;
        aliveCount++;
        
        //Debug.Log($"[SpawnMob] Round {currentRound}, spriteType = {round.mobSpawnData.spriteType}");
    }
    void SpawnMimic(RoundData07 round)
    {
        for(int i = 0; i < 6; i++)
        {
            GameObject mimicObj = poolManager.Get(round.mob2SpawnData.spriteType);
            Mimic mimic = mimicObj.GetComponent<Mimic>();
            enemyScript = mimicObj.GetComponent<Enemy07>();
            Destroy(enemyScript);
            mimicObj.transform.localScale = new Vector3(-1, 1, 1);
            Vector3 pos = spawnPoint[i].position;
            pos.x = Mathf.Clamp(pos.x, -8f, 8f);
            pos.y = Mathf.Clamp(pos.y, -4f, 4f);
            mimicObj.transform.position = pos;
            mimic.Init(round.mob2SpawnData);
            mimic.SetPosition(pos);
        }
    }

    void Ending()
    {
        Debug.Log($"Game Clear!");
        potal.SetActive(true);
        return;
    }
}

[System.Serializable]
public class SpawnData07
{
    public int spriteType;
    public int Health;
    public int Speed;
    public float Range; // 사정거리
    public float ATK;
}

[System.Serializable]
public class RoundData07
{
    public SpawnData07 mob1SpawnData;
    public SpawnData07 mob2SpawnData;
    public int mobCount;
}