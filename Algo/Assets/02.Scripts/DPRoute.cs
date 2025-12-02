using UnityEngine;

using System.Collections.Generic;

using System.Linq;

using TMPro;



public class DPRoute : MonoBehaviour

{

    // 필드 선언 시 초기화
    List<int> route = new List<int>();
    int[] points = new int[10]
        {
            1, 2, 3, 0, 4, 5, 6, 7, 8, 9 //<- 각 노드 포인트 정의
//          1  2     3  4     5  6     7 <- 레벨
        };
    int[,] pointMap = new int[10, 10]
    {
        {0, 1, 1, 1, 0, 0, 0, 0, 0, 0 },
        {0, 0, 0, 1, 1, 0, 0, 0, 0, 0 },
        {0, 0, 0, 1, 0, 1, 0, 0, 0, 0 },
        {0, 0, 0, 0, 1, 1, 1, 0, 0, 0 },
        {0, 0, 0, 0, 0, 0, 1, 1, 0, 0 },
        {0, 0, 0, 0, 0, 0, 1, 0, 1, 0 },
        {0, 0, 0, 0, 0, 0, 0, 1, 1, 1 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 1 },
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
    };

    [Header("사용할 텍스트 연결")]
    public TextMeshProUGUI routeText;

    void Start()
    {
        pointMap = new int?[10, 10]
        {
            { null,   12,   13,   14, null, null, null, null, null, null},
            { null, null, null,   24,   25, null, null, null, null, null},
            { null, null, null,   34, null,   36, null, null, null, null},
            { null, null, null, null,   45,   46,   47, null, null, null},
            { null, null, null, null, null, null,   57,   58, null, null},
            { null, null, null, null, null, null,   67, null,   69, null},
            { null, null, null, null, null, null, null,   78,   79,  710},//DP 알고리즘을 사용하기 위한 가중치의 수치 (가중치가 높을 수록 더 우선)
            { null, null, null, null, null, null, null, null, null,  810},//위나 아래로 간 후 옆 노드로 갈 수 있으므로 옆 노드가 0이 아니라면 무조건 둘 중 더 큰쪽으로 갔다가 돌아오는 것이 이득
            { null, null, null, null, null, null, null, null, null,  910},//옆 노드가 0이라면 위 + 위의 옆노드 ? 아래 + 아래의 옆 노드 비교후 높은쪽으로 가야함
            { null, null, null, null, null, null, null, null, null, null} 
        };
        // findRoute(pointMap);
    }
    void findRoute(int?[,] point)
    {
        route.Add(0);
        findRoute(pointMap, 0);

        GameObject toggleGroup = GameObject.Find("ToggleGroup");
        Transform[] allChildren = toggleGroup.GetComponentsInChildren<Transform>(true); // true -> 비활성화된 오브젝트같이 찾아냄

        routeText.text = ("");
        for (int i = 0; i < route.Count; i++)  //<- 추천 루트 노드 처음부터 끝까지 순서대로 반복됨 route[i]로 접근
        {
            routeText.text += ($"{route[i]} > ");//여기서 비주얼적 업데이트를 함수를 돌리든 뭘 하든 하면 됨
            allChildren.Find<$"{route[i]}">.transform.Find("Green Line").SetActive(true);
        }
        routeText.text += ($"도착");
    }
    // DP 탐색 및 경로 추적 함수

    void findRoute(int[,] weights, int nodeNum)
    {
        int thisLev = searchLev(nodeNum);
        int nextNode;
        if (nodeNum > points.Length) return;
        if (thisLev % 2 == 1 && nodeNum + 3 <= points.Length) 
        {
            if (points[nodeNum + 3] != 0 || (points[nodeNum + 3] == 0 && thisLev > 3)) 
            { 
                nextNode = points[nodeNum + 1] >= points[nodeNum + 2] ? nodeNum + 1 : nodeNum + 2;
                route.Add(nextNode);
                route.Add(nodeNum+3);
                findRoute(weights, nodeNum + 3);
            }else if (points[nodeNum + 3] == 0 && thisLev <= 3)
            {
                if(points[nodeNum + 6] != 0 || (points[nodeNum + 6] == 0 && thisLev > 1))
                {
                    nextNode = points[nodeNum + 1] + points[nodeNum + 4] >= points[nodeNum + 2] + points[nodeNum + 5] ? nodeNum + 1 : nodeNum + 2;
                    route.Add(nextNode);
                    route.Add(nextNode+3);
                    route.Add(nodeNum+6);
                    findRoute(weights, nodeNum + 6);
                }
                else if(points[nodeNum + 6] == 0 && thisLev == 1)
                {
                    nextNode = points[nodeNum + 1] + points[nodeNum + 4] + points[nodeNum + 7] >= points[nodeNum + 2] + points[nodeNum + 5] + points[nodeNum + 8] ? nodeNum + 1 : nodeNum + 2;
                    route.Add(nextNode);
                    route.Add(nextNode + 3);
                    route.Add(nextNode + 6);
                    route.Add(nodeNum + 9);
                    findRoute(weights, nodeNum + 9);
                }
            }
        }
    }
    int searchLev(int nodeNum)
    {
        if(nodeNum % 3 == 0)
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