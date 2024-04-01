using System;
using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class Animation : MonoBehaviour
{
    private Animator animator;
    public Movement movement; // Thêm reference đến script Movement
    public ThirdPersonCamera camera;

    // Các biến để xác định trạng thái điều khiển
    private bool Walking = false;
    private bool Running = false;
    private bool Jumping = false;
    private bool DoubleJumping = false;
    private bool LeftStrafeWalking = false;
    private bool LeftStrafeRun = false;
    private bool RightStrafeWalking = false;
    private bool RightStrafeRun = false;
    private bool WalkingBackward = false;
    private bool RunningBackward = false;
    private bool Falling = false; // Thêm biến để xác định rơi tự do

    // Các biến để xác định trạng thái dodge
    private bool DodgeFront = false;
    private bool DodgeBack = false;
    private bool DodgeRight = false;
    private bool DodgeLeft = false;

    //biến của animtion combat
    private bool isAttacking = false;
    public bool IsAttacking{ get { return isAttacking; } }
    private int comboCount = 0;
    private float timeSinceLastClick = 0f;
    public float comboResetTime = 1.5f; // Thời gian để reset combo về trạng thái ban đầu
    public PlayerHealth playerHealth;
    private bool isBlockAttacking = false;
    public bool IsBlockAttacking { get { return isBlockAttacking; } set { isBlockAttacking = value; } }

    // Start is called before the first frame update
    void Start()
    {
        // Lấy reference tới Animator component
        animator = GetComponent<Animator>();
        // Lấy reference tới script Movement
        movement = GetComponentInParent<Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        //---------------------Biến thiết lập animation------------------------
        // Xác định trạng thái của các hành động dựa trên các biến đã cung cấp
        Walking = Input.GetKey("w") || Input.GetKey(KeyCode.UpArrow);
        Running = Input.GetKey("left shift") && Walking;    
        Jumping = Input.GetKey("space");
        LeftStrafeWalking = Input.GetKey("a") || Input.GetKey(KeyCode.LeftArrow);
        LeftStrafeRun = Input.GetKey("left shift") && LeftStrafeWalking;
        RightStrafeWalking = Input.GetKey("d") || Input.GetKey(KeyCode.RightArrow);
        RightStrafeRun = Input.GetKey("left shift") && RightStrafeWalking;
        WalkingBackward = Input.GetKey("s") || Input.GetKey(KeyCode.DownArrow);
        RunningBackward = Input.GetKey("left shift") && WalkingBackward;

        //-----------------animation di chuyển---------------------------------
        // Áp dụng trạng thái này vào Animator để chuyển đổi giữa các animation
        animator.SetBool("Walking", Walking);
        animator.SetBool("Running", Running);
        animator.SetBool("Jumping", Jumping);
        animator.SetBool("LeftStrafeWalking", LeftStrafeWalking);
        animator.SetBool("LeftStrafeRun", LeftStrafeRun);
        animator.SetBool("RightStrafeWalking", RightStrafeWalking);
        animator.SetBool("RightStrafeRun", RightStrafeRun);
        animator.SetBool("WalkingBackward", WalkingBackward);
        animator.SetBool("RunningBackward", RunningBackward);

        //-----------------animation di chuyển chi tiết------------------------
        animator.SetBool("WalkBackLeft45", WalkingBackward && LeftStrafeWalking);
        animator.SetBool("WalkFrontLeft45", Walking && LeftStrafeWalking);
        animator.SetBool("WalkBackRight45", WalkingBackward && RightStrafeWalking);
        animator.SetBool("WalkFrontRight45", Walking && RightStrafeWalking);

        animator.SetBool("RunBackLeft45", RunningBackward && LeftStrafeRun);
        animator.SetBool("RunFrontLeft45", Running && LeftStrafeRun);
        animator.SetBool("RunBackRight45", RunningBackward && RightStrafeRun);
        animator.SetBool("RunFrontRight45", Running && RightStrafeRun);

        // Sử dụng trạng thái rơi tự do để kích hoạt animation
        Falling = movement.IsFalling;
        animator.SetBool("Falling", Falling); // Sử dụng trạng thái rơi tự do

        //------------------------animation dodge--------------------------------

        // Thiết lập các biến bool vào Animator để chuyển đổi giữa các animation dodge
        animator.SetBool("DodgeFront", DodgeFront);
        animator.SetBool("DodgeBack", DodgeBack);
        animator.SetBool("DodgeRight", DodgeRight);
        animator.SetBool("DodgeLeft", DodgeLeft);
        // Kiểm tra hướng dodge từ script Movement
        if (movement.Dodging)
        {
            if (movement.moveAmount.z > 0)
            {
                DodgeFront = true;
                StartCoroutine(ResetDodge(0.5f));
            }
            else if (movement.moveAmount.z < 0)
            {
                DodgeBack = true;
                StartCoroutine(ResetDodge(0.5f));
            }

            if (movement.moveAmount.x > 0)
            {
                DodgeRight = true;
                StartCoroutine(ResetDodge(0.5f));
            }
            else if (movement.moveAmount.x < 0)
            {
                DodgeLeft = true;
                StartCoroutine(ResetDodge(0.5f));
            }
        }

        //-------------------animation chiến đấu cơ bản--------------------------
        // Tăng thời gian từ lần click cuối cùng
        timeSinceLastClick += Time.deltaTime;
        // Kiểm tra nếu có click chuột và không trong quá trình tấn công
        if (Input.GetMouseButtonDown(0) && !isAttacking)
        {
            // Reset thời gian từ lần click cuối cùng
            timeSinceLastClick = 0f;
            // Tăng comboCount lên mỗi lần click
            comboCount++;
            // Kiểm tra và thiết lập animation cho từng đòn đánh kết hợp
            if (comboCount == 1)
            {
                animator.SetBool("Combo1", true);
                StartCoroutine(ResetAttackState(1.0f));
            }
            else if (comboCount == 2)
            {
                animator.SetBool("Combo2", true);
                StartCoroutine(ResetAttackState(1.0f));
            }
            else if (comboCount == 3)
            {
                animator.SetBool("Combo3", true);
                StartCoroutine(ResetAttackState(2.0f));
            }
            // Nếu đã đạt đến số lần tấn công kết hợp tối đa, reset comboCount và đặt trạng thái tấn công về false
            if (comboCount > 3)
            {
                comboCount = 0;
            }
            // Đặt trạng thái tấn công về true để tránh việc chấp nhận nhiều lần click trong một lần tấn công
            isAttacking = true;
        }
        // Nếu không có click chuột trong khoảng thời gian quy định, reset comboCount về 0
        if (timeSinceLastClick > comboResetTime)
        {
            comboCount = 0;
        }
        if (playerHealth.IsTakeDamage)
        {
            // Gọi hàm xử lý animation TakeDamage
            StartCoroutine(RandomTakeDmg());

            // Đặt lại trạng thái nhận sát thương
            playerHealth.IsTakeDamage = false;
        }

        //Xử lí BlockAttack
        if (Input.GetMouseButtonDown(1) && !isBlockAttacking)
        {
            animator.SetBool("BlockAttack", true);
            isBlockAttacking = true;

            // Gọi hàm để reset trạng thái BlockAttack
            StartCoroutine(ResetBlockAttack());
        }
        if (playerHealth.health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    // Coroutine để reset trạng thái tấn công về false sau một khoảng thời gian
    IEnumerator ResetAttackState(float resetTime)
    {
        yield return new WaitForSeconds(resetTime); // Đặt thời gian reset theo giá trị được truyền vào
        isAttacking = false;
        // Reset các trạng thái tấn công kết hợp về false
        animator.SetBool("Combo1", false);
        animator.SetBool("Combo2", false);
        animator.SetBool("Combo3", false);
    }
    IEnumerator ResetBlockAttack()
    {
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("BlockAttack", false);
        isBlockAttacking = false;
    }
    IEnumerator RandomTakeDmg()
    {
        int randomDmg = UnityEngine.Random.Range(1, 5);
        animator.SetBool("TakeDamage1", randomDmg == 1);
        animator.SetBool("TakeDamage2", randomDmg == 2);
        animator.SetBool("TakeDamage3", randomDmg == 3);
        animator.SetBool("TakeDamage4", randomDmg == 4);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("TakeDamage1", false);
        animator.SetBool("TakeDamage2", false);
        animator.SetBool("TakeDamage3", false);
        animator.SetBool("TakeDamage4", false);
    }
    IEnumerator ResetDodge(float resetTime)
    {
        yield return new WaitForSeconds(resetTime);
        DodgeFront = false;
        DodgeBack = false;
        DodgeRight = false;
        DodgeLeft = false;
    }
    IEnumerator Death()
    {
        animator.SetBool("Death", true);
        yield return new WaitForSeconds(0.5f);
        movement.enabled= false;
        camera.enabled= false;
        isAttacking= false;
    }
}