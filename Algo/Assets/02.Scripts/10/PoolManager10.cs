using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager10 : MonoBehaviour
{
    // 프리펩들을 보관할 변수 / 생성할 물건의 원본들 (예: 총알, 몬스터 등)
    public GameObject[] prefabs;

    // 풀을 담당할 리스트들 / 창고 (종류별로 리스트를 따로 만듦)
    List<GameObject>[] pools;

        void Awake()
        {

        // 인스펙터에서 지정한 만큼의 개수만큼 리스트가 들어갈 배열을 만듬
        // 우리 프리팹 종류가 몇 개지? 3개네? 그럼 3칸짜리 보관함 틀을 짜자.
        // 처음 만들면 null로 시작하기 때문에 넣어줘야한다.
        pools = new List<GameObject>[prefabs.Length];

        // 위에서 만든 배열을 방문하며
        // index = 0일 때: pools[0] 자리에 새로운 빈 리스트를 생성해서 집어넣음.
        // 실제 물건을 담을 수 있는 바구니(List)를 하나씩 넣어주자.
        for (int index = 0; index < pools.Length; index++)
            {
                pools[index] = new List<GameObject>();
            }
        }

    // 게임 오브젝트를 반환하는 함수 선언
    public GameObject Get(int index)
    {
        GameObject select = null;

        // 선택한 풀의 놀고 있는 게임 오브젝트 접근
        

        foreach (GameObject item in pools[index])
        {
            // 총알 하나 줘!"라고 요청(Get)이 오면, 리스트를 뒤져서 현재 놀고 있는(꺼져 있는) 오브젝트가 있는지 확인합니다.
            // 있으면 그걸 다시 켜서 줍니다. (재활용)
            if (!item.activeSelf)
            {
                // 발견하면 select 변수에 할당
                select = item;
                select.SetActive(true);
                break;
            }
        }

        // 못 찾으면 새롭게 생성하고 select 변수에 할당
        // 창고에 남는 게 없으면 어쩔 수 없이 Instantiate로 새로 하나 만들어서 줍니다.
        // 그리고 이 새로 만든 녀석도 리스트에 등록해둬서, 나중에 다 쓰고 꺼지면 재활용할 수 있게 합니다.
        if (!select)
        {
            select = Instantiate(prefabs[index], transform);
            select.SetActive(true);
            pools[index].Add(select);
        }

        return select;
    }
}
