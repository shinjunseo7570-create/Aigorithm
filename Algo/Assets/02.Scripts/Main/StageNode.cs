using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageNode : MonoBehaviour
{

    // 스테이지 토글에 직접 넣을 스크립트
    // 맵 번호만 입력하면 됨

    [Header("스테이지 정보 입력")]
    public int mapID; // 맵 번호

    void Start()
    {
        // 자신의 토글 컴포넌트를 찾음
        Toggle toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            // 클릭하면 OnClicked 함수 실행
            toggle.onValueChanged.AddListener(OnClicked);
        }
    }

    // 토글이 눌렸을 때 실행됨
    void OnClicked(bool isOn)
    {
        SelectScene.nodeNum = int.Parse(gameObject.name);
        if (isOn)
        {
            // MapDataManager, StageUIManager 찾기
            // FindFirstObjectByType<T>()는 자동으로 스크립트를 찾아 연결해줌
            MapDataManager mapDataManager = FindFirstObjectByType<MapDataManager>();
            StageUIManager stageUiManager = FindFirstObjectByType<StageUIManager>();

            if (mapDataManager != null && stageUiManager != null)
            {
                // ID를 이용해 MapDataManager에서 정보를 가져옴
                MapDataManager.StageData data = mapDataManager.GetStageDataByID(mapID);

                if (data != null)
                {
                    // 가져온 정보를 StageUIManager의 UpdateStageInfoUI에게 넘겨줌
                    // 정보를 바탕으로 UI를 변경해주는 스크립트
                    stageUiManager.UpdateStageInfoUI(
                        data.mapID,
                        data.mapName,
                        data.mapDescription,
                        data.enemyList,
                        data.clearCondition,
                        data.rewardPoints,
                        data.staminaCost
                    );
                }
                else Debug.LogWarning("mapID에 연결된 데이터 없음");
            }
            else
            {
                if (mapDataManager == null) Debug.LogError("발견된 MapDataManager 없음");
                if (stageUiManager == null) Debug.LogError("발견된 StageUIManager 없음");
            }
        }
    }
}