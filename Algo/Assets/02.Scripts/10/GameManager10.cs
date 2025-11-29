using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager10 : MonoBehaviour
{


    [Header("게임 설정")]
    public float limitTime = 60f; // 제한 시간 (예: 60초)
    bool isGameOver = false;

    public static GameManager10 instance;
    public PlayerInteract player;
    public PoolManager10 pool;

    [Header("UI 연결")]
    public GameObject gameClearScreen;
    public GameObject gameOverScreen;

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


        // 게임 오버나 클리어 상태면 타이머 멈춤
        if (isGameOver) return;

        // 1. 시간 줄이기
        limitTime -= Time.deltaTime;

        // (선택사항) UI에 남은 시간 표시
        // if(timeText != null) timeText.text = $"Time: {limitTime:F1}";

        // 2. 시간이 0보다 작아지면? -> 실패(Fail)!
        if (limitTime <= 0)
        {
            GameFail();
        }
    }
    public void GameWin()
    {
        Debug.Log("축하합니다! 게임 클리어!");

        // 게임 시간 멈추기 (선택사항)
        // Time.timeScale = 0;
        // Time.timeScale = 0;으로 시간을 멈춘 뒤에, '다시하기(Restart)' 버튼을 눌러서
        // 씬(Scene)을 재시작하면 오류남 그래서
        // Time.timeScale = 1f; 를 Awake나 Start 할 때 넣어줘야 함

        // 클리어 화면 띄우기

        gameClearScreen.SetActive(true);
    }

    public void GameFail()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("게임 오버 (시간 초과 or 사망)");
        Time.timeScale = 0; // 시간 정지
        if (gameOverScreen != null) gameOverScreen.SetActive(true);
    }
}

