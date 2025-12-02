using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    PlayerStats stats;

    [Header("정보")]
    public Transform playerPivot;   // 플레이어 중심 (슬라임 Pivot)
    public LayerMask enemyLayer;    // 몹이 속한 레이어

    [Header("기본 공격")]
    public float radius = 1f;       // 칼이 도는 거리
    public float swingAngle = 45f;  // 휘두르는 각도
    public float swingTime = 0.15f; // 휘두르는 시간 (공속 건들꺼면 이거 건들면 됨)
    public Collider2D weaponCollider;

    

    bool isSwinging = false;
    float currentAngle = 0f;

    void Start()
    {
        stats = GetComponent<PlayerStats>();

        if(weaponCollider != null)
        {
            weaponCollider.enabled = false;
        }
    }

    void Update()
    {
        if(!isSwinging)
        {
            AimToMouse();
        }

        if(Input.GetMouseButtonDown(0) && !isSwinging)
        {
            StartCoroutine(Swing());
        }
    }

    void AimToMouse()
    {
        // 마우스 위치 (월드 기준)
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;

        // 슬라임 중심 → 마우스 방향
        Vector3 dir = (mousePos - playerPivot.position).normalized;

        // 칼이 원을 그리며 배치될 위치
        transform.position = playerPivot.position + dir * radius;

        // 칼이 마우스를 바라보도록 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        currentAngle = angle - 90f;

        transform.rotation = Quaternion.Euler(0, 0, currentAngle);
        // 칼 스프라이트가 위로 향해 있으니까 -90 조정
    }


    public void EnableWeapon(bool enable)
    {
        if(weaponCollider != null)
        {
            weaponCollider.enabled = enable;
        }
    }



    IEnumerator Swing()
    {
        isSwinging = true;

        float duration = swingTime;
        if (stats != null)
        {
            duration = stats.GetSwingDuration(swingTime);
        }

        if (weaponCollider != null)
        {
            weaponCollider.enabled = true;
        }

        

        float start = currentAngle - swingAngle;
        float end = currentAngle + swingAngle;
        float t = 0f;

        while ( t < 1f)
        {
            t += Time.deltaTime / duration;

            float angle = Mathf.Lerp(start, end, t);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            Vector3 dir = new Vector3(
                Mathf.Cos((angle + 90f) * Mathf.Deg2Rad),
                Mathf.Sin((angle + 90f) * Mathf.Deg2Rad),
                0
                );

            transform.position = playerPivot.position + dir * radius;

            yield return null;


        }

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;

        }

        isSwinging = false;
    }
    private void OnDisable()
    {
        isSwinging = false;

        if (weaponCollider != null)
        {
            weaponCollider.enabled = false;

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isSwinging) return;
        if (!collision.CompareTag("Enemy")) return;

        float damage = stats != null ? stats.RollDamage() : 10f;

        if (stats == null)
        {
            Debug.LogWarning("PlayerStats가 연결되지 않아 흡혈 로직을 수행할 수 없습니다.");
        }

        // 1) Enemy
        Enemy enemy = collision.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] Enemy hit! damage = {damage}");
            return;
        }

        Enemy06 enemy06 = collision.GetComponent<Enemy06>();
        if (enemy06 != null)
        {
            enemy06.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] Enemy06 hit! damage = {damage}");
            return;
        }

        // 2) Enemy07
        Enemy07 enemy07 = collision.GetComponent<Enemy07>();
        if (enemy07 != null)
        {
            enemy07.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] Enemy07 hit! damage = {damage}");
            return;
        }

        // 3) Mimic
        Mimic mimic = collision.GetComponent<Mimic>();
        if (mimic != null)
        {
            mimic.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] Mimic hit! damage = {damage}");
            return;
        }


        // 4) Enemy10(Banshee)
        Enemy10 Banshee = collision.GetComponent<Enemy10>();
        if (Banshee != null)
        {
            Banshee.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] Banshee hit! damage = {damage}");
            return;
        }

        // 5) EnemyClone
        EnemyClone enemyClone = collision.GetComponent<EnemyClone>();
        if (enemyClone != null)
        {
            enemyClone.TakeDamage(damage);
            if (stats != null) stats.OnAttackHit();
            Debug.Log($"[Player_Attack] enemyClone hit! damage = {damage}");
            return;
        }

        // 6) Enemy03
        Pot pot = collision.GetComponent<Pot>();
        if (pot != null)
        {
            pot.TakeDamage(damage);
            Debug.Log($"[Player_Attack] Enemy07 hit! damage = {damage}");
            return;
        }
    }

}
