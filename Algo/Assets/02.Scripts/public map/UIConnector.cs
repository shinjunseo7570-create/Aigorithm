using UnityEngine;
using TMPro;

public class UIConnector : MonoBehaviour
{

    // 인벤토리, 스탯 창 UI를 플레이어와 연결해 줄 스크립트

    [Header("1. 인벤토리 UI 연결")]
    public GameObject inventoryPanel; // 인벤토리 패널
    public Transform inventorySlotParent; // 아이템 슬롯이 생성될 부모 (Grid Layout Group이 있는 Content 오브젝트)

    [Header("2. 스탯 UI 연결")]
    public GameObject statsPanel;
    public TextMeshProUGUI statusText;

    void Start()
    {
        // 안전장치
        if (inventoryPanel == null || statsPanel == null || statusText == null || inventorySlotParent == null)
        {
            Debug.LogError($"[UIConnector] Inspector 연결을 확인해주세요! (빈칸이 있습니다)");
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Inventory 연결
            var inventory = player.GetComponent<Inventory>();
            if (inventory != null)
            {
                // Inventory.cs의 'inventory' 변수에 -> 내 'inventoryPanel'을 넣음
                inventory.inventory = this.inventoryPanel;

                // Inventory.cs의 'slotParent' 변수에 -> 내 'inventorySlotParent'를 넣음
                inventory.slotParent = this.inventorySlotParent;
            }

            // StatsManager 연결
            var statsManager = player.GetComponent<StatsManager>();
            if (statsManager != null)
            {
                // StatsManager.cs의 'statsMenu' 변수에 -> 내 'statsPanel'을 넣음
                statsManager.statsMenu = this.statsPanel;

                // StatsManager.cs의 'statusText' 변수에 -> 내 'statusText'를 넣음
                statsManager.statusText = this.statusText;

                // 텍스트 갱신을 위해 코루틴 재실행
                statsManager.StopAllCoroutines(); // 혹시 모를 중복 방지
                statsManager.StartCoroutine("ShowStatusRoutine");
            }

            Debug.Log($"UI 연결 완료: {player.name}");
        }
    }
}