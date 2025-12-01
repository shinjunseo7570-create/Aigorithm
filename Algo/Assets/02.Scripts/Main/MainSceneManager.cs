using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{

    [Header("플레이어 설정")]
    public GameObject player;

    [Header("UI 연결")]
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
        // 잠시 대기
        yield return null;

        // 플레이어 정보 받아오기
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();

        // 1. 스태미나 스탯을 초기화
        staminaSlider.value = (playerStats.currentStamina * 0.01f);
        staminaText.text = playerStats.currentStamina.ToString();

        // 2. 마지막으로 입장한 맵의 태그를 가져온 후, 그 값을 바탕으로 주변부를 밝힙니다.


        switch(playerInteract.lastStageNum)
        {
            case 1:
                break;
        }
    }
}
