using UnityEngine;

public class SelectScene : MonoBehaviour
{
    public static string targetScene = null;
    public static int nodeNum;
    static bool tutorial = false;
    public void LoadScene()
    {
        if (targetScene != null && DPRoute.routeMap[PlayerStats.nodeNum, nodeNum] == 1 && searchLev(nodeNum) > searchLev(PlayerStats.nodeNum)) 
        {
            Debug.Log("씬 로드중: " + targetScene);

            GameObject player = GameObject.FindWithTag("Player");
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            PlayerStats.nodeNum = nodeNum;
            playerStats.point += DPRoute.points[nodeNum];
            playerStats.currentStamina -= 10;

            LoadingSceneManager.LoadScene("Map" + targetScene);
        }
        else if (searchLev(nodeNum) <= searchLev(PlayerStats.nodeNum))
        {
            Debug.Log("이미 지나온 단계");
        }
        else
        {
            Debug.Log("씬이 선택되지 않음");
        }
    }
    int searchLev(int nodeNum)
    {
        if (nodeNum % 3 == 0)
        {
            return (nodeNum / 3) * 2 + 1;
        }
        else
        {
            return (nodeNum / 3) * 2 + 2;
        }
    }
    void Start()
    {
        if (!tutorial)
        {
            tutorial = true;
            GameObject player = GameObject.FindWithTag("Player");
            PlayerStats playerStats = player.GetComponent<PlayerStats>();
            playerStats.currentStamina -= 10;
            LoadingSceneManager.LoadScene("Map1");
        }
    }
}
