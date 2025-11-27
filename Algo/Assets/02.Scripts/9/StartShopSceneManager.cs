using UnityEngine;
using TMPro;

public class StartShopSceneManager : MonoBehaviour
{
    [Header("Scene의 DialogueManager 연결")]
    public DialogueManager dialogueManager;

    [Header("플레이어 연결")]
    public PlayerStatus player;

    [Header("대사 설정")]
    [TextArea(3, 10)] // 인스펙터에서 입력창을 넓게 보여줍니다.
    public string startMessage = "...";

    [Header("스탯 표시 텍스트 연결")]
    public TextMeshProUGUI statusText;

    void Start()
    {
        // DialogueManager가 연결되어 있다면
        if (dialogueManager != null)
        {
            //StartMessage의 텍스트를 출력합니다.
            dialogueManager.ShowMessage(startMessage);
        }
        else
        {
            Debug.LogWarning("SceneStartDialogue: DialogueManager가 연결되지 않았습니다!");
        }

        ShowCurrentStatus();
    }

    public void ShowCurrentStatus()
    {
        statusText.text = ($"체력: {player.currentHealth}/{player.maxHealth}\n" +
            $"스태미나: {player.currentStamina}/{player.maxStamina}\n" +
            $"공격력: {player.strength}\n");
    }
}
