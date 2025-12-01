using UnityEngine;
using TMPro;
using System.Collections; // 코루틴을 쓰기 위해

public class DialogueManager : MonoBehaviour
{
    [Header("UI 연결")]
    public TextMeshProUGUI dialogueText;

    [Header("글자 출력 간격 (초)")]
    public float typingSpeed = 0.05f; // 숫자가 작을수록 빠름

    [Header("오디오 설정")]
    public AudioSource audioSource; // 스피커 컴포넌트
    public AudioClip typingSound;   // 사운드 파일

    private Coroutine currentCoroutine; // 현재 진행 중인 타이핑을 추적할 수 있는 변수

    // 외부에서 이 함수를 호출해서 메시지를 띄웁니다.
    public void ShowMessage(string message)
    {
        // 만약 이미 다른 글자가 타이핑 중이라면 멈춤 (중복 방지)
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 새 타이핑 시작
        dialogueText.text = ""; // 기존 텍스트 비우기
        currentCoroutine = StartCoroutine(TypeText(message)); // 현재 실행 중인 코루틴을 저장
    }

    // 실제로 한 글자씩 타이핑하는 코루틴 함수
    IEnumerator TypeText(string message)
    {

        //foreach (타입 변수명 in 컬렉션명)
        // 입력받은 메시지를 문자 배열로 바꿔서 하나씩 반복
        foreach (char letter in message.ToCharArray())
        {
            dialogueText.text += letter; // 한 글자 추가

            // 플레이어와 타이핑 사운드가 존재할 때
            // 띄어쓰기(공백)가 아닐 때만 소리를 재생합니다.
            if (letter != ' ' && audioSource != null && typingSound != null)
            {
                // PlayOneShot은 효과음을 겹쳐서 재생할 수 있습니다.
                audioSource.PlayOneShot(typingSound);
            }

            yield return new WaitForSeconds(typingSpeed); // 설정한 시간만큼 대기합니다.
        }

        currentCoroutine = null; // 타이핑 완료 후 현재 실행중 코루틴을 비웁니다.
    }

    // 텍스트창 숨기기
    public void HideDialogue()
    {
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        dialogueText.text = "";
    }
}
