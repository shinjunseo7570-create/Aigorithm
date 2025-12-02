using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{

    [Header("플레이어 연결")]
    public GameObject player;
    private PlayerStats playerStats; // 매번 GetComponent하지 않음

    [Header("UI 연결")]
    public Slider staminaSlider;
    public TextMeshProUGUI staminaText;

    void Start()
    {
        // 씬을 이동하고 돌아왔을 때, 살아남은 '진짜 플레이어(싱글톤)'를 우선적으로 찾습니다.
        // 싱글톤은 프로그래밍 디자인 패턴 중 하나로, 전체 게임을 통틀어 이 클래스의 실체(Instance)는 딱 하나만 존재해야 한다는 규칙을 강제하는 방법임
        // Main을 로드하면서 잠시 생기는 새 플레이어(꽉 찬 스탯)가 생기는데, 걔를 불러오지 않게 하려고
        // instance가 이미 있다면 instance를 잡아서 연결해버림
        // PlayerStats.cs에서 instance를 public으로 바꿨기 때문에 접근 가능
        if (PlayerStats.instance != null)
        {
            playerStats = PlayerStats.instance;
            player = playerStats.gameObject;
        }
        else
        {
            Debug.LogWarning("싱글톤 플레이어를 찾지 못함");
        }
    }

    void Update()
    {
        UpdateMainSceneUI();
    }

    void UpdateMainSceneUI()
    {
        // 스태미나 UI
        staminaSlider.value = (float)playerStats.currentStamina / playerStats.maxStamina;
        staminaText.text = playerStats.currentStamina.ToString();
    }
}
