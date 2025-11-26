using UnityEngine;
using TMPro; // TextMeshPro를 쓰기 위해 필수!
using System.Collections; // 코루틴을 쓰기 위해 필수!

public class DialogueManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI dialogueText; // 캔버스에 있는 TMP 텍스트 컴포넌트

    [Header("설정")]
    [Tooltip("글자 하나 나오는 시간 (초)")]
    public float typingSpeed = 0.05f; // 숫자가 작을수록 빠름

    [Header("오디오 설정")]
    public AudioSource audioSource; // 스피커 컴포넌트
    public AudioClip typingSound;   // 사운드 파일

    private Coroutine currentCoroutine; // 현재 진행 중인 타이핑을 추적

    // 외부(DevilShop 등)에서 이 함수를 호출해서 메시지를 띄움
    public void ShowMessage(string message)
    {
        // 만약 이미 다른 글자가 타이핑 중이라면 멈춤 (중복 방지)
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 새 타이핑 시작
        dialogueText.text = ""; // 기존 텍스트 비우기
        currentCoroutine = StartCoroutine(TypeText(message));
    }

    // 실제로 한 글자씩 타이핑하는 코루틴 함수
    IEnumerator TypeText(string message)
    {
        // 입력받은 메시지를 문자 배열로 바꿔서 하나씩 반복
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter; // 한 글자 추가

            // ↓↓↓ 새로 추가된 부분: 소리 재생 ↓↓↓
            // 띄어쓰기(공백)가 아닐 때만 소리를 재생합니다. (더 자연스러움)
            if (letter != ' ' && audioSource != null && typingSound != null)
            {
                // PlayOneShot은 짧은 효과음을 겹쳐서 재생할 때 좋습니다.
                audioSource.PlayOneShot(typingSound);
            }

            // 나중에 여기에 '툭' 하는 효과음 재생 코드를 넣으면 완벽한 언더테일 느낌이 납니다.
            yield return new WaitForSeconds(typingSpeed); // 설정한 시간만큼 대기
        }

        currentCoroutine = null; // 타이핑 완료
    }

    // (옵션) 텍스트창 숨기기 기능이 필요할 때 사용
    public void HideDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        dialogueText.text = "";
        // dialogueText.gameObject.SetActive(false); // 오브젝트 자체를 끌 수도 있음
    }
}
