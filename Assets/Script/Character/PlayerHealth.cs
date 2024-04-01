using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public Slider healthBar;
    public Collider playerCollider;
    private EnemyWeapon enemyWeapon;
    //public Animation animation;
    public float maxHealth = 100;
    public float health;
    public bool canTakeDamage = true;
    private bool isTakeDamage = false;
    public bool IsTakeDamage { get { return isTakeDamage; } set { isTakeDamage = value; } }
    private float delayBeforeNextDamage = 1.5f;
    private float damageMinusSpeed = 10f; // Tốc độ giảm dần

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {

        if (healthBar.value != health)
        {
            healthBar.value = Mathf.Lerp(healthBar.value, health, Time.deltaTime * damageMinusSpeed);
        }
        HandleAttack();

    }
    void HandleAttack()
    {
        if (IsAttacked() && canTakeDamage /*&& !animation.IsBlockAttacking*/)
        {
            int damage = Random.Range(enemyWeapon.minDamage, enemyWeapon.maxDamage +1);
            TakeDamage(damage);
            StartCoroutine(WaitForNextDamage());
        }
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        isTakeDamage = true;
        if (health <= 0)
        {
            Debug.Log("Die");
        }
    }
    bool IsAttacked()
    {
        // Use the attackCollider for attack detection instead of the transform properties
        Collider[] hitColliders = Physics.OverlapBox(playerCollider.bounds.center, playerCollider.bounds.extents, Quaternion.identity);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy Weapon"))
            {
                enemyWeapon = col.GetComponent<EnemyWeapon>();
                return true;
            }
        }

        return false;
    }
    IEnumerator WaitForNextDamage()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(delayBeforeNextDamage);
        canTakeDamage = true;
    }

}
