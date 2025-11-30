using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy10 : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;

    public bool isBoss = false;

    public static Action<Enemy10> OnEnemyDead;

    public float attackRange;
    public float attackDelay = 1f;

    float attackTimer = 0f;

    bool isAttacking = false;
    public float attackAnimDuration = 0.5f;

    float spawnTime;
    public float spawnProtectTime = 0.3f;

    bool isLive = true;

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;
    Collider2D myCollider;

    [Header("흐려지는 시간 설정")]
    [SerializeField] float fadeDuration = 1.0f; // 흐려지는 시간 (기본값 1초)

    int typeId;

    // 몬스터 종류(typeId)와 공격의 속성(elem)을 비교해서 데미지를 2배로 주거나 아예 안 받게(0배) 합니다.
    float CalcElementMultiplier(ElementType elem)
    {
        // 데미지 연산
        float mult = 1f;

        if (elem == ElementType.Wind || elem == ElementType.Earth)
            mult = 0.5f;

        switch (typeId)
        {
            case 0: // 박쥐
                if (elem == ElementType.Fire)
                {
                    mult = 2f;
                }
                else if (elem == ElementType.Water)
                {
                    mult = 0f;
                }
                break;

            case 1: // 크랩
                if (elem == ElementType.Fire)
                {
                    mult = 0f;
                }
                else if (elem == ElementType.Water)
                {
                    mult = 2f;
                }
                break;

            case 2: // 골렘
                if (elem == ElementType.Fire || elem == ElementType.Water)
                {
                    mult = 2f;
                }
                break;
        }
        return mult;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        if (!isLive || target == null)
            return;

        float distance = Vector2.Distance(target.position, rigid.position);
        attackTimer += Time.fixedDeltaTime;

        // 1) 공격 중이면 자리에서 멈추고 애니메이션만 재생
        if (isAttacking)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        // 2) 기본 상태: 항상 플레이어 쪽으로 날아간다
        Chase();

        // 3) 사정거리 안 + 쿨타임 끝났으면 공격 시작
        if (distance <= attackRange && attackTimer >= attackDelay)
        {
            attackTimer = 0f;
            StartCoroutine(AttackRoutine());
        }
    }

    // 평소에 플레이어를 향해 날아가는 동작
    void Chase()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        // 평소 상태는 공격 아님
        anim.SetBool("isAttack", false);
    }

    void AttackPlayer()
    {
        Debug.Log("플레이어 공격");
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 공격 시작: 이동 멈추고, 공격 애니메이션
        rigid.linearVelocity = Vector2.zero;
        anim.SetBool("isAttack", true);

        // 필요하다면 타격 타이밍 맞춰서 약간 딜레이 줘도 됨
        // yield return new WaitForSeconds(0.2f);
        AttackPlayer();

        // 공격 애니메이션이 끝날 때까지 대기
        yield return new WaitForSeconds(attackAnimDuration);

        // 공격 끝나면 다시 평상시 상태로
        anim.SetBool("isAttack", false);
        isAttacking = false;
    }

    void LateUpdate()
    {
        if (!isLive || target == null)
            return;

        // 플레이어의 X축 값과 적의 X축 값을 비교하여 작으면 true
        spriter.flipX = target.position.x < rigid.position.x;
    }

    void OnEnable()
    {
        if (GameManager10.instance != null && GameManager10.instance.player != null)
        {
            target = GameManager10.instance.player.GetComponent<Rigidbody2D>();
        }
        else
        {
            target = null;
        }

        isLive = true;
        maxHealth = health;  // 혹시 maxHealth가 따로 세팅되어 있으면 이 줄 조절
        health = maxHealth;

        attackTimer = 0f;
        isAttacking = false;
        spawnTime = Time.time;

        // 재활용할 때 콜라이더와 색상을 원래대로 돌려놓음.
        if (myCollider != null) myCollider.enabled = true;
        if (spriter != null)
        {
            Color c = spriter.color;
            spriter.color = new Color(c.r, c.g, c.b, 1f);
        }
    }

    // 소환될 때, 데이터에 적힌 번호(spriteType)에 맞는 애니메이션으로 갈아입음
    public void Init(SpawnData10 data)
    {
        typeId = data.spriteType;

        anim.runtimeAnimatorController = animCon[data.spriteType];
        speed = data.Speed;
        maxHealth = data.Health;
        health = data.Health;
        attackRange = data.Range;

        float spawnDist = Vector2.Distance(target.position, rigid.position);
        Debug.Log($"[Enemy.Init] spawnDist = {spawnDist}");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Skill"))
        {
            SkillController skill = collision.GetComponent<SkillController>();

            if (skill == null)
                return;

            float multiplier = CalcElementMultiplier(skill.Element);
            float finalDamage = skill.Damage * multiplier;
            health -= finalDamage;

            Debug.Log($"[Enemy] type={typeId}, elem={skill.Element}, mult={multiplier}, dmg={skill.Damage} -> {finalDamage}");

            if (health <= 0)
            {
                Dead();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 벤시(Enemy10)는 공격 중이든 아니든 몸에 닿으면 스턴을 건다고 가정했습니다.
        // 만약 공격 모션 중에만 걸고 싶다면 && isAttacking 조건을 다시 넣으세요.
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

            // 무적 상태가 아닐 때만 스턴
            if (player != null && !player.IsInvincible)
            {
                // 데미지 함수(Damaged) 대신 스턴 함수 호출
                // 플레이어 스크립트에 추가한 GetStunned(1.0f)를 호출합니다.
                player.GetStunned(1.0f);

                // 1초동안 사라지는 연출을 시작합니다.
                StartCoroutine(DisappearRoutine(fadeDuration));
            }
        }
    }

    // 부딪힌 순간에만 스턴 적용하려면 주석처리
    /*
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

            if (player != null && !player.IsInvincible)
            {
                player.GetStunned(1.0f);
            }
        }
    }
    */

    public void TakeDamage(float amount)
    {
        if (!isLive) return;

        health -= amount;
        Debug.Log($"밴시 {amount}만큼 데미지 받음.");

        if (health <= 0f)
        {
            Dead();
        }
    }

    void Dead()
    {
        isLive = false;

        OnEnemyDead?.Invoke(this);

        gameObject.SetActive(false);
    }

    IEnumerator DisappearRoutine(float fadeDuration)
    {
        // 1. 즉각적인 물리 판정 제거
        isLive = false;
        rigid.linearVelocity = Vector2.zero; // 속도 0으로
        if (myCollider != null) myCollider.enabled = false; // 충돌 끄기

        StopCoroutine("AttackRoutine");
        anim.SetBool("isAttack", false);

        // 2. 천천히 시각적 사라짐 (페이드 아웃)
        float timer = 0f;
        Color startColor = spriter.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            spriter.color = new Color(startColor.r, startColor.g, startColor.b, newAlpha);
            yield return null;
        }

        // 3. 뒷정리
        spriter.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        OnEnemyDead?.Invoke(this);
        gameObject.SetActive(false);
    }
}
