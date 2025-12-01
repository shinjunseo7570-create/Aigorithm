using UnityEngine;

public class SetNextScene : MonoBehaviour
{
    public void changeTargetScene(string SceneName)
    {
        SelectScene.targetScene = SceneName;
        Debug.Log("씬 대상 설정: " + SelectScene.targetScene);
    }
}
