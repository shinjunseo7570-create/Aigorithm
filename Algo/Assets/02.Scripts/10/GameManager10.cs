using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager10 : MonoBehaviour
{

    public float gameTime;
    public float maxGameTime = 2 * 10f;
    public static GameManager10 instance;
    public PlayerInteract player;
    public PoolManager10 pool;

    // 게임 전반을 관리하는 스크립트

    void Awake()
    {
        // GameManager를 게임 내에서 유일한 관리자로 만들고, 다른 스크립트에서 쉽게 접근할 수 있게 합니다.
        // static 변수인 instance에 자기 자신(this)을 할당합니다.
        // 사용 예: 다른 스크립트(예: 플레이어, 적)에서 GameManager.instance.gameTime과 같이 코드를 작성하여
        // 이 매니저의 변수나 함수를 바로 가져다 쓸 수 있습니다.
        instance = this;
    }
    
    void Update()
    {

        // 게임에 제한시간을 설정

        gameTime += Time.deltaTime;

        if(gameTime > maxGameTime)
        {
            gameTime = maxGameTime;
        }
    }

    
}

