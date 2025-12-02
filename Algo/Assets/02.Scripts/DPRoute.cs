using UnityEngine;

public class DPRoute : MonoBehaviour
{
    int?[,] pointMap;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
