using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManageIconImage : MonoBehaviour
{
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

    void Start()
    {
        // 모든 스테이지 오브젝트 찾아서 저장하기
        if (stageParentGroup != null)
        {
            Transform[] allChildren = stageParentGroup.GetComponentsInChildren<Transform>(true);
            foreach (Transform child in allChildren)
            {
                // 이름이 숫자로 된 것들만 딕셔너리에 추가
                if (int.TryParse(child.name, out int _))
                {
                    if (!stageObjectMap.ContainsKey(child.name))
                    {
                        stageObjectMap.Add(child.name, child);
                    }
                }
            }
        }

        // 이미지 갱신 함수 호출
        ChangeAllIcons();
    }

    // 모든 아이콘을 돌며 상태에 따라 이미지를 바꾸는 함수
    public void ChangeAllIcons()
    {
        int currentNode = PlayerStats.nodeNum;

        // null이면
        if (DPRoute.routeMap == null)
        {
            Debug.LogWarning("DPRoute.routeMap이 아직 생성되지 않았거나 비어있습니다.");
            return;
        }

        // 저장된 모든 스테이지를 하나씩 검사
        foreach (KeyValuePair<string, Transform> entry in stageObjectMap)
        {
            string nodeName = entry.Key;      // 0, 1, 2 등등
            Transform targetStage = entry.Value; // 해당 오브젝트

            // 이름을 숫자로 변환 (배열 인덱스로 쓰기 위해)
            if (int.TryParse(nodeName, out int nodeIndex))
            {
                // RawImage 컴포넌트 찾기
                RawImage rawImage = targetStage.GetComponentInChildren<RawImage>();

                if (rawImage != null)
                {
                    // 현재 내가 있는 노드인지
                    if (nodeIndex == currentNode)
                    {
                        // 현재 위치 텍스쳐
                        rawImage.texture = nowTexture;
                    }
                    // 현재 노드에서 갈 수 있는 노드인지
                    // 배열 범위를 벗어나지 않는지 체크
                    else if (DPRoute.routeMap.GetLength(0) > currentNode && DPRoute.routeMap.GetLength(1) > nodeIndex)
                    {
                        // 이용가능 텍스쳐
                        if (DPRoute.routeMap[currentNode, nodeIndex] == 1)
                        {
                            rawImage.texture = availableTexture;
                        }
                        // 비활성화 텍스쳐
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