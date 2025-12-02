using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainSceneManager : MonoBehaviour
{

    [Header("플레이어 연결")]
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

        // 플레이어 가져오기
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        PlayerInteract playerInteract = player.GetComponent<PlayerInteract>();

        // 스태미나 UI
        staminaSlider.value = (playerStats.currentStamina * 0.01f);
        staminaText.text = playerStats.currentStamina.ToString();
    }
}
