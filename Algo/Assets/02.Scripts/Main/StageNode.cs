using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class StageNode : MonoBehaviour
{
    [Header("스테이지 정보 입력")]
    public int mapID; // 맵 번호

    void Start()
    {
        // 자신의 토글 컴포넌트를 찾음
        Toggle toggle = GetComponent<Toggle>();

        if (toggle != null)
        {
            // 2. 클릭하면 OnClicked 함수 실행
            toggle.onValueChanged.AddListener(OnClicked);
        }
    }

    // 토글이 눌렸을 때 실행됨
    void OnClicked(bool isOn)
    {
        if (isOn)
        {
            // 1. 데이터 매니저(MapDataManager)를 찾습니다.
            MapDataManager dataManager = FindFirstObjectByType<MapDataManager>();
            // 2. UI 매니저(StageUIManager)를 찾습니다.
            StageUIManager uiManager = FindFirstObjectByType<StageUIManager>();

            if (dataManager != null && uiManager != null)
            {
                // 3. ID를 이용해 데이터 매니저에서 정보를 가져옵니다.
                var data = dataManager.GetStageDataByID(mapID);

                if (data != null)
                {
                    // 4. 가져온 정보를 UI 매니저에게 넘겨줘서 화면에 띄웁니다.
                    uiManager.UpdateStageInfoUI(
                        data.mapID,
                        data.mapName,
                        data.mapDescription,
                        data.enemyList,
                        data.clearCondition,
                        data.rewardPoints,
                        data.staminaCost
                    );
                }
                else Debug.LogWarning("mapID에 연결된 data가 없음");
            }
            else
            {
                if (dataManager == null) Debug.LogError("MapDataManager 없음");
                if (uiManager == null) Debug.LogError("StageUIManager 없음");
            }
        }
    }
}