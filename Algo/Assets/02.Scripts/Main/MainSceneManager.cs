using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{

    [Header("�÷��̾� ����")]
    public GameObject player;

    [Header("UI ����")]
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;

    void Awake()
    {

    }

    void Start()
    {

        StartCoroutine(InitMainSceneRoutine());
    }

    IEnumerator InitMainSceneRoutine()
    {
        // ��� ���
        yield return null;

        // �÷��̾� ���� �޾ƿ���
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();

        // 1. ���¹̳� ������ �ʱ�ȭ
        staminaSlider.value = (playerStats.currentStamina * 0.01f);
        staminaText.text = playerStats.currentStamina.ToString();

        // 2. ���������� ������ ���� �±׸� ������ ��, �� ���� �������� �ֺ��θ� �����ϴ�.


        switch(playerInteract.lastStageNum)
        {
            case 1:
                break;
        }
    }
}
