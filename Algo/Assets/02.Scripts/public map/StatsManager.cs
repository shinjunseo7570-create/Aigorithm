using System.Collections;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.Audio;

public class StatsManager : MonoBehaviour
{
    [Header("스탯 메뉴 연결")]
    public GameObject statsMenu;

    [Header("플레이어 연결")]
    public PlayerStats player;

    [Header("스탯 표시 텍스트 연결")]
    public TextMeshProUGUI statusText;

    [Header("오디오 설정")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        if (statsMenu != null) statsMenu.SetActive(false);
        StartCoroutine(ShowStatusRoutine());
    }


    void Update()
    {
        // 'I' 키를 누르면 인벤토리 껐다 켰다 하기
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (statsMenu != null)
                statsMenu.SetActive(!statsMenu.activeSelf);
            audioSource.PlayOneShot(clickSound);
        }

    }

    IEnumerator ShowStatusRoutine()
    {
        // 잠시 대기
        yield return null;

        statusText.text =
            ($"{player.health} / {player.maxHealth}\n" + // 체력
            $"{player.currentStamina} /{ player.maxStamina}\n" + // 스태미나
            $"{player.attackPower}\n" + // 공격력
            $"{player.attackSpeed} / {player.maxAttackSpeed}\n" + // 공격 속도
            $"{player.moveSpeed} / {player.maxMoveSpeed}\n" + // 이동 속도
            $"{player.critRate}\n" + // 치명타 확률
            $"{player.attackRange}"); // 공격 사거리
    }
}
