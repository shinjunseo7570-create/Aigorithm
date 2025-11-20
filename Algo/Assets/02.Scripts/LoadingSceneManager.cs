using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadingSceneManager : MonoBehaviour
{

    public static string nextScene;

    [Header("최소로 보장해줄 로딩 시간")]
    public float minDuration = 1.0f;

    void Start()
    {
        // 시작하자마자 코루틴 실행
        StartCoroutine(LoadingSceneProcess());
    }
    // 다른 스크립트에서 이 함수를 호출 가능
    // static의 역할 : 객체 생성 없이 즉시 호출 가능
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName; // 받아온 Scene의 이름을 저장
        SceneManager.LoadScene("Loading"); // 중간에 Loading Scene을 불러온다.
    }

    IEnumerator LoadingSceneProcess()

    {
        // LoadingScene: 동기, Scene을 불러오면서 다른 작업이 불가능
        // LoadingSceneAsync: 비동기, Scene을 불러오면서 다른 작업이 가능하도록 함
        // AsyncOperation: 비동기적 연산을 위한 코루틴
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);

        // allowSceneActivation: 장면이 준비되는 즉시 장면을 활성화할까요?
        // Scene이 바로 넘어가지 않게(어색하지 않게)
        op.allowSceneActivation = false;

        float timer = 0;

        while ((op.progress < 0.9f) || (timer < minDuration))
        {
            timer += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
        
    }
}
