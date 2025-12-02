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
            { null, null, null, null, null, null, null,   78,   79,  710},
            { null, null, null, null, null, null, null, null, null,  810},
            { null, null, null, null, null, null, null, null, null,  910},
            { null, null, null, null, null, null, null, null, null, null}
        };
        findRoute(pointMap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
