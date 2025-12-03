using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using TMPro;

public class ItemSelectManager : MonoBehaviour
{
    public static ItemSelectManager Instance; // 어디서든 부르기 쉽게 

    [Header("연결 필요")]
    public GameObject selectPanel;   // UI 패널 (ItemSelectPanel)

    [Header("연결하지 않아도 됨")]
    public Inventory playerInventory; // 아이템을 넣어줄 인벤토리 (Player 연결)

    [Header("왼쪽 선택지 UI")]
    public Button leftButton;
    public Image leftIcon;
    public TMP_Text leftName;
    public TMP_Text leftDesc;

    [Header("오른쪽 선택지 UI")]
    public Button rightButton;
    public Image rightIcon;
    public TMP_Text rightName;
    public TMP_Text rightDesc;

    [Header("전체 아이템 데이터베이스")]
    public List<ItemData> allItems;  

    private ItemData currentLeftItem;
    private ItemData currentRightItem;

    void Awake()
    {
        Instance = this;
        selectPanel.SetActive(false); // 시작할 땐 끔
    }

    public void ShowItemSelection()
    {
        // 플레이어 인벤토리 인스펙터에 연결되지 않았다면
        if (playerInventory == null)
        {
            // Tag로 플레이어를 찾아옴
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerInventory = player.GetComponent<Inventory>();
            }
            else
            {
                Debug.LogError("'Player' Tag를 가진 오브젝트를 찾을 수 없음");
                return; // 플레이어가 있어야 기능 실행 가능
            }
        }

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
    void UpdateUI(Image icon, TMP_Text name, TMP_Text desc, ItemData data)
    {
        if (data.icon != null) icon.sprite = data.icon;
        name.text = data.itemName;
        desc.text = data.description; // 설명이 있다면 표시
    }

    // 아이템 선택 시
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