using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Mimic : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    public RuntimeAnimatorController[] animCon;
    public Rigidbody2D target;
    public bool isMove = false;
    public bool isBoss = false;

    public static Action<Mimic> OnEnemyDead;

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
    Vector2 origin;
    int typeId;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
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
        if (isMove)
        {
            Chase();
        }
        else if (origin != null)
        {
            FixPosition();
        }

        // 3) 사정거리 안 + 쿨타임 끝났으면 공격 시작
        if (distance <= attackRange && attackTimer >= attackDelay && isMove)
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
        if (GameManager.instance != null && GameManager.instance.player != null)
        {
            target = GameManager.instance.player.GetComponent<Rigidbody2D>();
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
    }

    void FixPosition()
    {
        this.transform.position = origin;
    }

    public void SetPosition(Vector2 oripos)
    {
        origin = oripos;
    }

    public void Init(SpawnData07 data)
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
            isMove = true;
            SkillController skill = collision.GetComponent<SkillController>();

            if (skill == null)
                return;
            Debug.Log("스킬 데미지: " + skill.Damage);
            float finalDamage = skill.Damage;
            Debug.Log("최종 데미지: " +  finalDamage);
            Debug.Log("공격 받기 전 HP: " + health);
            health -= finalDamage;
            Debug.Log("공격 받은 후 HP: " + health);

            if (health <= 0)
            {
                Dead();
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

            if (player != null && !player.IsInvincible)
            {
                Damaged();
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttacking)
        {
            PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

            if (player != null && !player.IsInvincible)
            {
                Damaged();
            }
        }
    }

    void Damaged()
    {
        Debug.Log("GameOver");
        return;
    }

    void Dead()
    {
        isLive = false;

        OnEnemyDead?.Invoke(this);

        gameObject.SetActive(false);
    }
}
