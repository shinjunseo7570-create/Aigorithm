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
        if (!value.isPressed) return;   // 누르면 대쉬
        if (isSprinting) return;    // 이미 대쉬 했으면 기각
        if (Time.time < lastSprintTime + sprintCooltime) return;    // 대쉬 쿨타임

        StartCoroutine(SprintRoutine());
    }

    void Run()
    {
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

        myRigidbody.linearVelocity = moveInput * moveSpeed; // 대쉬 끝나고 입력 따라 다시 일반 이동
                                                         
    }

    void FlipSprite()
    {
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
}
