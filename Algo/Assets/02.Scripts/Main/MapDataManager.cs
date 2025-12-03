using UnityEngine;
using System.Collections.Generic;
public class MapDataManager : MonoBehaviour
{
    // 맵 정보 하나를 정의하는 클래스 생성
    // List 내의 내용물을 모두 보이게 하기 위해 System.Serializable 사용
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

    // ID로 data를 찾는 함수
    public StageData GetStageDataByID(int id)
    {
        // 리스트에 있는 데이터를 하나씩 검사
        foreach (StageData data in mapDataList)
        {
            // 만약 꺼낸 데이터의 ID가 내가 찾는 ID랑 같다면
            if (data.mapID == id)
            {
                return data; // 그 데이터를 return
            }
        }

        // 끝까지 탐색해도 없다면
        Debug.LogWarning($"mapID {id}에 할당된 데이터 없음");
        return null; // null return
    }
}