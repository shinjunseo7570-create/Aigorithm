using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageIconImage : MonoBehaviour
{
    [Header("사용할 오브젝트 연결")]
    public Transform stageParentGroup;
    // DPRoute 스크립트는 static 변수(routeMap)를 쓰므로 연결 안 해도 될 수 있지만, 
    // 혹시 모르니 남겨둡니다.
    public DPRoute dpRoute;

    [Header("변경할 텍스쳐")]
    public Texture availableTexture; // 갈 수 있음
    public Texture disabledTexture;  // 못 감
    public Texture nowTexture;       // 현재 위치
    public Texture routeTexture;     // (추후 사용) 경로

    // Key: 스테이지 번호(이름), Value: 해당 스테이지의 Transform
    Dictionary<string, Transform> stageObjectMap = new Dictionary<string, Transform>();

    void Start()
    {
        // 1. 모든 스테이지 오브젝트 찾아서 저장하기
        if (stageParentGroup != null)
        {
            Transform[] allChildren = stageParentGroup.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                // 이름이 숫자로 된 것들만 딕셔너리에 추가 (0, 1, 2...)
                // (TryParse를 써서 숫자인지 확인하는게 안전함)
                if (int.TryParse(child.name, out int _))
                {
                    if (!stageObjectMap.ContainsKey(child.name))
                    {
                        stageObjectMap.Add(child.name, child);
                    }
                }
            }
        }

        // 2. 이미지 갱신 함수 호출 (매개변수 필요 없음)
        ChangeAllIcons();
    }

    // [수정됨] 모든 아이콘을 순회하며 상태에 따라 이미지를 바꾸는 함수
    public void ChangeAllIcons()
    {
        // PlayerStats.nodeNum은 static 변수라고 가정합니다.
        int currentNode = PlayerStats.nodeNum;

        // DPRoute의 맵 데이터가 준비되었는지 확인
        if (DPRoute.routeMap == null)
        {
            Debug.LogWarning("DPRoute.routeMap이 아직 생성되지 않았거나 비어있습니다.");
            return;
        }

        // 딕셔너리에 저장된 모든 스테이지를 하나씩 꺼내서 검사
        foreach (KeyValuePair<string, Transform> entry in stageObjectMap)
        {
            string nodeName = entry.Key;      // "0", "1", "2"...
            Transform targetStage = entry.Value; // 해당 오브젝트

            // 이름을 숫자로 변환 (배열 인덱스로 쓰기 위해)
            if (int.TryParse(nodeName, out int nodeIdx))
            {
                // RawImage 컴포넌트 찾기
                RawImage rawImage = targetStage.GetComponentInChildren<RawImage>(); // 자식에 있을수도 있으니 InChildren 권장

                if (rawImage != null)
                {
                    // 1. 현재 내가 있는 노드라면?
                    if (nodeIdx == currentNode)
                    {
                        rawImage.texture = nowTexture;
                    }
                    // 2. 현재 노드에서 갈 수 있는 곳인가? (routeMap 값이 1이면 연결됨)
                    // 배열 범위를 벗어나지 않게 체크 필수
                    else if (DPRoute.routeMap.GetLength(0) > currentNode && DPRoute.routeMap.GetLength(1) > nodeIdx)
                    {
                        if (DPRoute.routeMap[currentNode, nodeIdx] == 1)
                        {
                            rawImage.texture = availableTexture;
                        }
                        else
                        {
                            rawImage.texture = disabledTexture;
                        }
                    }
                }
            }
        }
    }
}