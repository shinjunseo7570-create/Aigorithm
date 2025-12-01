using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public float maxHealth = 100f;
    public float health = 100f;

    [Header("Stamina")] // 스테미나 관련 변
    public int maxStamina = 100;
    public int currentStamina = 100;
    public bool hasStaminaGuard = false; // [아이템] 스테미나 감소 방지권

    [Header("공격력")]
    public float attackPower = 10f;  // 기본 10

    [Header("공격 속도")]
    public float attackSpeed = 10f;  // 기본 10
    public float maxAttackSpeed = 20f;
    public float baseAttackSpeed = 0.5f;

    [Header("이동 속도")]
    public float moveSpeed = 5f;     // 기본 5
    public float maxMoveSpeed = 30f;

    [Header("치명타")]
    public float critRate = 25f;    // 기본 25%
    public float critDamageMultiplier = 2f; // 2배 데미지

    [Header("공격 사거리")]
    public float attackRange = 1.2f;   // 고정

    public float GetSwingDuration(float baseSwingTime)
    {
        float clampedSpeed = Mathf.Clamp(attackSpeed, 1f, maxAttackSpeed);

        return baseSwingTime * (baseAttackSpeed / clampedSpeed);
    }

    public float RollDamage()
    {
        float overflow = Mathf.Max(0f, critRate - 100f);

        float effectiveCritRate = Mathf.Clamp(critRate, 0f, 100f);

        float effectiveAttackPower = attackPower + overflow;

        bool isCrit = Random.value * 100f < effectiveCritRate;
        float dmg = effectiveAttackPower;

        if(isCrit)
        {
            dmg *= critDamageMultiplier;
        }
        return dmg;
    }



    // 치명타 판정 함수
    public bool IsCritical()
    {
        // critChance가 100 넘으면 넘친 만큼 공격력으로 전환
        if (critRate > 100f)
        {
            float overflow = critRate - 100f;
            attackPower += overflow; // 공격력 증가
            critRate = 100f;       // 확률은 100에 고정
        }

        return Random.value * 100f < critRate;
    }

    // 데미지 계산
    public float CalculateDamage()
    {
        float dmg = attackPower;

        if (IsCritical())
        {
            dmg *= critDamageMultiplier;
        }

        return dmg;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0f, maxHealth);

        Debug.Log($"Player가 {amount}만큼 데미지 입음!");

        if(health < 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Player 사망, GameOver");
    }
}

