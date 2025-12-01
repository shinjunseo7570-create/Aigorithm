using System.Collections;
using UnityEngine;

public class PlayerQSkillController : MonoBehaviour
{
    [Header("칼 오브젝트")]
    public GameObject defaultSword;   
    public GameObject fireSword;      

    [Header("칼 피벗 설정")]
    public Transform swordPivot;      
    public Collider2D fireSwordCollider;   

    [Header("Q 스킬 설정")]
    public int totalSwings = 4;
    public float swingAngle = 45f;
    public float swingTime = 0.15f;
    public float betweenSwingDelay = 0.05f;
    public float qCooldown = 3f;

    [Header("회전 보정")]
    public float angleOffset = 0f;

    [Header("기본 공격 스크립트")]
    public MonoBehaviour basicAttackScript;

    private PlayerStats stats;

    float lockedAngle = 0f;
    float nextAvailableTime = 0f;
    bool isQActive = false;

    void Start()
    {
        if (defaultSword != null) defaultSword.SetActive(true);
        if (fireSword != null) fireSword.SetActive(false);
        if (fireSwordCollider != null) fireSwordCollider.enabled = false;

        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            QSkill();
    }

    void QSkill()
    {
        if (isQActive) return;
        if (Time.time < nextAvailableTime) return;

        lockedAngle = GetMouseAngleFromPlayer();
        StartCoroutine(QRoutine());
        nextAvailableTime = Time.time + qCooldown;
    }

    float GetMouseAngleFromPlayer()
    {
        Vector3 playerPos = transform.position;
        Vector3 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 dir = (mouse - playerPos);
        dir.z = 0f;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        angle -= 90f;

        return angle;
    }

    IEnumerator QRoutine()
    {
        isQActive = true;

        if (basicAttackScript != null)
            basicAttackScript.enabled = false;

        defaultSword.SetActive(false);
        fireSword.SetActive(true);

        if (fireSwordCollider != null)
            fireSwordCollider.enabled = false;

        if (swordPivot != null)
            swordPivot.rotation = Quaternion.Euler(0f, 0f, lockedAngle + angleOffset);

        for (int i = 0; i < totalSwings; i++)
        {
            yield return StartCoroutine(SwingOnce());
            yield return new WaitForSeconds(betweenSwingDelay);
        }

        if (fireSwordCollider != null)
            fireSwordCollider.enabled = false;

        fireSword.SetActive(false);
        defaultSword.SetActive(true);

        if (basicAttackScript != null)
            basicAttackScript.enabled = true;

        isQActive = false;
    }

    IEnumerator SwingOnce()
    {
        if (swordPivot == null)
            yield break;

        if (fireSwordCollider != null)
            fireSwordCollider.enabled = true;

        float startAngle = lockedAngle - swingAngle;
        float endAngle = lockedAngle + swingAngle;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / swingTime;
            float cur = Mathf.Lerp(startAngle, endAngle, t);

            swordPivot.rotation = Quaternion.Euler(0f, 0f, cur + angleOffset);

            yield return null;
        }

        if (fireSwordCollider != null)
            fireSwordCollider.enabled = false;
    }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isQActive) return;

        
            if (!collision.CompareTag("Enemy")) return;

            if (stats == null) return;
            float damage = stats.RollDamage() * 2;   // 스킬이라 최종 뎀지에서 x2 했는데 밸런스 조정 필요하면 여기 건드리면 됨.

            // 1) Enemy
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Debug.Log($"[Player_Attack] Enemy hit(Q)! damage = {damage}");
                return;
            }
            // 2) Enemy07
            Enemy07 enemy07 = collision.GetComponent<Enemy07>();
            if (enemy07 != null)
            {
                enemy07.TakeDamage(damage);
                Debug.Log($"[Player_Attack] Enemy07 hit(Q)! damage = {damage}");
                return;
            }
            // 3) Mimic
            Mimic mimic = collision.GetComponent<Mimic>();
            if (mimic != null)
            {
                mimic.TakeDamage(damage);
                Debug.Log($"[Player_Attack] Mimic hit(Q)! damage = {damage}");
                return;
            }
            // 4) Enemy10(Banshee)
            Enemy10 Banshee = collision.GetComponent<Enemy10>();
            if (Banshee != null)
            {
                Banshee.TakeDamage(damage);
                Debug.Log($"[Player_Attack] Banshee hit(Q)! damage = {damage}");
                return;
            }
            // 5) EnemyClone
            EnemyClone enemyClone = collision.GetComponent<EnemyClone>();
            if (enemyClone != null)
            {
                enemyClone.TakeDamage(damage);
                Debug.Log($"[Player_Attack] enemyClone hit(Q)! damage = {damage}");
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
