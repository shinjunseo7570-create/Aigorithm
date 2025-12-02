using UnityEngine;

public class SelectScene : MonoBehaviour
{
    public static string targetScene = null;
    public void LoadScene()
    {
        if (targetScene != null)
        {
            Debug.Log("씬 로드중: " + targetScene);

            GameObject player = GameObject.FindWithTag("Player");
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            playerStats.currentStamina -= 10;

            LoadingSceneManager.LoadScene("Map" + targetScene);
        }
        else
        {
            Debug.Log("씬이 선택되지 않음");
        }
    }   
}
