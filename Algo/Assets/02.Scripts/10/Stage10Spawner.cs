using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage10Spawner : MonoBehaviour
{
    [Header("스폰 설정")]
    public Transform[] spawnPoint;

    [Header("프리팹 연결")]
    public GameObject bansheePrefab; // 벤시 (Enemy10) 프리팹
    public GameObject clonePrefab;   // 도플갱어 (EnemyClone) 프리팹

    [Header("밴시 설정")]
    public int initialBansheeCount = 3; // 처음에 미리 깔아둘 개수
    public SpawnData10 bansheeData;  // 벤시 스탯 (속도 등 설정을 위해 필요)
    public float spawnInterval = 2f; // 벤시 스폰 딜레이

    float timer = 0f;

    void Awake()
    {
        Transform[] points = GetComponentsInChildren<Transform>();

        spawnPoint = new Transform[points.Length - 1];

        // 포인트가 1개 이상일 경우에만
        if (points.Length > 1)
        {
            spawnPoint = new Transform[points.Length - 1];
            for (int i = 1; i < points.Length; i++)
            {
                spawnPoint[i - 1] = points[i];
            }
        }
    }
    void Start()
    {
        // 1. 보스 소환 (한 번만)
        SpawnBoss();

        // 2. 처음에 기본으로 몇 마리 깔아두기
        for (int i = 0; i < initialBansheeCount; i++)
        {
            SpawnRandomBanshee();
        }
    }
    void Update()
    {

        // 스포너가 플레이어 위치를 계속 따라다님 (중심축 맞추기)
        if (GameManager10.instance != null && GameManager10.instance.player != null)
        {
            transform.position = GameManager10.instance.player.transform.position;
        }

        // ★ 핵심: 시간이 흐르면 벤시를 하나씩 계속 소환
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f; // 타이머 초기화
            SpawnRandomBanshee(); // 벤시 한 마리 소환
        }
    }
    // 벤시 한 마리를 랜덤한 위치에 소환하는 함수
    void SpawnRandomBanshee()
    {
        if (bansheePrefab == null || spawnPoint.Length == 0) return;

        // 1. 랜덤 위치 선정
        int rand = Random.Range(0, spawnPoint.Length);
        Vector3 pos = spawnPoint[rand].position;

        // 2. 맵 밖으로 안 나가게 보정
        pos.x = Mathf.Clamp(pos.x, -8f, 8f);
        pos.y = Mathf.Clamp(pos.y, -4f, 4f);

        // 3. 생성
        GameObject bansheeObj = Instantiate(bansheePrefab, pos, Quaternion.identity);

        // 4. 데이터 초기화
        Enemy10 enemyScript = bansheeObj.GetComponent<Enemy10>();
        if (enemyScript != null)
        {
            enemyScript.isBoss = false;
            enemyScript.Init(bansheeData);
        }
    }

    void SpawnBoss()
    {
        if (clonePrefab == null) return;

        Vector3 pos = Vector3.zero;
        if (spawnPoint.Length > 0) pos = spawnPoint[0].position;

        Instantiate(clonePrefab, pos, Quaternion.identity);
    }

}

[System.Serializable]
public class SpawnData10
{
    public int spriteType;
    public int Health;
    public int Speed;
    public float Range; // 사정거리
}