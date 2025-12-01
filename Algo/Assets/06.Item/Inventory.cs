using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI; // UI �ǵ���� �ϴ� �ʼ�

public class Inventory : MonoBehaviour
{
    /*[Header("���� ����")]
    public PlayerStats playerStats;  // ���� ������ ���� �ʿ�
    public GameObject inventory; // �κ��丮 â (UI Panel)
    public Transform slotParent;     // ������ ������ �θ� (Grid Layout Group�� �ִ� �г�)
    public GameObject slotPrefab;    // �Ʊ� ���� ���� ������ (ItemSlot)

    [Header("���� ������")]
    public List<ItemData> myItems = new List<ItemData>(); // ȹ���� ������ ���

    void Start()
    {
        // ������ �� �κ��丮 â�� ���α�
        if (inventory != null) inventory.SetActive(false);
    }

    void Update()
    {
        // 'I' Ű�� ������ �κ��丮 ���� �״� �ϱ�
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventory != null)
                inventory.SetActive(!inventory.activeSelf);
        }
    }

    // [�ٽ�] �ܺο��� �������� �Ծ��� �� �θ��� �Լ�
    public void AddItem(ItemData item)
    {
        // 1. ����Ʈ�� �߰� (������ ����)
        myItems.Add(item);

        // 2. �÷��̾� ���ȿ� ȿ�� ���� (PlayerStats�� �佺)
        if (playerStats != null) playerStats.ApplyItem(item);

        // 3. UI�� ������ �߰� (���� ���̰�)
        CreateSlot(item);
    }

    // ���� �ϳ��� �����ؼ� �׸��� �־��ִ� �Լ�
    void CreateSlot(ItemData item)
    {
        if (slotPrefab == null || slotParent == null) return;

        // ���� ���� (�θ� slotParent�� ����)
        GameObject newSlot = Instantiate(slotPrefab, slotParent);

        // ������ �̹��� ����
        Image iconImage = newSlot.GetComponent<Image>();
        if (item.icon != null)
        {
            iconImage.sprite = item.icon;
        }
        // �������� ���ٸ� �׳� �⺻ ����� ��
    }*/
}