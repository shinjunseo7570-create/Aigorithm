using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Sprint")]
    [SerializeField] float sprintDistance = 10f; // 사거리
    [SerializeField] float sprintDuration = 0.1f; // 무적 판정
    [SerializeField] float sprintCooltime = 2f; // 쿨타임 2초

    [Header ("마지막으로 접속한 Scene Tag")]
    public int lastStageNum = 1;

    // [SerializeField] float runSpeed = 10;
    public static int stemina = 100;
    Vector2 moveInput;
    Vector2 lastMoveDir = Vector2.right;

    Animator myAnimator;
    Rigidbody2D myRigidbody;
    Vector3 baseScale;

    

    bool isSprinting = false;
    public bool IsInvincible { get; private set; } // 무적 판정 확인
    float lastSprintTime = -999f;

    public bool isStunned = false; // 다른 곳(이동, 공격)에서 이 변수가 true면 동작 안 하게 막아야 함

    private static PlayerInteract instance;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        baseScale = transform.localScale;
    }

    void Update()
    {
        Run();
        FlipSprite();
    }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        /*Debug.Log(moveInput);*/
    }
    
    void OnSprint(InputValue value)
    {

        if (isStunned) return; // 스턴상태면 기각

        if (!value.isPressed) return;   // 누르면 대쉬
        if (isSprinting) return;    // 이미 대쉬 했으면 기각
        if (Time.time < lastSprintTime + sprintCooltime) return;    // 대쉬 쿨타임

        StartCoroutine(SprintRoutine());
    }

    void Run()
    {
        // 스턴 상태면 움직임 로직 차단하고 강제로 멈춤
        if (isStunned)
        {
            myRigidbody.linearVelocity = Vector2.zero; // 확실하게 정지. 이동 속도를 0으로 만듬.
            myAnimator.SetBool("IsRunning", false); // 달리기 애니메이션 끄기
            return; // 아래 코드를 실행하지 않고 나감
        }

        if (isSprinting) return;

        Vector2 playerVelocity = moveInput * moveSpeed; // new Vector2(moveInput.x * runSpeed, moveInput.y * runSpeed);
        myRigidbody.linearVelocity = playerVelocity;

        bool isMoving = playerVelocity.sqrMagnitude > 0.001f;
        myAnimator.SetBool("IsRunning", isSprinting);

        if(isMoving)
        {
            lastMoveDir = moveInput.normalized;
        }

        // bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon || Mathf.Abs(myRigidbody.linearVelocity.y) > Mathf.Epsilon;
        // myAnimator.SetBool("IsRunning", playerHasHorizontalSpeed);
    }
    
    IEnumerator SprintRoutine()
    {
        isSprinting = true;
        IsInvincible = true;
        lastSprintTime = Time.time;

        Vector2 dir = lastMoveDir;
        if (dir.sqrMagnitude < 0.01f)
        {
            dir = new Vector2(Mathf.Sign(transform.localScale.x), 0f);
        }

        float sprintSpeed = sprintDistance / sprintDuration; // 거 / 시 = 속
        float elapsed = 0f;

        myAnimator.SetBool("IsRunning", true);

        while(elapsed < sprintDuration)
        {
            myRigidbody.linearVelocity = dir * sprintSpeed;
            elapsed += Time.deltaTime;
            yield return null;
        }

        isSprinting = false;
        IsInvincible = false;

        // 대쉬 끝나고 바로 이동 가능하게 (스턴 상태가 아니라면)
        // 대쉬가 끝나고 스턴 걸렸을때 꼬임 방지
        if (!isStunned)
        {
            myRigidbody.linearVelocity = moveInput * moveSpeed; // 대쉬 끝나고 입력 따라 다시 일반 이동
        }
                                                         
    }

    void FlipSprite()
    {
        // 스턴 상태면 바라보는 방향도 안 바뀌게
        if (isStunned) return;

        float vx = myRigidbody.linearVelocity.x;
        if(Mathf.Abs(vx) > Mathf.Epsilon)
        {
            float sign = Mathf.Sign(vx);
            transform.localScale = new Vector3(Mathf.Abs(baseScale.x) * sign, baseScale.y, baseScale.z);
        }

        /*bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.linearVelocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2((Mathf.Sign(myRigidbody.linearVelocity.x)*5), 5f);
        }*/

    }

    // Enemy10에서 이 함수를 호출할 겁니다.
    public void GetStunned(float duration)
    {
        if (isStunned) return; // 이미 스턴 상태면 무시 (중복 방지)

        // 무적 상태(대쉬 중)라면 스턴 안 걸림
        if (IsInvincible) return;

        StartCoroutine(StunRoutine(duration));
    }

    IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        
        Debug.Log("플레이어 스턴 시작!");

        myRigidbody.linearVelocity = Vector2.zero;

        myAnimator.SetBool("IsRunning", false);

        yield return new WaitForSeconds(duration);

        isStunned = false;
        Debug.Log("플레이어 스턴 해제!");
    }
}
