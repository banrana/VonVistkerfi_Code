using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

[RequireComponent(typeof(GravityBody))]
public class Enemy : MonoBehaviour
{
    private Transform player;
    private Rigidbody rb;
    public Movement playerMovement;
    private GravityBody gravityBody;
    Animator animator;

    [Header("Vision")]
    public float visionAngle = 180f;
    public float visionDistance = 30f;
    
    public float rotationSpeed = 10f;
    private float currentDistance; // Khoảng cách hiện tại giữa Enemy và Player
    private bool isObstacleInFront = false; // Kiểm tra có vật cản phía trước không

    [Header("Movement")]
    private Vector3 moveAmount; // Di chuyển mịn
    private Vector3 smoothMoveVelocity; // Vận tốc mịn
    public float moveSpeed = 6f;
    public float stopDistance = 2f; // Khoảng cách mà Enemy sẽ dừng lại
    public float KnockBackSpeed = 6f;
    private bool isKnockBack = false;
    private bool shouldKnockBack = false;
    [Header("Combat")]
    public float initialAttackDelay = 0.5f;// Thời gian thực hiện đòn đánh đầu tiên
    public float attackInterval = 1.5f;// Thời gian giữa các đòn đánh
    private bool isAttacking = false;// Biến để kiểm soát việc thực hiện đòn đánh
    public Collider enemyWeapon;
    public EnemyHealth enemyHealth;
    private bool isReviving = false;
    public float reviveTime = 100f;
    public Animation playerAnimation;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        gravityBody = GetComponent<GravityBody>();
        animator = GetComponentInChildren<Animator>();
        enemyHealth= GetComponentInChildren<EnemyHealth>();
        enemyWeapon.enabled= false;
    }

    void Update()
    {
        Vector3 toPlayer = player.position - transform.position;
        float distanceToPlayer = toPlayer.magnitude;
        // Kiểm tra nếu nhân vật đang đứng trên mặt đất
        if (playerMovement.IsGrounded && !isAttacking && !isReviving)
        {
            // Kiểm tra có vật che khuất giữa Enemy và Player không
            RaycastHit hit;
            if (Physics.Raycast(transform.position, toPlayer.normalized, out hit, visionDistance))
            {
                isObstacleInFront = hit.collider.tag != "Player";
            }
            else
            {
                isObstacleInFront = false;
            }

            if (distanceToPlayer <= visionDistance)
            {
                currentDistance = Vector3.Distance(transform.position, player.position);

                // Kiểm tra xem người chơi có trong góc nhìn của kẻ thù không
                Vector3 toPlayerDirection = (player.position - transform.position).normalized;
                float angleToPlayer = Vector3.Angle(toPlayerDirection, transform.forward);

                if (angleToPlayer <= visionAngle * 0.5f && currentDistance > stopDistance && !isObstacleInFront && enemyHealth.health > 0)
                {

                    // Xác định hướng mới để nhìn vào người chơi
                    Vector3 directionToPlayer = player.position - transform.position;
                    Vector3 projectedDirection = Vector3.ProjectOnPlane(directionToPlayer, gravityBody.GetUpDirection());
                    Quaternion lookRotation = Quaternion.LookRotation(projectedDirection, gravityBody.GetUpDirection());

                    // Hạn chế tốc độ xoay
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

                    // Di chuyển về phía người chơi
                    Vector3 targetMoveAmount = projectedDirection.normalized * moveSpeed;
                    moveAmount = Vector3.SmoothDamp(moveAmount, targetMoveAmount, ref smoothMoveVelocity, 0.15f);
                    rb.MovePosition(transform.position + moveAmount * Time.deltaTime);

                    // Set trạng thái di chuyển trong Animator
                    animator.SetBool("Movement", true);
                }
                else
                {
                    // Set trạng thái Idle trong Animator
                    animator.SetBool("Movement", false);
                }

                if (distanceToPlayer <= stopDistance)
                {
                    // Xác định thời điểm thực hiện đòn đánh
                    if (!isAttacking)
                    {
                        enemyWeapon.enabled = true;
                        StartCoroutine(PerformAttack());
                    }
                }
                
                Debug.DrawLine(transform.position, player.position, currentDistance <= stopDistance || isObstacleInFront ? Color.magenta : Color.red);

                if (isObstacleInFront)
                {
                    // Nếu có vật che khuất phía trước, dừng lại và không di chuyển
                    rb.velocity = Vector3.zero;
                    return;
                }

                //Xử lý khi kẻ thù hết máu
                if (enemyHealth.health <= 0)
                {
                    animator.SetBool("Movement", false);
                    animator.SetBool("Attack1", false);
                    animator.SetBool("Attack2", false);
                    animator.SetBool("Attack3", false);
                    enemyWeapon.enabled = false;
                    StartCoroutine(EnemyRevival());
                    return;
                }
            }
            
            else
            {
                Debug.DrawLine(transform.position, player.position, Color.green);
            }
        }
        else
        {
            animator.SetBool("Movement", false);
        }

        if (enemyHealth.IsTakeDamage)
        {
            // Gọi hàm xử lý animation TakeDamage
            StartCoroutine(TakeDamageAnimation());

            // Đặt lại trạng thái nhận sát thương
            enemyHealth.IsTakeDamage = false;

            // Tính xác suất thực hiện knockBack (ví dụ: 50%)
            float KnockBackRate= 0.5f;

            // Tạo số ngẫu nhiên trong khoảng từ 0 đến 1
            float randomValue = Random.Range(0f, 1f);

            // Kiểm tra xem nên thực hiện dodge hay không
            shouldKnockBack = randomValue <= KnockBackRate;

            // Nếu nên thực hiện dodge, kích hoạt hàm xử lý lùi về và dừng lại
            if (shouldKnockBack)
            {
                StartCoroutine(KnockBack());
            }
        }
        if (playerAnimation.IsBlockAttacking && distanceToPlayer <= stopDistance)
        {
            StartCoroutine(KnockBackAttack());
        }
    }

    //Thực hiện cơ chế chiến đấu
    IEnumerator PerformAttack()
    {
        // Đánh dấu đang thực hiện đòn đánh
        isAttacking = true;

        // Dừng di chuyển
        moveAmount = Vector3.zero;
        smoothMoveVelocity = Vector3.zero;

        // Chờ một khoảng thời gian trước khi thực hiện đòn đánh đầu tiên
        yield return new WaitForSeconds(initialAttackDelay);

        // Random lựa chọn animation tấn công (Attack1, Attack2, Attack3)
        int randomAttack = Random.Range(1, 4);
        animator.SetBool("Attack1", randomAttack == 1);
        animator.SetBool("Attack2", randomAttack == 2);
        animator.SetBool("Attack3", randomAttack == 3);

        // Chờ cho đến khi hoàn thành animation tấn công
        yield return new WaitForSeconds(attackInterval);

        // Đặt tất cả các biến tấn công về false để chuẩn bị cho lần tấn công tiếp theo
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Attack3", false);

        // Đánh dấu đã kết thúc đòn đánh
        isAttacking = false;
        // Tắt collider sau khi hoàn thành đòn đánh
        enemyWeapon.enabled = false;
    }
    IEnumerator TakeDamageAnimation()
    {
        animator.SetBool("TakeDamage", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("TakeDamage", false);
    }
    // Hàm xử lý lùi về và dừng lại
    IEnumerator KnockBack()
    {
        if (!isKnockBack)
        {
            // Đánh dấu đang trong trạng thái lùi về
            isKnockBack = true;

            // Tính hướng lùi ra xa người chơi
            Vector3 toPlayerDirection = (player.position - transform.position).normalized;
            Vector3 dodgeDirection = -toPlayerDirection;
            // Xác định hướng mới để nhìn vào hướng lùi về
            Vector3 projectedDirection = Vector3.ProjectOnPlane(dodgeDirection, gravityBody.GetUpDirection());
            Quaternion lookRotation = Quaternion.LookRotation(projectedDirection, gravityBody.GetUpDirection());

            // Hạn chế tốc độ xoay
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

            // Di chuyển lùi về với tốc độ dodgeSpeed
            rb.velocity = dodgeDirection * KnockBackSpeed;
            yield return new WaitForSeconds(1f);
            // Đặt lại trạng thái lùi về
            isKnockBack = false;
        }
    }
    IEnumerator KnockBackAttack()
    {
        enemyWeapon.enabled = false;
        animator.SetBool("KnockBackAttack", true) ;
        animator.SetBool("Attack1", true);
        animator.SetBool("Attack2", true);
        animator.SetBool("Attack3", true);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("Attack1", false);
        animator.SetBool("Attack2", false);
        animator.SetBool("Attack3", false);
        yield return new WaitForSeconds(0.2f);
        animator.SetBool("KnockBackAttack", false) ;
        enemyWeapon.enabled = true;
    }

    //Hàm xử lý việc hồi sinh kẻ thù
    IEnumerator EnemyRevival()
    {
        isReviving = true;
        StopCoroutine(PerformAttack());
        StopCoroutine(TakeDamageAnimation());
        StopCoroutine(KnockBack());
        enemyWeapon.enabled = false;
        animator.SetBool("Death", true);

        // Thực hiện các bước hồi sinh
        yield return new WaitForSeconds(reviveTime);
        animator.SetBool("Death", false);
        enemyHealth.health = 100;
        isReviving = false;
        // Bắt đầu theo dõi nhân vật lại
        isAttacking = true;
        StartCoroutine(PerformAttack());
        StartCoroutine(TakeDamageAnimation());
        StartCoroutine(KnockBack());
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 direction = transform.forward;
        Vector3 rightDirection = Quaternion.Euler(0, visionAngle / 2f, 0) * direction;
        Vector3 leftDirection = Quaternion.Euler(0, -visionAngle / 2f, 0) * direction;
        Vector3 upDirection = Quaternion.Euler(visionAngle / 2f, 0, 0) * direction;
        Vector3 downDirection = Quaternion.Euler(-visionAngle / 2f, 0, 0) * direction;

        Gizmos.DrawRay(transform.position, direction * 5f);
        Gizmos.DrawRay(transform.position, rightDirection * 5f);
        Gizmos.DrawRay(transform.position, leftDirection * 5f);
        Gizmos.DrawRay(transform.position, upDirection * 5f);
        Gizmos.DrawRay(transform.position, downDirection * 5f);
    }
}