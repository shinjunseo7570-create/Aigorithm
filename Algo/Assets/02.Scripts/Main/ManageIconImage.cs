using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageIconImage : MonoBehaviour
{
    // ToggleGroup
    [Header("사용할 오브젝트 연결")]
    public Transform stageParentGroup;
    public DPRoute dpRoute;

    [Header("변경할 텍스쳐")]
    public Texture availableTexture;
    public Texture disabledTexture;
    public Texture nowTexture;
    public Texture routeTexture;

    // Key: 스테이지 번호(이름), Value: 해당 스테이지의 Transform
    Dictionary<string, Transform> stageObjectMap = new Dictionary<string, Transform>();

    // 필드 선언 시 초기화
    List<int> route = new List<int>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 부모(ToggleGroup) 아래의 모든 자식을 찾음
        if (stageParentGroup != null)
        {
            Transform[] allChildren = stageParentGroup.GetComponentsInChildren<Transform>(true); // true로 비활성화 오브젝트까지 다 긁어옴
            foreach (Transform child in allChildren)
            {
                // 딕셔너리에 이름(ex: "0", "1")을 키로 저장
                // 중복 방지를 위해 키가 없을 때만 추가
                if (!stageObjectMap.ContainsKey(child.name))
                {
                    // 오브젝트 이름과 트랜스폼을 매핑
                    stageObjectMap.Add(child.name, child);
                }
            }
        }

        changeRouteImage(route);
    }

    void changeRouteImage(List<int> route)
    {
        GameObject player = GameObject.FindWithTag("Player");
        PlayerStats playerStats = player.GetComponent<PlayerStats>();

        for (int i = 0; i < route.Count; i++)  // <- 추천 루트 노드 처음부터 끝까지 순서대로 반복됨 route[i]로 접근
        {
            int nodeNum = route[i];
            string nodeName = nodeNum.ToString(); // 게임오브젝트 이름(stageObjectMap의 string)을 처리하기 위해 nodeNum을 string으로 변환할 변수

            // Highlight Outline 켜기
            // 미리 만들어둔 Dictionary에서 해당 번호의 오브젝트를 찾음
            if (stageObjectMap.ContainsKey(nodeName))
            {
                Transform targetStage = stageObjectMap[nodeName];

                // 해당 스테이지 오브젝트 자식 "Raw Image"을 찾음
                RawImage rawImage = targetStage.GetComponent<RawImage>();

                if (rawImage != null)
                {

                    if (DPRoute.routeMap[PlayerStats.nodeNum, nodeNum] != 1)
                    {
                        rawImage.texture = disabledTexture;
                    }
                    else if (PlayerStats.nodeNum == nodeNum)
                    {
                        rawImage.texture = nowTexture;
                    }
                    else
                    {
                        rawImage.texture = availableTexture;
                    }

                }
                else
                {
                    Debug.LogWarning($"'{nodeName}' 스테이지 아이콘에서 오류 발생!");
                }
            }
            else
            {
                Debug.LogWarning($"Hierarchy에서 '{nodeName}' 이름을 가진 오브젝트를 찾을 수 없습니다.");
            }

        }
    }
}
