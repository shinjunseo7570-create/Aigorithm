using UnityEngine;
using UnityEngine.SceneManagement;

// 이름 변경: SceneLoader (씬 로더)
public class SceneLoader : MonoBehaviour
{
    // public string gameSceneName;  <-- 이 줄을 삭제합니다!

    // 이제 이 함수는 'sceneName'이라는 '매개변수'를 받습니다.
    // 버튼이 이 함수를 호출할 때 씬의 이름을 직접 전달해줄 것입니다.
    public void LoadScene(string sceneName)
    {
        LoadingSceneManager.LoadScene(sceneName);
    }
}