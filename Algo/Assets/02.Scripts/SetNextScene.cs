using UnityEngine;

public class SetNextScene : MonoBehaviour
{
    public string SceneName;
    void OnMouseDown()
    {
        SelectScene.targetScene = SceneName;
        Debug.Log("씬 대상 설정: " + SelectScene.targetScene);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
