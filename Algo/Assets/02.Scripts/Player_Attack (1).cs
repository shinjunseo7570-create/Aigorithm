using System.Collections;
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 스윙 중이 아닐 때 들어온 충돌은 무시
        if (!isSwinging) return;

        // Enemy 태그인 애 처리
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy == null) return;

            // 플레이어 공격력으로 데미지 굴리기
            float damage = stats != null ? stats.RollDamage() : 10f;
            enemy.TakeDamage(damage);

            Debug.Log($"[Player_Attack] Enemy hit! damage = {damage}");
        }

        // Banshee 태그인 애 처리
        if (collision.CompareTag("Banshee"))
        {
            Enemy10 Banshee = collision.GetComponent<Enemy10>();
            if (Banshee == null) return;

            // 플레이어 공격력으로 데미지 굴리기
            float damage = stats != null ? stats.RollDamage() : 10f;
            Banshee.TakeDamage(damage);

            Debug.Log($"[Player_Attack] Banshee hit! damage = {damage}");
        }

        // Boss10인 태그인 애 처리
        if (collision.CompareTag("Clone"))
        {
            EnemyClone Boss10 = collision.GetComponent<EnemyClone>();
            if (Boss10 == null) return;

            // 플레이어 공격력으로 데미지 굴리기
            float damage = stats != null ? stats.RollDamage() : 10f;
            Boss10.TakeDamage(damage);

            Debug.Log($"[Player_Attack] EnemyClone hit! damage = {damage}");
        }
    }

}
