using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Enemy06 : MonoBehaviour
{
    public float speed;
    public float health;
    public float maxHealth;
    
    public Rigidbody2D target;

    public bool isBoss = false;

    public static Action<Enemy06> OnEnemyDead;

    public float attackRange;
    public float attackDelay = 1f;

    float attackTimer = 0f;

    bool isAttacking = false;
    public float attackAnimDuration = 0.5f;

    float spawnTime;
    public float spawnProtectTime = 0.3f;

    bool isLive = true;

    public float ATK = 10f;


    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriter;

    int typeId;

    public Sprite cursedStatueSprite_ATtack;
    public Sprite cursedStatueSprite;
    public SpriteRenderer[] statueRenderers;

    Sprite[] originalSprites;


    
    bool blinkDangerActive = false;        // 눈 켜져 있을 때 움직이면 안됨
    bool blinkDamageApplied = false;       // 데미지 한번만 줄려고 만듦
    Vector2 blinkStartPlayerPos;          // 눈 켜졌을 때 플레이어 위치 계산하기 위한 좌표
    public float blinkMoveThreshold = 0.01f; // 움직이면 뎀지

    bool isSmashAttack = false;   // 내려찍는 중인지
    bool smashHitPlayer = false;  // 내려찍는 동안 플레이어와 충돌했는지
    bool isStunnedEnemy = false;  

    Vector3 originPosition;
    bool originSaved = false;

    void Start()
    {
        originPosition = transform.position;
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();

        if (statueRenderers != null && statueRenderers.Length > 0)
        {
            originalSprites = new Sprite[statueRenderers.Length];
            for (int i = 0; i < statueRenderers.Length; i++)
            {
                if (statueRenderers[i] != null)
                    originalSprites[i] = statueRenderers[i].sprite;
            }
        }
    }

    void FixedUpdate()
    {
        if (!isLive || target == null)
            return;

        if (isStunnedEnemy)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }

        if (isSmashAttack)
        {
            rigid.linearVelocity = Vector2.zero;
            return;
        }


        if (blinkDangerActive && !blinkDamageApplied)
        {
            float moved = Vector2.Distance(target.position, blinkStartPlayerPos);
            if (moved > blinkMoveThreshold)
            {
                
                PlayerInteract player = target.GetComponent<PlayerInteract>();
                if (player != null && !player.IsInvincible)
                {
                    PlayerStats stats = target.GetComponent<PlayerStats>();
                    if (stats != null)
                    {
                        Damaged(stats);  
                        Debug.Log("[Enemy06] 눈 켜진 동안 움직여서 데미지!");
                    }
                }

                blinkDamageApplied = true;
            }
        }

        float distance = Vector2.Distance(target.position, rigid.position);
        attackTimer += Time.fixedDeltaTime;

        
    }

    
    void Chase()
    {
        Vector2 dirVec = target.position - rigid.position;
        Vector2 nextVec = dirVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);

        
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        float duration = 2f;
        float time = 0f;

        Color c = spriter.color;
        float startAlpha = c.a;
        float endAlpha = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            spriter.color = c;

            yield return null;
        }

        
        c.a = endAlpha;
        spriter.color = c;
    }

    IEnumerator FadeInRoutine()
    {
        float duration = 2f;
        float time = 0f;

        Color c = spriter.color;
        float startAlpha = c.a;
        float endAlpha = 1f;    

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            spriter.color = c;

            yield return null;
        }

        
        c.a = endAlpha;
        spriter.color = c;
    }

    void AttackPlayer()
    {
        Debug.Log("플레이어 공격");
    }

    

    void LateUpdate()
    {
        if (!isLive || target == null)
            return;

        
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

        attackTimer = 0f;
        isAttacking = false;
        spawnTime = Time.time;

        
        
        
        

        StartCoroutine(BlinkAfterSpawn());
    }

    IEnumerator BlinkAfterSpawn()
    {
        
        yield return new WaitForSeconds(2f);

        

        yield return StartCoroutine(BlinkRoutine());
    }

    IEnumerator BlinkRoutine()
    {
        if (target == null)
            yield break;

        
        yield return StartCoroutine(FadeOutRoutine());

        yield return new WaitForSeconds(0.5f);

        
        blinkDangerActive = true;
        blinkDamageApplied = false;
        blinkStartPlayerPos = target.position;

        yield return new WaitForSeconds(1f);


        
        yield return StartCoroutine(FadeInRoutine());

        
        blinkDangerActive = false;

        yield return StartCoroutine(MoveToPlayerY(12f, 3f));

        yield return StartCoroutine(MoveToPlayerX(6f));

        SetCursedSpriteAttack();

       yield return StartCoroutine(Smash(0.5f, 2f, 2f));
    }

    IEnumerator MoveToPlayerY(float distance, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(0, distance, 0);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
    }
    IEnumerator MoveToPlayerX(float search)
    {
        if (target == null)
            yield break;

        float dir = -1f;

        float prevDiff = transform.position.x - target.position.x;
        float maxTime = 10f;
        float elapsed = 0f;

        while(elapsed < maxTime)
        {
            elapsed += Time.deltaTime;

            Vector3 pos = transform.position;
            pos.x += dir * search * Time.deltaTime;
            transform.position = pos;

            if (target == null)
                yield break;

            float currentDiff = pos.x - target.position.x;

            
            if (prevDiff == 0f || currentDiff == 0f || (prevDiff * currentDiff < 0f))
            {
                
                transform.position = new Vector3(target.position.x, transform.position.y, transform.position.z);
                break;
            }

            prevDiff = currentDiff;
            yield return null;
        }

        
    }

    IEnumerator Smash(float duration, float stunDuration, float returnDuration)
    {
        if (target == null)
            yield break;

        isSmashAttack = true;
        smashHitPlayer = false;   

        Vector3 startPos = transform.position;

        float targetY = target.position.y;
        Vector3 endPos = new Vector3(startPos.x, targetY, startPos.z);

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            
            transform.position = Vector3.Lerp(startPos, endPos, t);

            if (!smashHitPlayer)
            {
                Collider2D hit = Physics2D.OverlapCircle(transform.position, 0.8f, LayerMask.GetMask("Player"));
                if (hit != null)
                    smashHitPlayer = true;
            }

            yield return null;
        }

        transform.position = endPos;

        
        isSmashAttack = false;

        



        
        if (smashHitPlayer)
        {
            

            PlayerStats stats = target.GetComponent<PlayerStats>();
            PlayerInteract pi = target.GetComponent<PlayerInteract>();

            if (stats != null)
                Damaged(stats);
            if (pi != null)
                pi.GetStunned(stunDuration);

            SetCursedSprite();

            Debug.Log("Hit → Return!");
            yield return StartCoroutine(Return(returnDuration));
        }
        else
        {
            

            Debug.Log("Miss → Enemy stun → Return");
            yield return StartCoroutine(EnemyStunRoutine(stunDuration));

            SetCursedSprite();

            yield return StartCoroutine(Return(returnDuration));
        }



        
    }

    IEnumerator Return(float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = originPosition;

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;

        if (isLive)
        {
            yield return new WaitForSeconds(1f);

            StartCoroutine(BlinkRoutine());
        }
    }

    IEnumerator EnemyStunRoutine(float duration)
    {
        isStunnedEnemy = true;
        rigid.linearVelocity = Vector2.zero;


        yield return new WaitForSeconds(duration);

        isStunnedEnemy = false;
    }



    void SetCursedSprite()
    {
        if (statueRenderers == null || statueRenderers.Length == 0)
            return;

        if (statueRenderers.Length > 0 &&
            statueRenderers[0] != null &&
            cursedStatueSprite != null)
        {
            statueRenderers[0].sprite = cursedStatueSprite;
        }

        
        if (originalSprites != null &&
            originalSprites.Length > 1 &&
            statueRenderers.Length > 1 &&
            statueRenderers[1] != null)
        {
            statueRenderers[1].sprite = originalSprites[1];
        }
    }

    void SetCursedSpriteAttack()
    {
        if (cursedStatueSprite_ATtack == null)
            return;

        if (statueRenderers == null || statueRenderers.Length == 0)
            return;

        foreach (var sr in statueRenderers)
        {
            if (sr != null)
                sr.sprite = cursedStatueSprite_ATtack;
        }
    }
    




    public void Init(SpawnData data)
    {
        typeId = data.spriteType;

        
        speed = data.Speed;
        maxHealth = data.Health;
        health = data.Health;
        attackRange = data.Range;
        ATK = data.ATK;

        float spawnDist = Vector2.Distance(target.position, rigid.position);
        Debug.Log($"[Enemy.Init] spawnDist = {spawnDist}");

        Enemy06_Hp hp = GetComponentInChildren<Enemy06_Hp>();
        if(hp != null)
        {
            hp.Setup(this);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Skill"))
            return;

        if (collision.CompareTag("Skill"))
        {
            SkillController skill = collision.GetComponent<SkillController>();

            if (skill == null)
                return;


            float finalDamage = skill.Damage * 0.5f;


            TakeDamage(finalDamage);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isSmashAttack)
            {
                smashHitPlayer = true;
            }

            
            if (isAttacking)
            {
                PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

                if (player != null && !player.IsInvincible)
                {
                    PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
                    if (stats != null)
                    {
                        Damaged(stats);
                    }
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isSmashAttack)
            {
                smashHitPlayer = true;
            }

            if (isAttacking)
            {
                PlayerInteract player = collision.gameObject.GetComponent<PlayerInteract>();

                if (player != null && !player.IsInvincible)
                {
                    PlayerStats stats = collision.gameObject.GetComponent<PlayerStats>();
                    if (stats != null)
                    {
                        Damaged(stats);
                    }
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        if (!isLive) return;

        health -= amount;
        Debug.Log($"enemy {amount}만큼 데미지 받음.");

        if(health <= 0f)
        {
            Dead();
        }
    }

    void Damaged(PlayerStats playerStats)
    {
        playerStats.TakeDamage(ATK);
    }

    void Dead()
    {
        isLive = false;

        OnEnemyDead?.Invoke(this);

        gameObject.SetActive(false);
    }
}
