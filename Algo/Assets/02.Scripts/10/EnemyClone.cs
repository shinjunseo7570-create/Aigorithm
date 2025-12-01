using System.Collections;
using UnityEngine;

public class EnemyClone : MonoBehaviour
{
    [Header("Clone Stats")]
    public float speed = 3f;
    public float health;       // 플레이어 체력 복사
    public float attackPower;  // 플레이어 공격력 복사
    public float attackRange = 1.5f;

    bool isLive = true;
    bool isAttacking = false;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    Rigidbody2D target; // 플레이어

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // 게임 시작 시 플레이어 정보를 복사하기 위한 코루틴 실행
        StartCoroutine(CopyPlayerInfoRoutine());
    }

    // 플레이어의 능력치와 외형을 가져오는 함수
    IEnumerator CopyPlayerInfoRoutine()
    {
        // 플레이어가 로딩될 때까지 잠깐 대기
        yield return null;

        if (GameManager10.instance.player != null)
        {
            PlayerInteract playerScript = GameManager10.instance.player;
            GameObject playerObj = playerScript.gameObject;

            // 타겟 설정
            target = playerObj.GetComponent<Rigidbody2D>();

            // 외형(애니메이션) 복사
            // 플레이어가 쓰는 애니메이터 컨트롤러를 그대로 가져옴
            if (playerObj.GetComponent<Animator>() != null)
            {
                anim.runtimeAnimatorController = playerObj.GetComponent<Animator>().runtimeAnimatorController;
            }

            // 플레이어 스프라이트 복사
            if (playerObj.GetComponent<SpriteRenderer>() != null)
            {
                spriter.sprite = playerObj.GetComponent<SpriteRenderer>().sprite;
            }

            // 클론의 색은 검정색으로
            spriter.color = new Color(0f, 0f, 0f);

            // 스탯 복사 (플레이어 스크립트에 변수가 있다고 가정)
            // PlayerInteract에 maxHealth, damage 변수가 있어야 합니다.
            // 없다면 임의의 값을 넣거나 변수를 만들어주세요.

            // health = playerScript.maxHealth; 
            // attackPower = playerScript.damage;

            // (임시) 변수가 없으면 기본값 적용
            health = 100f;
            attackPower = 10f;
        }
    }

    void FixedUpdate()
    {
        if (!isLive || target == null || isAttacking) return;

        // 추격 로직
        Vector2 dirVec = target.position - rigid.position;
        float dist = dirVec.magnitude;

        if (dist <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
            rigid.MovePosition(rigid.position + nextVec);

            // 이동 애니메이션 (플레이어랑 같은 파라미터 이름을 쓴다고 가정)
            anim.SetBool("IsRunning", true);
        }
    }

    void LateUpdate()
    {
        if (!isLive || target == null) return;

        // 바라보는 방향 전환
        spriter.flipX = target.position.x < rigid.position.x;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("IsRunning", false);

        // 공격 애니메이션 등 실행 (플레이어 애니메이터 구조에 따라 다름)
        // anim.SetTrigger("Attack"); 

        yield return new WaitForSeconds(0.5f); // 공격 딜레이

        // 실제 데미지 주는 로직은 OnTriggerEnter 등에서 처리하거나 여기서 거리 체크

        isAttacking = false;
    }

    public void TakeDamage(float amount)
    {
        if (!isLive) return;

        health -= amount;
        Debug.Log($"클론 {amount}만큼 데미지 받음.");

        if (health <= 0f)
        {
            Dead();
        }
    }

    void Dead()
    {
        isLive = false;
        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("IsRunning", false);

        GameManager10.instance.GameWin(
        GameManager10.instance.GetPlayer()); 

        Destroy(gameObject); // 혹은 사라지는 연출
    }
}