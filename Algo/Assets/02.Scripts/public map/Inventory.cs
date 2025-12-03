using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // UI 건드려야 하니 필수

public class Inventory : MonoBehaviour
{
    [Header("UI 버튼")]
    public Button sort1Button; // 획득순
    public Button sort2Button; // 등급순

    [Header("연결 정보")]
    public PlayerStats playerStats;  // 스탯 적용을 위해 필요
    public GameObject inventory; // 인벤토리 창 (UI Panel)
    public Transform slotParent;     // 슬롯이 생성될 부모 (Grid Layout Group이 있는 패널)

    // 설정과 새로고침을 한 번에 해주는 함수
    // UIConnector에서 slotParent에 직접 넣는 대신, 이 함수를 호출하면 됨
    // Scene을 이동해서 UI가 갱신되면 아이템을 새로고침
    public void SetSlotParent(Transform newParent)
    {
        slotParent = newParent; // 부모 연결
        UpdateSlotUI();         // 아이콘 다시 그리기
    }

    public GameObject slotPrefab;    // ItemSlot

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("저장 데이터")]
    public List<ItemData> myItems = new List<ItemData>(); // 획득한 아이템 목록
    private List<int> acquireOrderList = new List<int>();
    private int currentOrderCount = 0;

    private bool isRecentDescending = false; 
    private bool isNameAscending = false;
    void OnEnable() 
    { // 씬이 로드되면 OnSceneLoaded 함수를 실행하라고 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDisable()
    { // 기능 꺼질 때 등록 해제
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // 씬이 로딩될 때마다 이 함수를 자동으로 불러
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {       
        ConnectUI(); // 여기서 다시 연결
    }
    void Awake()
    {
        SyncLists(); // 시작할 때도 싱크 맞추기
    }

    void SyncLists()
    {
        // 아이템은 있는데 번호표가 모자라면, 모자란 만큼 채워넣음
        if (myItems.Count > acquireOrderList.Count)
        {
            int missingCount = myItems.Count - acquireOrderList.Count;
            for (int i = 0; i < missingCount; i++)
            {
                acquireOrderList.Add(currentOrderCount++);
            }          
        }
    }

    void Start()
    {
        // 게임 처음 켤 때도 연결 시도
        ConnectUI();
    }

    void ConnectUI()
    {
        // 1. InGameScreen에 있는 부모 찾기
        GameObject screenObj = GameObject.Find("InGameScreen");

        if (screenObj != null)
        {
            // 2. 인벤토리 찾기
            Transform invTr = screenObj.transform.Find("Inventory");
            if (invTr != null)
            {
                inventory = invTr.gameObject;

                // 처음엔 꺼져있을 수 있으니 잠시 기억해둠
                bool wasActive = inventory.activeSelf;             

                // 3. 획득순 버튼 찾기
                Transform btn1Tr = invTr.Find("SortByRecent");

                if (btn1Tr != null)
                {
                    sort1Button = btn1Tr.GetComponent<Button>();
                    sort1Button.onClick.RemoveAllListeners();
                    sort1Button.onClick.AddListener(SortByRecent);
                    Debug.Log("연결 성공 획득순");
                }

                // 4. 등급순 버튼 찾기
                Transform btn2Tr = invTr.Find("SortGrade");               

                if (btn2Tr != null)
                {
                    sort2Button = btn2Tr.GetComponent<Button>();
                    sort2Button.onClick.RemoveAllListeners();
                    sort2Button.onClick.AddListener(SortGrade);
                    Debug.Log("연결 성공 등급순");
                }

                // 슬롯 부모도 다시 연결           
                if (slotParent == null) slotParent = invTr;
            }
        }
    }

    void Update()
    {
        // 'I' 키를 누르면 인벤토리 껐다 켰다
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
                inventory.SetActive(!inventory.activeSelf);
            audioSource.PlayOneShot(clickSound);
        }
       
    }



    // 아이템을 먹었을 때 두둥등장
    public void AddItem(ItemData item)
    {
        // 1. 리스트에 추가
        myItems.Add(item);

        // 2. 플레이어 스탯에 효과 적용 (PlayerStats로 패스)
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

    // 저장된 아이템 목록을 보고 슬롯을 다시 만드는 함수
    public void UpdateSlotUI()
    {
        if (slotParent == null || slotPrefab == null) return;

        // 1. 혹시 기존에 남아있는 슬롯이 있다면 싹 청소 (중복 방지)
        foreach (Transform child in slotParent)
        {
            if (child.name.Contains("Sort"))
            {
                continue;
            }
            Destroy(child.gameObject);
        }

        // 2. 내 주머니(myItems)에 있는 모든 아이템을 다시 슬롯으로 만듦
        foreach (ItemData item in myItems)
        {
            CreateSlot(item);
        }
    }
    public void SortByRecent()
    {
        SyncLists();
        if (myItems.Count < 2) return;    

        for (int i = 1; i < myItems.Count; i++)
        {
            ItemData keyItem = myItems[i];
            int keyOrder = acquireOrderList[i];
            int j = i - 1;

            while (j >= 0 && acquireOrderList[j] < keyOrder)
            {
                myItems[j + 1] = myItems[j];
                acquireOrderList[j + 1] = acquireOrderList[j];
                j--;
            }
            myItems[j + 1] = keyItem;
            acquireOrderList[j + 1] = keyOrder;
        }

        Debug.Log("획득순 정렬 완료");
        UpdateSlotUI();
    }

    // 등급순 정렬
    public void SortGrade()
    {
        // 리스트 에서 배열로
        ItemData[] arr = myItems.ToArray();

        // 합병 정렬 실행시키고
        MergeSort.Sort(arr);

        // 배열을 다시 리스트에 다시 반영시킨 다음에
        myItems = new List<ItemData>(arr);

        // UI 새고
        Debug.Log("등급순 정렬 완료");
        UpdateSlotUI();

    }

}