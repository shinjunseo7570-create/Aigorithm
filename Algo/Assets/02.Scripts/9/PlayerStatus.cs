using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 10;
    public int currentHealth;

    [Header("Stamina")]
    public float maxStamina = 100f;
    public float currentStamina;

    [Header("Stats")]
    public int strength = 10; // 예시 스탯

    void Start()
    {
        currentHealth = maxHealth;
        currentStamina = maxStamina;
    }

    // 최대 체력 영구 감소/증가
    public void ModifyMaxHealth(int amount)
    {
        maxHealth += amount;

        // 최대 체력이 줄어들 때 현재 체력도 같이 조정
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        // 최소 체력 1은 유지 (즉사 방지)
        if (maxHealth < 1) maxHealth = 1;

        Debug.Log($"최대 체력 변경: {maxHealth}");
    }

    // 현재 체력 소모/회복
    public void ModifyCurrentHealth(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        if (currentHealth < 0) currentHealth = 0; // 사망 로직 연결 필요
    }

    // 스테미나 소모/회복
    public void ModifyStamina(float amount)
    {
        currentStamina += amount;
        if (currentStamina > maxStamina) currentStamina = maxStamina;
        if (currentStamina < 0) currentStamina = 0;
    }

    // 스탯 영구 증가
    public void IncreaseStat(int amount)
    {
        strength += amount;
        Debug.Log($"힘 스탯 증가! 현재 힘: {strength}");
    }
}