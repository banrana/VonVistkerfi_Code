using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class Movement : MonoBehaviour
{
    [Header("Move Speed")]
    public float walkSpeed = 6;
    public float runSpeed = 12f;
    private bool canMove = true; // Biến kiểm soát xem nhân vật có thể di chuyển không
    [Header("Jump")]
    public float jumpForce = 440f;
    public LayerMask groundedMask;
    private bool canDoubleJump = true;
    public int maxJumps = 2; // Số lượng nhảy tối đa, đặt là 2 để có thể double jump
    private int jumpCount = 0;
    public bool IsJumping { get; private set; }
    public bool IsFalling { get; private set; }
    [Header("Dodge")]
    public float dodgeSpeed = 20f; // Tốc độ trong lúc dodge
    public float dodgeDistance = 5f; // Khoảng cách di chuyển trong lúc dodge
    private bool isDodging = false;
    private float originalWalkSpeed;
    private float originalRunSpeed;
    private bool canDodge = true; // Biến kiểm soát xem có thể thực hiện dodge hay không
    public float dodgeCooldown = 1f; // Thời gian chờ giữa các lần dodge
    public TrailEffect trailEffect; // Tham chiếu đến script TrailEffect
    public bool Dodging { get { return isDodging; } }
    private float doubleTapTimeThreshold = 0.5f; // Ngưỡng thời gian giữa hai lần nhấn
    private float lastMoveKeyPressTime; // Thời điểm cuối cùng phím di chuyển được nhấn
    private KeyCode lastKeyCode;

    private bool isGrounded;
    public bool IsGrounded { get { return isGrounded; } }
    private GravityBody gravityBody;
    private Rigidbody rigidbody;
    public Vector3 moveAmount;
    private Vector3 smoothMoveVelocity;
    
    //Stop movement
    private bool isStopping = false;
    private float stopTime = 0.5f;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        trailEffect.enabled= false;
    }

    void Update()
    {
        // Tính toán di chuyển:
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");

        Vector3 moveDir = new Vector3(inputX, 0, inputY).normalized;
        Vector3 targetMoveAmount = moveDir * (Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed);
        moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, .15f);

        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            else if (canDoubleJump && jumpCount < maxJumps)
            {
                Jump();
            }
        }

        // Grounded check
        Ray ray = new Ray(transform.position, -transform.up);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1 + .1f, groundedMask))
        {
            isGrounded = true;
            IsFalling = false; // Không rơi tự do khi đứng trên mặt đất
            canDoubleJump = true;
            jumpCount = 0;
        }
        else
        {
            isGrounded = false;
            IsFalling = true; // Đang rơi tự do khi không đứng trên mặt đất
        }

        //Dodge action
        CheckDoubleTap(KeyCode.W); 
        CheckDoubleTap(KeyCode.A); 
        CheckDoubleTap(KeyCode.S); 
        CheckDoubleTap(KeyCode.D);
    }

    void FixedUpdate()
    {
        if (!isStopping && canMove)
        {
            // Áp dụng di chuyển vào Rigidbody:
            Vector3 localMove = transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;

            // Tính toán vị trí dự định sau khi di chuyển
            Vector3 nextPosition = rigidbody.position + localMove;

            // Thực hiện raycast để kiểm tra xem có va chạm với vật thể hay không
            Ray ray = new Ray(rigidbody.position, localMove.normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, localMove.magnitude + 0.1f))
            {
                // Nếu có va chạm và collider không phải là trigger
                if (!hit.collider.isTrigger)
                {
                    // Điều chỉnh vị trí dự định để dừng lại tại vị trí va chạm
                    nextPosition = hit.point - localMove.normalized * 1.0f;

                    // Bắt đầu đếm ngược
                    StartCoroutine(StopMovement());
                }
            }

            // Di chuyển đối tượng đến vị trí mới
            rigidbody.MovePosition(nextPosition);
        }
    }

    IEnumerator StopMovement()
    {
        isStopping = true;

        // Đợi trong 0.5 giây
        yield return new WaitForSeconds(stopTime);

        // Kết thúc đếm ngược, cho phép di chuyển lại
        isStopping = false;
    }

    //Double Jump
    void Jump()
    {
        rigidbody.AddForce(transform.up * jumpForce);
        IsJumping = true;
        jumpCount++;
        if (!isGrounded)
        {
            canDoubleJump = false;
        }
    }
    //Dodge
    void CheckDoubleTap(KeyCode key) //Kiểm tra double tap
    {
        if (Input.GetKeyDown(key))
        {
            if (IsDoubleTap(key))
            {
                // Nếu đúng là double tap, thực hiện dodge
                if (!isDodging)
                {
                    originalWalkSpeed = walkSpeed;
                    originalRunSpeed = runSpeed;
                    StartCoroutine(Dodge());
                }
            }
        }
    }

    bool IsDoubleTap(KeyCode key)
    {
        if (Time.time - lastMoveKeyPressTime < doubleTapTimeThreshold && lastKeyCode == key)
        {
            // Nếu thời gian giữa hai lần nhấn nhỏ hơn ngưỡng và cùng một phím được nhấn lần thứ hai, là double tap
            lastMoveKeyPressTime = 0f; // Đặt lại thời điểm nhấn phím để tránh việc xử lý nhiều lần
            return true;
        }
        else
        {
            // Nếu không phải là double tap, ghi lại thời điểm và phím được nhấn
            lastMoveKeyPressTime = Time.time;
            lastKeyCode = key;
            return false;
        }
    }
    IEnumerator Dodge()
    {
        if (canDodge)
        {
            isDodging = true;
            canMove = false; // Ngăn chặn di chuyển trong khi dodge
            trailEffect.enabled = true;
            // Tạm thời set tốc độ dodge
            walkSpeed = dodgeSpeed;
            runSpeed = dodgeSpeed;

            // Đợi cho đến khi kết thúc thời gian dodge
            yield return new WaitForSeconds(dodgeDistance / dodgeSpeed);

            // Khôi phục tốc độ về giá trị ban đầu
            walkSpeed = originalWalkSpeed;
            runSpeed = originalRunSpeed;

            isDodging = false;
            canMove = true; // Cho phép di chuyển trở lại sau khi kết thúc dodge

            // Tắt khả năng dodge trong khoảng cooldown
            canDodge = false;

            // Thiết lập thời gian chờ cho dodge tiếp theo
            yield return new WaitForSeconds(dodgeCooldown);

            // Bật lại khả năng dodge
            canDodge = true;
            trailEffect.enabled = false;
        }
    }
}