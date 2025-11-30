using UnityEngine;

public class PlayerSkillController : MonoBehaviour
{
    [Header("발사 위치 설정")]
    public Transform Balsa;

    [Header("스킬 데이터")]
    public AttackModeData skillData;

    private float nextActionTime = 0f;

    void Update()
    {
        // E를 누르면 스킬 발사
        if (Input.GetKeyDown(KeyCode.E))
        {
            PerformSkill();
        }
    }

    private void PerformSkill()
    {
        if (Time.time < nextActionTime) return;
        if (skillData == null) return;

        
        Vector3 spawnPosition = (Balsa != null) ? Balsa.position : transform.position;
        spawnPosition.z = 0f;

        
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        Vector3 direction = (mouseWorldPos - spawnPosition).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion spawnRotation = Quaternion.Euler(0, 0, angle);

        
        GameObject projectile = Instantiate(skillData.assetPrefab, spawnPosition, spawnRotation);

        
        SkillController skillScript = projectile.GetComponent<SkillController>();
        if (skillScript != null)
        {
            skillScript.Init(skillData.projectileSpeed, skillData.range, skillData.damage);
        }

        
        nextActionTime = Time.time + skillData.attackInterval;
    }
}
