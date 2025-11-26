using UnityEngine;

// 비용 종류 (최대 체력, 현재 체력, 스테미나)
public enum CostType { MaxHP, CurrentHP, Stamina }

// 보상 종류 (스탯, 아이템, 스테미나 회복, 최대 체력 증가 등)
public enum RewardType { StatUp, RestoreStamina, GetItem, MaxHPUp }

//Editor에 이 데이터를 만드는 버튼을 추가한다.
[CreateAssetMenu(fileName = "New Devil Deal", menuName = "Devil Shop/Deal Item")]

public class ShopItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    [TextArea] public string description;

    [Header("대가 (Cost)")]
    public CostType costType;
    public float costAmount; // 스테미나는 소수점이 있을 수 있으므로 float 사용

    [Header("보상 (Reward)")]
    public RewardType rewardType;
    public float rewardAmount; // 증가량
    // 아이템 획득의 경우 아래에 GameObject나 Item ID 등을 추가 가능
}