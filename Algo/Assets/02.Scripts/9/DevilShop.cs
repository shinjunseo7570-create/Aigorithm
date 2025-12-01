using UnityEngine;
using TMPro;

public class DevilShop : MonoBehaviour
{
    public PlayerStatus player;
    [Header("연결")]
    public DialogueManager dialogueManager;
    public StartShopSceneManager startShopSceneManager;

    [Header("스탯 표시 텍스트")]
    public TextMeshProUGUI statusText;

    // 버튼에 이 함수를 연결하고, 해당 버튼이 파는 아이템 데이터(ScriptableObject)를 인자로 넣습니다.
    public void TryBuyItem(ShopItemData itemData)
    {
        if (CanAfford(itemData))
        {

            PayCost(itemData);
            GetReward(itemData);

            GameObject.Find("Character Image").transform.Find("Image 2").gameObject.SetActive(false);
            GameObject.Find("Character Image").transform.Find("Image 1").gameObject.SetActive(true);

            dialogueManager.ShowMessage("거래가 성사되었다...");
        }
        else
        {
            GameObject.Find("Character Image").transform.Find("Image 1").gameObject.SetActive(false);
            GameObject.Find("Character Image").transform.Find("Image 2").gameObject.SetActive(true);

            dialogueManager.ShowMessage("대가가 부족하군. 장난치는 건가?");
        }
    }

    // 구매 가능 여부 확인
    bool CanAfford(ShopItemData item)
    {
        switch (item.costType)
        {
            case CostType.MaxHP:
                // 최대 체력은 최소 1은 남겨야 한다면 > 조건 사용
                return player.maxHealth > (int)item.costAmount;

            case CostType.CurrentHP:
                return player.currentHealth > (int)item.costAmount;

            case CostType.Stamina:
                return player.currentStamina >= item.costAmount;
        }
        return false;
    }

    // 대가 지불 (Cost 차감)
    void PayCost(ShopItemData item)
    {
        switch (item.costType)
        {
            case CostType.MaxHP:
                // 음수로 변환하여 빼줌 (CostAmount가 양수라고 가정)
                player.ModifyMaxHealth(-(int)item.costAmount);
                break;

            case CostType.CurrentHP:
                player.ModifyCurrentHealth(-(int)item.costAmount);
                break;

            case CostType.Stamina:
                player.ModifyStamina(-item.costAmount);
                break;
        }
    }

    // 3. 보상 지급
    void GetReward(ShopItemData item)
    {
        switch (item.rewardType)
        {
            case RewardType.StatUp:
                player.IncreaseStat((int)item.rewardAmount);
                break;

            case RewardType.RestoreStamina:
                player.ModifyStamina(item.rewardAmount);
                break;

            case RewardType.MaxHPUp:
                player.ModifyMaxHealth((int)item.rewardAmount);
                break;

            case RewardType.GetItem:
                Debug.Log($"아이템 획득 로직 실행 (ID: {item.rewardAmount})");
                // 인벤토리 시스템 연동
                break;
        }
    }
}