using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // UI 건드려야 하니 필수

public class Inventory : MonoBehaviour
{
    [Header("연결 정보")]
    public PlayerStats playerStats;  // 스탯 적용을 위해 필요
    public GameObject inventory; // 인벤토리 창 (UI Panel)
    public Transform slotParent;     // 슬롯이 생성될 부모 (Grid Layout Group이 있는 패널)
    public GameObject slotPrefab;    // 아까 만든 슬롯 프리팹 (ItemSlot)

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("저장 데이터")]
    public List<ItemData> myItems = new List<ItemData>(); // 획득한 아이템 목록

    


    void Start()
    {
        // 시작할 때 인벤토리 창은 꺼두기
        if (inventory != null) inventory.SetActive(false);
    }

    void Update()
    {
        // 'I' 키를 누르면 인벤토리 껐다 켰다 하기
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
                inventory.SetActive(!inventory.activeSelf);
            audioSource.PlayOneShot(clickSound);
        }
       
    }

    // [핵심] 외부에서 아이템을 먹었을 때 부르는 함수
    public void AddItem(ItemData item)
    {
        // 1. 리스트에 추가 (데이터 저장)
        myItems.Add(item);

        // 2. 플레이어 스탯에 효과 적용 (PlayerStats로 토스)
        if (playerStats != null) playerStats.ApplyItem(item);

        // 3. UI에 아이콘 추가 (눈에 보이게)
        CreateSlot(item);
    }

    // 슬롯 하나를 생성해서 그림을 넣어주는 함수
    void CreateSlot(ItemData item)
    {
        if (slotPrefab == null || slotParent == null) return;

        // 슬롯 생성 (부모를 slotParent로 지정)
        GameObject newSlot = Instantiate(slotPrefab, slotParent);

        // 아이콘 이미지 변경
        Image iconImage = newSlot.GetComponent<Image>();
        if (item.icon != null)
        {
            iconImage.sprite = item.icon;
        }
        // 아이콘이 없다면 그냥 기본 흰색이 뜸
    }
}