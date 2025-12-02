using UnityEngine;
using UnityEngine.UI; // UI 사용
using System.Collections.Generic;

public class ItemSelectManager : MonoBehaviour
{
    public static ItemSelectManager Instance; // 어디서든 부르기 쉽게 싱글톤 처리

    [Header("연결 필요")]
    public GameObject selectPanel;   // UI 패널 (ItemSelectPanel)
    public Inventory playerInventory; // 아이템을 넣어줄 인벤토리 (Player 연결)

    [Header("왼쪽 선택지 UI")]
    public Button leftButton;
    public Image leftIcon;
    public Text leftName;
    public Text leftDesc;

    [Header("오른쪽 선택지 UI")]
    public Button rightButton;
    public Image rightIcon;
    public Text rightName;
    public Text rightDesc;

    [Header("전체 아이템 데이터베이스")]
    public List<ItemData> allItems;  // [중요] 게임에 존재하는 모든 아이템을 여기에 드래그!

    private ItemData currentLeftItem;
    private ItemData currentRightItem;

    void Awake()
    {
        Instance = this;
        selectPanel.SetActive(false); // 시작할 땐 끔
    }

    // --- [외부에서 호출] 선택창 띄우기 ---
    public void ShowItemSelection()
    {
        if (allItems.Count < 2)
        {
            Debug.LogError("아이템 데이터가 최소 2개 이상 필요합니다!");
            return;
        }

        // 1. 게임 일시 정지
        Time.timeScale = 0f;
        selectPanel.SetActive(true);

        // 2. 랜덤으로 2개 뽑기 (중복 방지)
        int index1 = Random.Range(0, allItems.Count);
        int index2 = Random.Range(0, allItems.Count);

        // 혹시 같은게 나오면 다를 때까지 다시 뽑기
        while (index1 == index2)
        {
            index2 = Random.Range(0, allItems.Count);
        }

        currentLeftItem = allItems[index1];
        currentRightItem = allItems[index2];

        // 3. UI 업데이트 (왼쪽)
        UpdateUI(leftIcon, leftName, leftDesc, currentLeftItem);
        // 4. UI 업데이트 (오른쪽)
        UpdateUI(rightIcon, rightName, rightDesc, currentRightItem);

        // 5. 버튼에 기능 연결 (기존 리스너 제거 후 다시 연결)
        leftButton.onClick.RemoveAllListeners();
        leftButton.onClick.AddListener(() => OnSelectItem(currentLeftItem));

        rightButton.onClick.RemoveAllListeners();
        rightButton.onClick.AddListener(() => OnSelectItem(currentRightItem));
    }

    // UI 갱신 헬퍼 함수
    void UpdateUI(Image icon, Text name, Text desc, ItemData data)
    {
        if (data.icon != null) icon.sprite = data.icon;
        name.text = data.itemName;
        desc.text = data.description; // 설명이 있다면 표시
    }

    // --- 아이템 선택 시 실행 ---
    void OnSelectItem(ItemData selectedItem)
    {
        // 1. 인벤토리에 추가 (스탯 적용 포함)
        playerInventory.AddItem(selectedItem);
        Debug.Log($"선택 완료: {selectedItem.itemName}");

        // 2. 패널 닫기 및 게임 재개
        selectPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}