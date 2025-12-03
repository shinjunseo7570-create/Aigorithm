using UnityEngine;
using TMPro;

public class StartShopSceneManager : MonoBehaviour
{
    [Header("Scene의 DialogueManager 연결")]
    public DialogueManager dialogueManager;

    [Header("플레이어 연결(필요 없음)")]
    public PlayerStats player;

    [Header("대사 설정")]
    [TextArea(3, 10)] // 인스펙터에서 입력창을 넓게 보여줍니다.
    public string startMessage = "...";

    void Start()
    {
        if (player == null)
        {
            // Player Tag를 가진 오브젝트를 찾음
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            
            if (playerObject != null)
            {
                // playerObject에서 PlayerStatus 컴포넌트를 가져옴
                player = playerObject.GetComponent<PlayerStats>();
            }
            
            // 여전히 못 찾았다면 로그 출력
            if (player == null)
            {
                Debug.LogWarning("오류 : Player를 찾지 못함");
            }
        }

        // DialogueManager가 연결되어 있다면
        if (dialogueManager != null)
        {
            //StartMessage의 텍스트를 출력합니다.
            dialogueManager.ShowMessage(startMessage);
        }
        else
        {
            Debug.LogWarning("오류 : DialogueManager가 연결되지 않음");
        }
    }
}
