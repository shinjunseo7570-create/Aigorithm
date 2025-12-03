using UnityEngine;
using System.Collections.Generic;
using TMPro;
// TMPro는 이제 DialogueManager 내부에서 처리하므로 여기서 직접 쓸 일은 줄어들지만, 
// 혹시 모를 상황을 위해 남겨둡니다.

public class DevilKnapsackShop : MonoBehaviour
{
    // 계약서(물건) 클래스
    [System.Serializable]
    public class SoulContract
    {
        public string name;
        public int cost;   // 대가 :  소모 체력 : 알고리즘에서의 무게
        public int value;  // 힘 : 공격력 증가량 : 알고리즘에서의 가치
    }

    [Header("설정")]
    public int contractCount = 5; // 생성할 계약서 개수

    [Header("시작 대화 설정")]
    [TextArea]
    public string startMessage;

    [Header("연결 (자동 기능 없음 / 필수 연결 필요)")]
    public TextMeshProUGUI healthDealText;    // 체력 계약서 텍스트
    public TextMeshProUGUI staminaDealText;    // 스태미나 계약서 텍스트

    public TextMeshProUGUI healthContractsText; // 체력 계약서 목록
    public TextMeshProUGUI staminaContractsText; // 스태미나 계약서 목록

    [Header("연결 (자동 기능 있음)")]
    public PlayerStats player; // PlayerStats 자동 연결
    public DialogueManager dialogueManager; // dialougeManager 자동 연결

    // 데이터 저장소
    // healthContracts: 체력을 대가로 하는 계약서 목록
    // staminaContracts: 스태미나를 대가로 하는 계약서 목록
    // bestHealthDeal: 알고리즘으로 계산한 최적 체력 계약 조합
    // bestStaminaDeal: 알고리즘으로 계산한 최적 스태미나 계약 조합
    private List<SoulContract> healthContracts = new List<SoulContract>();
    private List<SoulContract> staminaContracts = new List<SoulContract>();
    private List<SoulContract> bestHealthDeal = new List<SoulContract>();
    private List<SoulContract> bestStaminaDeal = new List<SoulContract>();

    // 구매 여부 체크 변수
    private bool isHealthSold = false;
    private bool isStaminaSold = false;

    void Start()
    {
        // PlayerStats 자동 찾기
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            // PlayerStats 가져오기
            if (playerObject != null)
            {
                player = playerObject.GetComponent<PlayerStats>();
            }

            // 디버깅
            if (player == null)
            {
                // PlayerStats를 찾을 수 없다면
                Debug.LogWarning("FindGameObjectWithTag로 플레이어(PlayerStats)를 찾을 수 없음");
            }
        }

        // dialougeManager 자동 찾기
        if (dialogueManager == null)
        {
            dialogueManager = FindFirstObjectByType<DialogueManager>();

            // 디버깅
            if (dialogueManager == null)
            {
                Debug.LogWarning("Scene에 DialogueManager가 없음");
            }
        }

        // 상점 열기 함수 실행
        OpenShop();
    }

    public void OpenShop()
    {
        isHealthSold = false;
        isStaminaSold = false;

        // 랜덤 계약서 생성 (체력용 / 스태미나용)
        GenerateContracts(healthContracts, true);  // true = 체력 기반
        GenerateContracts(staminaContracts, false); // false = 스태미나 기반

        // 배낭 문제 알고리즘 실행
        // 최적의 계약 묶음(배낭) 계산
        SolveHealthKnapsack();
        SolveStaminaKnapsack();

        // 대사 출력
        if (dialogueManager != null)
        {
            dialogueManager.ShowMessage("\"생명력과 기력... 훌륭한 댓가가 되겠군. 최대한 담아두었다.\"");
        }

        // UI 갱신
        UpdateResultUI();
    }

    // 계약서 생성 함수
    void GenerateContracts(List<SoulContract> list, bool isHealth)
    {
        // list로 받아오는 것: 계약서 목록 저장소(healthContracts/staminaContracts)
        // 체력 계약서는 isHealth를 true로 받아오고 스태미나 계약서는 false로 받아오기 때문

        // list를 Clear로 초기화
        list.Clear();

        // isHealth가 true라면 플레이어의 최대 체력을, false라면 최대 스태미나를 기준으로 계산
        int maxValue = isHealth ? player.maxHealth : player.maxStamina;

        // 설정한 개수만큼 for문으로 계약서를 생성
        for (int i = 0; i < contractCount; i++)
        {
            // 빈 계약서를 메모리에 생성
            SoulContract contract = new SoulContract();

            // 이름은 각각 다르고, 번호로 계약서를 구분함
            contract.name = isHealth ? $"피의 계약 #{i + 1}" : $"기력의 계약 #{i + 1}";

            // 비용: 최대치의 5% ~ 40%
            //Mathf.Max(float a,float b)는 둘 중에서 더 큰 값을 반환함.
            // 최소 비용: 전체 통의 5% 정도 (최소 1 이상)
            int minCost = Mathf.Max(1, (int)(maxValue * 0.05f));
            // 최대 비용: 상한선 설정. 40% 이상 떼어가면 너무 힘듬. (최소 5 이상)
            int maxCost = Mathf.Max(5, (int)(maxValue * 0.4f));

            // 위에서 정한 minCost ~ maxCost 범위 안에서 랜덤하게 비용을 뽑습니다. 
            // ex) 체력 100이라면 5 ~ 40 사이의 random 값으로 설정
            contract.cost = Random.Range(minCost, maxCost);

            // 가치 계산
            // 기본 효율: 체력의 가치를 높게(1.0), 스태미나는 살짝 아래(0.5)로
            float efficiencyBase = isHealth ? 1.0f : 0.5f;
            // 랜덤 효율: "비용 대비 얼마나 이득인가?"를 랜덤하게 정함. (0.8배 ~ 2.5배)
            // 계약서마다 비용이 비싼데 효율이 나쁘고, 싼데 효율이 좋을 수 있음
            float efficiency = Random.Range(0.8f, 2.5f) * efficiencyBase;

            // 공격력 증가량 = 비용 * 효율
            // Mathf.RoundToInt: 소숫점 첫 번째 자리에서 반올림. int 반환
            contract.value = Mathf.RoundToInt(contract.cost * efficiency);

            // 완성된 계약서를 리스트(계약서 목록)에 추가
            list.Add(contract);
        }
    }

    // 배낭 문제 알고리즘 : 체력 (최소 1 남김)
    void SolveHealthKnapsack()
    {
        // 이미 결과가 들어있다면 초기화
        bestHealthDeal.Clear();

        // 배낭 용량(플레이어의 체력 기준) 설정
        // 현재 체력의 1만큼을 남기고 나머지를 최대 용량으로 사용 가능하게
        int capacity = player.maxHealth - 10;

        // 체력이 10 이하라면 거래 할 수 없음(return)
        if (capacity <= 0) return;

        // 실제 알고리즘을 실행하러 감
        // 받아온 결과를 bestHealthDeal 리스트에 저장
        bestHealthDeal = GetBestCombination(healthContracts, capacity);
    }

    // 배낭 문제 알고리즘 : 스태미나 (최소 20 남김)
    void SolveStaminaKnapsack()
    {
        // 이미 결과가 들어있다면 초기화
        bestStaminaDeal.Clear();

        // 배낭 용량(플레이어의 체력 기준) 설정
        // 현재 스태미나의 20만큼을 남기고 나머지를 최대 용량으로 사용 가능하게
        // 20 기준: 마지막 스테이지에서 가장 먼 상점 기준으로 마지막까지 도달할 수 있는 최소 스태미나
        int capacity = player.currentStamina - 20;

        // 스태미나가 20 이하라면 거래 할 수 없음(return)
        if (capacity <= 0) return;

        // 실제 알고리즘을 실행하러 감
        // 받아온 결과를 bestStaminaDeal 리스트에 저장
        bestStaminaDeal = GetBestCombination(staminaContracts, capacity);
    }

    // DP 알고리즘 함수
    // 복잡한 문제를 여러 개의 작은 하위 문제로 나누고, 그 해답을 저장해 두었다가 다시 사용해 효율을 높이는 알고리즘
    // options: 계약서 목록(cost, value 포함), capacity: 배낭 용량
    List<SoulContract> GetBestCombination(List<SoulContract> contracts, int capacity)
    {
        // 물건(계약서)의 개수
        int n = contracts.Count;

        // dp 테이블 생성
        // dp[i, j]: i번째 물건까지만 고려했을 때, 배낭 용량이 j일 때 얻을 수 있는 최대 가치
        // 크기가 [n+1, capacity+1]인 이유는 물건 없는 상태(0)와 용량이 0인 경우를 포함하기 위해
        int[,] dp = new int[n + 1, capacity + 1]; // [계약서의 개수 + 1, 배낭 용량 + 1]

        // DP 채우기 (작은 문제부터 풀기)
        // i: 현재 고려 중인 물건의 순서 (1번째 물건부터 n번째까지 물건)
        for (int i = 1; i <= n; i++)
        {
            // i번째 물건의 무게와 가치를 가져옴
            // 리스트 인덱스는 0부터 시작하므로 [i - 1]
            int weight = contracts[i - 1].cost; // 무게 : 대가
            int value = contracts[i - 1].value; // 가치 : 공격력 증가량

            // j는 현재 가정하는 배낭의 임시 용량
            // 0부터 capacity(용량)까지 1씩 늘려가며 계산)
            for (int j = 1; j <= capacity; j++)
            {

                // 현재 물건의 무게(weight)가 현재 가상의 용량(j)보다 작거나 같다면 -> 넣을 수 있음
                if (weight <= j)
                {
                    // 두 case 중 더 이득인 것을 선택

                    // A안: 이 물건 안 넣기
                    // dp[i-1, j] : 이전 물건까지 계산했던 값 그대로 가져옴.

                    // B안: 이 물건 넣기
                    // value           +  dp[i - 1, j - weight]:
                    // 이 물건의 가치  +  이 물건의 무게만큼 뺀 나머지 공간에 채울 수 있는 최대 가치
                    dp[i, j] = Mathf.Max(dp[i - 1, j], dp[i - 1, j - weight] + value);
                }

                // 현재 물건이 가방보다 무겁다면 -> 넣을 수 없음
                else
                {
                    // 강제 A안 (이전 값 유지)
                    dp[i, j] = dp[i - 1, j];
                }
                // 반복문 종료 후 dp[n, capacity]에는 모든 물건을 고려했을 때의 최대 공격력이 들어가 있음.
            }
        }

        // 역추적 알고리즘
        // 최대 공격력 숫자는 확인했지만 무슨 계약서를 사야 하는지 알아내는 부분
        List<SoulContract> result = new List<SoulContract>(); // 최종 선택된 계약서들을 담을 리스트 생성

        // 남은 용량 (추적하면서 점점 줄어듦)
        int remainCapacity = capacity;

        // 표의 맨 오른쪽 아래 결과(n)부터 시작해 거꾸로 1까지 for문
        for (int i = n; i > 0; i--)
        {
            // 현재의 가치가 이전 가치와 다르다면
            // 값이 바뀌었다는 건 i번째 물건을 가방에 넣었다는 뜻
            if (dp[i, remainCapacity] != dp[i - 1, remainCapacity])
            {
                // 선택된 계약서를 원본 리스트에서 찾기
                // contracts[i - 1]인 이유: dp는 +1을 의도해서 작성(두 변수가 0인 경우를 생각, 위쪽 주석 참고)했기 때문에
                // 실제로 contracts(원본 계약서) 에서는 하나 작은 값으로 진입해야 하기 때문
                SoulContract selected = contracts[i - 1];
                // result에 담기
                result.Add(selected);
                // 물건을 넣었으니, 그만큼 무게(cost)를 뺍니다.
                // 나머지 무게로 뭘 넣었는지 또 찾으러 감
                remainCapacity -= selected.cost;
            }
            // 값이 같다면 i번째 물건은 안 넣었다는 뜻
        }
        // 알고리즘 결과로 만들어진 계약서 묶음을 반환
        return result;
    }

    // UI 텍스트 통합 표시
    void UpdateResultUI()
    {
        if (healthDealText == null && staminaDealText == null) return;

        // 문자열은 반드시 ""로 초기화해야 += 연산 가능
        string healthText = "";
        string staminaText = "";

        // 계약서 이름 로그
        string healthLog = "";
        string staminaLog = "";

        // 체력 거래
        if (isHealthSold)
        {
            healthText += "<color=grey>(거래 완료됨)</color>\n";
            healthLog += "<color=grey>(되돌리기는 늦었습니다...)</color>\n";
        }
        else if (bestHealthDeal.Count > 0)
        {
            int totalCost = 0, totalValue = 0;
            foreach (var item in bestHealthDeal)
            {
                totalCost += item.cost;
                totalValue += item.value;
                // 체력 계약서 목록 추가
                healthLog += $"- {item.name} (체력 {item.cost})\n";
            }
            healthText += $"체력 소모: <color=red>{totalCost}</color>\n 이득: <color=green>공격력 +{totalValue}</color>\n";
            healthText += $"<size=80%>(남은 체력: {player.maxHealth - totalCost})</size>\n";
        }
        else
        {
            healthText += "(최대 체력이 너무 적음: 최소 1 필요)\n";
            healthLog += "(가능한 계약이 없음)\n";
        }

        // 스태미나 거래
        if (isStaminaSold)
        {
            staminaText += "<color=grey>(거래 완료됨)</color>\n";
            staminaLog += "<color=grey>(되돌리기는 늦었습니다...)</color>\n";
        }
        else if (bestStaminaDeal.Count > 0)
        {
            int totalCost = 0, totalValue = 0;
            foreach (var item in bestStaminaDeal)
            {
                totalCost += item.cost;
                totalValue += item.value;
                // 스태미나 계약서 목록 추가
                staminaLog += $"- {item.name} (영혼 {item.cost})\n";
            }
            staminaText += $"스태미나 소모: <color=yellow>{totalCost}</color>\n 이득: <color=green>공격력 +{totalValue}</color>\n";
            staminaText += $"<size=80%>(남은 스태미나: {player.currentStamina - totalCost})</size>\n";
        }
        else
        {
            staminaText += "(스태미나가 부족함: 최소 20 필요)\n";
            staminaLog += "(가능한 계약이 없음)\n";
        }

        healthDealText.text = healthText;
        staminaDealText.text = staminaText;
        healthContractsText.text = healthLog;
        staminaContractsText.text = staminaLog;
    }

    public void BuyHealthDeal()
    {
        if (player == null || isHealthSold || bestHealthDeal.Count == 0) return;

        int totalCost = 0;
        int totalReward = 0;
        foreach (var item in bestHealthDeal) { totalCost += item.cost; totalReward += item.value; }

        player.maxHealth -= totalCost;
        // 현재 체력이 최대 체력보다 크다면
        if (player.health > player.maxHealth)
        {
            player.health = player.maxHealth;
        }
        player.attackPower += totalReward;

        isHealthSold = true;
        dialogueManager.ShowMessage("\"흘린 피로 힘을 주도록 하지.\"");
        UpdateResultUI();
    }

    public void BuyStaminaDeal()
    {
        if (player == null || isStaminaSold || bestStaminaDeal.Count == 0) return;

        int totalCost = 0;
        int totalReward = 0;
        foreach (var item in bestStaminaDeal) { totalCost += item.cost; totalReward += item.value; }

        player.currentStamina -= totalCost;
        player.attackPower += totalReward;

        isStaminaSold = true;
        dialogueManager.ShowMessage("\"좋은 거래였다. 지치지 않도록 조심해야 할 거야.\"");
        UpdateResultUI();
    }
}