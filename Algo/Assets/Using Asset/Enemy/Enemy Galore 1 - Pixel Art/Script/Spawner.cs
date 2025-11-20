using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform[] spawnPoint;
    public RoundData[] rounds;

    public PoolManager poolManager;

    int currentRound = 0;

    float timer;
    public float spawnDelay = 2f;

    int spawnedCount = 0;
    int aliveCount = 0;

    bool bossSpawned = false;
    bool isSpawning = true;

    void Awake()
    {
        Transform[] points = GetComponentsInChildren<Transform>();

        spawnPoint = new Transform[points.Length - 1];

        for (int i = 1; i < points.Length; i++)
        {
            spawnPoint[i - 1] = points[i];
        }
    }

    void OnEnable()
    {
        Enemy.OnEnemyDead += HandleEnemyDead;
    }

    void OnDisable()
    {
        Enemy.OnEnemyDead -= HandleEnemyDead;
    }

    void HandleEnemyDead(Enemy enemy)
    {
        aliveCount--;


        if (aliveCount <= 0 && bossSpawned)
        {
            currentRound++;
            ResetRoundState();
        }
    }

    void ResetRoundState()
    {
        spawnedCount = 0;
        bossSpawned = false;
        isSpawning = true;
        timer = 0f;
    }

    void Update()
    {
        if(rounds == null)
        {
            return;
        }
        if (currentRound >= rounds.Length)
        {
            Ending();
            return;
        }

        timer += Time.deltaTime;

        RoundData round = rounds[currentRound];

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
        // 2) 몹 다 소환했고, 아직 보스 안 나왔을 때
        else if (!bossSpawned)
        {
            if (aliveCount <= 0)
            {
                SpawnBoss(round);
            }
        }
    }

    void SpawnMob(RoundData round)
    {
        if (poolManager == null)
        {
            Debug.LogError("PoolManager가 Spawner 인스펙터에 할당되지 않았습니다!");
            return;
        }
        if (round.mobSpawnData == null)
        {
            Debug.LogError($"Round {currentRound}의 mobSpawnData가 null입니다!");
            return;
        }
        GameObject enemyObj = poolManager.Get(round.mobSpawnData.spriteType);
        if (enemyObj == null)
        {
            Debug.LogError($"PoolManager.Get({round.mobSpawnData.spriteType})에서 Null이 반환되었습니다. 풀링 설정 확인이 필요합니다.");
            return;
        }
        Enemy enemy = enemyObj.GetComponent<Enemy>();
        if (enemy == null)
        {
            Debug.LogError("스폰된 객체에 Enemy 컴포넌트가 없습니다!");
            return;
        }

        int rand = Random.Range(0, spawnPoint.Length);
        if (spawnPoint == null || spawnPoint.Length == 0)
        {
            Debug.LogError("SpawnPoint가 할당되지 않았거나 자식 Transform이 없습니다.");
            return;
        }
        if (spawnPoint[rand] == null)
        {
            Debug.LogError($"SpawnPoint[{rand}] Transform이 null입니다. 자식 오브젝트를 확인하세요.");
            return;
        }
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance가 null입니다. 싱글톤 초기화 상태를 확인하세요.");
            // 이 경우 아래 로직을 건너뜁니다.
        }
        else if (GameManager.instance.player == null)
        {
            Debug.LogWarning("GameManager.instance.player가 null입니다. 플레이어 객체 할당을 확인하세요.");
            // 이 경우에도 아래 로직을 건너뜁니다.
        }
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






        enemy.isBoss = false;

        
        enemy.Init(round.mobSpawnData);

        aliveCount++;
        spawnedCount++;

        
        //Debug.Log($"[SpawnMob] Round {currentRound}, spriteType = {round.mobSpawnData.spriteType}");
    }

    void SpawnBoss(RoundData round)
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance가 null입니다. 보스 소환 실패.");
            return;
        }
        if (GameManager.instance.pool == null)
        {
            Debug.LogError("GameManager.instance.pool이 null입니다. 풀링 초기화 상태를 확인하세요.");
            return;
        }
        GameObject boss = GameManager.instance.pool.Get(0);
        if (boss == null)
        {
            Debug.LogError($"GameManager.instance.pool.Get(0)에서 Null이 반환되었습니다. 보스 풀링 설정 확인이 필요합니다.");
            return;
        }
        Vector3 pos = new Vector3(0f, 0f, 0f);
        boss.transform.position = pos;

        Enemy enemyComp = boss.GetComponent<Enemy>();
        if (enemyComp == null)
        {
            Debug.LogError("스폰된 보스 객체에 Enemy 컴포넌트가 없습니다!");
            return;
        }
        enemyComp.isBoss = true;
        enemyComp.Init(round.bossSpawnData);

        aliveCount++;
        bossSpawned = true;

        // Debug.Log($"[SpawnBoss] Round {currentRound}, spriteType = {round.bossSpawnData.spriteType}");
    }

    void Ending()
    {
        Debug.Log($"Game Clear!");
        return;
    }
}

[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public int Health;
    public int Speed;
    public float Range; // 사정거리
}

[System.Serializable]
public class RoundData
{
    public SpawnData mobSpawnData;
    public int mobCount = 100;
    public SpawnData bossSpawnData;
}

