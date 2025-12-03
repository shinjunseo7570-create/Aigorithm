using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine.UI;



public class DPRoute : MonoBehaviour

{
    // Key: 스테이지 번호(이름), Value: 해당 스테이지의 Transform
    Dictionary<string, Transform> stageObjectMap = new Dictionary<string, Transform>();

    // 필드 선언 시 초기화
    List<int> route = new List<int>();
    public static int[] points = new int[11]
        {
            1, 7, 3, 5, 3, 7, 10, 7, 3, 5, 15 //<- 각 노드 포인트 정의
//          1  2     3  4     5  6      7 <- 레벨
        };
    public static int[,] routeMap = new int[10, 11]
    {
        {0, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 1, 1, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
        {0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 1, 0, 1, 0, 0 },
        {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
    };

    [Header("사용할 텍스트 연결")]
    public TextMeshProUGUI routeText;

    [Header("사용할 오브젝트 연결")]
    // ToggleGroup
    public Transform stageParentGroup;

    [Header("매니저 연결")]
    public SetTargetScene setTargetScene;

    void Start()
    {
        route.Clear();
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

        route.Add(PlayerStats.nodeNum);
        findRoute(PlayerStats.nodeNum);
        visualizationRoute(route);

    }


    void visualizationRoute(List<int> route)
    {

        // 텍스트 초기화
        routeText.text = ("");

        for (int i = 0; i < route.Count; i++)  // <- 추천 루트 노드 처음부터 끝까지 순서대로 반복됨 route[i]로 접근
        {
            int nodeNum = route[i];
            string nodeName = nodeNum.ToString(); // 게임오브젝트 이름(stageObjectMap의 string)을 처리하기 위해 nodeNum을 string으로 변환할 변수

            // 텍스트 업데이트
            routeText.text += ($"{nodeNum} > ");

            // Highlight Outline 켜기
            // 미리 만들어둔 Dictionary에서 해당 번호의 오브젝트를 찾음
            if (stageObjectMap.ContainsKey(nodeName))
            {
                Transform targetStage = stageObjectMap[nodeName];

                // 해당 스테이지 오브젝트 자식 "Highlight Outline"을 찾음
                Transform greenLine = targetStage.Find("Highlight Outline");

                if (greenLine != null)
                {
                    greenLine.gameObject.SetActive(true);
                }
                else
                {
                    Debug.LogWarning($"'{nodeName}' 오브젝트 아래에 'Highlight Outline'이 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning($"Hierarchy에서 '{nodeName}' 이름을 가진 오브젝트를 찾을 수 없습니다.");
            }

        }
        routeText.text = routeText.text.Remove(routeText.text.Length - 3);
    }

    // DP 탐색 및 경로 추적 함수
    void findRoute(int nodeNum)
    {
        int thisLev = searchLev(nodeNum);
        int nextNode;
        if (nodeNum > points.Length) return;
        if (thisLev == searchLev(points.Length))//지금 레벨이 최대 레벨과 동일 
        {
            if (searchLev(route[route.Count - 1]) != thisLev) route.Add(nodeNum); //하면서 동시에 지금 노드의 레벨에서 리스트에 등록이 안되어 있을 때
            return; //결과와 무관하게 이게 마지막 노드이니 return으로 재귀 종료
        }
        if (thisLev % 2 == 1 && nodeNum + 3 <= points.Length) //홀수 레벨(위 아래에 노드 존재)이면서 다음 직진 노드 존재 시
        {
            nextNode = points[nodeNum + 1] >= points[nodeNum + 2] ? nodeNum + 1 : nodeNum + 2; //위 아래 노드 중 더 큰 쪽을 다음 노드로 삼고
            if (routeMap[route[route.Count - 1], nextNode] != 1 && routeMap[route[route.Count - 1], nodeNum] == 1 && routeMap[nodeNum, nextNode] == 1)
            {//이전 노드와 다음 노드가 직접 연결되어있지 않으면서 지금 노드는 양쪽 모두와 연결되어 있을 때 (왼쪽 아래 + 오른쪽 위가 최고값이며 현재값이 0이라 리스트에 등록이 되어있지 않을 때)
                route.Add(nodeNum); //징검다리 역할로 0을 리스트에 추가
            }
            if (points[nodeNum + 3] != 0 && routeMap[nodeNum, nextNode] == 1 && routeMap[nextNode, nodeNum + 3] == 1) //다음 직진 노드가 0이 아니고 현재 노드 - 다음 노드 - 다음 직진노드의 루트가 이어져 있다면
            {
                if(route[route.Count - 1] != nodeNum) route.Add(nodeNum);
                route.Add(nextNode); //다음 노드를 리스트에 추가
                route.Add(nodeNum + 3); //다음 직진노드가 0이 아니므로 최대 점수를 위해서 무조건 거쳐야 하니 리스트에 추가
                findRoute(nodeNum + 3); //다음 직진 노드를 기반으로 재귀함수 진행
            }
            else if (points[nodeNum + 3] == 0 && routeMap[nodeNum, nextNode] == 1) //다음 직진 노드가 0이고 현재 노드 - 다음 노드가 이어져 있다면
            {
                route.Add(nextNode); //다음 노드를 리스트에 추가
                findRoute(nodeNum + 3); //직진 노드는 리스트에 추가하지 않고 다음 직진 노드를 기반으로 재귀함수 진행
            }
        }
        else if (thisLev % 2 == 0)
        {
            findRoute(thisLev + thisLev / 2);
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
    void Update()
    {
    }

}