using UnityEngine;

public class SelectScene : MonoBehaviour
{
    public static string targetScene = null;
    void OnMouseDown()
    {
        if (targetScene != null)
        {
            Debug.Log("씬 로드중: " + targetScene);
            PlayerInteract.stemina -= 10;
            LoadingSceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.Log("씬이 선택되지 않음");
        }
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
