using UnityEngine;
using System.Collections.Generic;
public class MapDataManager : MonoBehaviour
{
    // 맵 정보 하나를 정의하는 설계도
    [System.Serializable]
    public class StageData
    {
        public int mapID;                        // 검색용 맵 ID
        public string mapName;                   // 맵 이름
        [TextArea] public string mapDescription; // 맵 설명

        public int rewardPoints;                 // 보상 포인트
        public int staminaCost;                  // 소모 스태미나

        public List<string> enemyList;           // 적 목록
        public string clearCondition;            // 클리어 조건
    }

    // 모든 맵 데이터를 담아둘 리스트 (inspector에서 입력)
    [Header("맵 데이터 리스트")]
    public List<StageData> mapDataList = new List<StageData>();

    // ID를 주면 데이터를 찾아서 꺼내주는 함수
    public StageData GetStageDataByID(int id)
    {
        // 리스트에서 mapID가 같은 것을 찾는다.
        StageData foundData = mapDataList.Find(x => x.mapID == id);

        if (foundData == null)
        {
            Debug.LogWarning($"ID [{id}]에 해당하는 맵 데이터를 찾을 수 없습니다!");
        }

        return foundData;
    }
}