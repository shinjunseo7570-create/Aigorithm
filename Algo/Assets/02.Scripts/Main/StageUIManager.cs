using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class StageUIManager : MonoBehaviour
{
    [Header("UI 오브젝트 연결")]
    public TextMeshProUGUI titleText;        // 스테이지 1 : 스테이지 이름
    public TextMeshProUGUI descriptionText;  // 맵 설명
    public TextMeshProUGUI enemyListText;    // 출현 적 리스트
    public TextMeshProUGUI conditionText;    // 클리어 조건
    public TextMeshProUGUI rewardText;       // 획득 포인트
    public TextMeshProUGUI staminaText;      // 소모 스태미나

    // 스테이지 설명 UI에 텍스트 쓰기
    // StageNode에서 사용(각 토글에 적용되어있음)
    public void UpdateStageInfoUI(int id, string name, string desc, List<string> enemies, string condition, int reward, int stamina)
    {
        // 제목 (맵 ID + 이름)
        if (titleText != null)
        {
            titleText.text = $"스테이지 {id} : {name}";
        }

        // 2. 설명
        if (descriptionText != null)
        {
            descriptionText.text = desc;
        }

        // 3. 적 리스트 (리스트를 문자열로 변환)
        if (enemyListText != null)
        {
            string enemyString = (enemies != null && enemies.Count > 0) ? string.Join(", ", enemies) : "없음";
            enemyListText.text = enemyString;
        }

        // 4. 클리어 조건
        if (conditionText != null)
        {
            conditionText.text = condition;
        }

        // 5. 보상 포인트
        if (rewardText != null)
        {
            rewardText.text = reward.ToString(); // 숫자만 깔끔하게 표시 (필요시 "Pt" 등을 붙여도 됨)
        }

        // 6. 스태미나
        if (staminaText != null)
        {
            staminaText.text = $"{stamina.ToString()} 소모";
        }
    }
}