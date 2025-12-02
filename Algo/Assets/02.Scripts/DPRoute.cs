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