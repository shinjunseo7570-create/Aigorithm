using UnityEngine;
using System.Collections.Generic;

public enum ItemEffectType
{
    HealthUp,
    AttackPowerUp,
    CritRateUp,
    StaminaGuard,
    AttackSpeedUp,
    TimeExtension,
    MoveSpeedUp,
    ReviveOnce,
    StunResist,
    TreasureRateUp,
    HealOnAttack
}
public enum ItemGrade
{
    C,
    B,
    A,
    S
}
[System.Serializable] // 인스펙터에 보이게 하기 위해 필수
public class ItemEffect
{
    public ItemEffectType effectType;
    public float amount;
}

[CreateAssetMenu(fileName = "New Item", menuName = "Game/Item Data")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemGrade grade;
    
    [TextArea]
    public string description;

    [Header("효과 목록")]
    public List<ItemEffect> effects;
}