using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("HP")]
    public int maxHealth = 5;
    public int health = 5;
    public bool hasRevive = false; // [아이템] 부활권 보유 여부

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

    [Header("Special Stats")] // [아이템] 특수 스탯 추가
    public float stunDurationMultiplier = 1f; // 1.0 = 100%, 0.7 = 70% (30% 감소)
    public float treasureChance = 0f;         // 보물 확률 추가
    public float healOnAttackChance = 0f;// 생흡

    public static PlayerStats instance;
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- [핵심] 아이템 획득 시 호출 ---
    public void ApplyItem(ItemData item)
    {
        Debug.Log($"아이템 획득: {item.itemName}");

        // 아이템에 들어있는 모든 효과를 하나씩 꺼내서 적용
        foreach (ItemEffect effect in item.effects)
        {
            ProcessEffect(effect);
        }
    }
    private void ProcessEffect(ItemEffect effect)
    {
        switch (effect.effectType)
        {
            case ItemEffectType.HealthUp:
                int hpAmount = (int)effect.amount;
                maxHealth += hpAmount;
                health += hpAmount;
                Debug.Log($" - 체력 {hpAmount} 증가");
                break;

            case ItemEffectType.AttackPowerUp:
                attackPower += effect.amount;
                Debug.Log($" - 공격력 {effect.amount} 증가");
                break;

            case ItemEffectType.CritRateUp:
                critRate += effect.amount;
                Debug.Log($" - 치명타율 {effect.amount}% 증가");
                break;

            case ItemEffectType.StaminaGuard:
                hasStaminaGuard = true;
                Debug.Log(" - 스테미나 방어권 획득");
                break;

            case ItemEffectType.AttackSpeedUp:
                attackSpeed += effect.amount;
                if (attackSpeed > maxAttackSpeed) attackSpeed = maxAttackSpeed;
                Debug.Log($" - 공격속도 {effect.amount} 증가");
                break;

            case ItemEffectType.MoveSpeedUp:
                moveSpeed += effect.amount;
                Debug.Log($" - 이동속도 {effect.amount} 증가");
                break;

            case ItemEffectType.ReviveOnce:
                hasRevive = true;
                Debug.Log(" - 부활 아이템 획득");
                break;

            case ItemEffectType.StunResist:
                stunDurationMultiplier -= (effect.amount / 100f);
                if (stunDurationMultiplier < 0) stunDurationMultiplier = 0;
                Debug.Log($" - 스턴 저항 {effect.amount}% 증가");
                break;

            case ItemEffectType.TreasureRateUp:
                treasureChance += effect.amount;
                Debug.Log($" - 보물 확률 {effect.amount}% 증가");
                break;

            case ItemEffectType.TimeExtension:
                // GameManager.Instance.AddTime(effect.amount);
                Debug.Log($" - 시간 {effect.amount}초 연장");
                break;
        }
    }

    public void OnAttackHit()
    {
        // 확률이 0보다 클 때만 계산
        if (healOnAttackChance > 0)
        {
            // 0 ~ 100 사이 랜덤 숫자가 확률보다 낮으면 성공
            if (Random.Range(0f, 100f) < healOnAttackChance)
            {
                Heal(1); // 체력 1 회복
                Debug.Log("흡혈 발동! 체력 1 회복");
            }
        }
    }

    // 체력 회복 전용 함수 (깔끔한 관리를 위해 분리 추천)
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth) health = maxHealth; // 최대 체력 초과 방지
    }

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

    public void TakeDamage(float amount)
    {
        // 들어오는 데미지(float)를 정수(int)로 반올림 변환
        int finalDamage = Mathf.RoundToInt(amount);

        health -= finalDamage;
        Debug.Log($"피격! 데미지: {finalDamage}, 남은 체력: {health}");

        if (health <= 0)
        {
            if (hasRevive)
            {
                hasRevive = false;
                health = 1; // 1의 체력으로 부활
                if (health < 1) health = 1; // 최소 1은 보장
                Debug.Log("사망 1회 저지!");
            }
            else
            {
                health = 0;
                Die();
            }
        }
    }

    

    public void Die()
    {
        Debug.Log("Player 사망, GameOver");
    }
}

