using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public Animation animation;
    private Collider swordCollider;
    private bool colliderEnabled = false;
    public TrailEffect weaponEffect;
    // Điều chỉnh sát thương: giá trị tối thiểu và tối đa
    public int minDamage = 6;
    public int maxDamage = 10;

    void Start()
    {
        swordCollider = GetComponent<Collider>();
        swordCollider.enabled = false;
        weaponEffect.enabled= false;
    }

    void Update()
    {
        if (animation.IsAttacking)
        {
            // Đảm bảo collider được bật chỉ khi nó chưa được bật trong khoảng thời gian đã đặt
            if (!colliderEnabled)
            {
                StartCoroutine(ToggleCollider());
            }
        }
    }

    IEnumerator ToggleCollider()
    {
        // Bật collider và istrigger
        swordCollider.enabled = true;
        weaponEffect.enabled = true;
        colliderEnabled = true;

        // Đợi một khoảng thời gian
        yield return new WaitForSeconds(0.1f);

        // Tắt collider và istrigger
        swordCollider.enabled = false;
        weaponEffect.enabled = false;
        colliderEnabled = false;
    }
}
