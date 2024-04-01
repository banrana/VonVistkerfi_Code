using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    public Transform cam;
    public Slider healthBar;
    public GameObject enemy;
    public Collider enemyCollider;
    private WeaponController weaponController;
    public float maxHealth = 100;
    public float health;
    private bool canTakeDamage = true;
    private bool isTakeDamage = false;
    public bool IsTakeDamage { get { return isTakeDamage; } set { isTakeDamage = value; } }
    private float delayBeforeNextDamage = 1.0f;
    private float damageMinusSpeed = 10f; // Tốc độ giảm dần

    void Start()
    {
        health = maxHealth;
    }

    void Update()
    {
        transform.LookAt(cam.position + cam.forward);

        if (healthBar.value != health)
        {
            healthBar.value = Mathf.Lerp(healthBar.value, health, Time.deltaTime * damageMinusSpeed);
        }

        HandleAttack();

    }

    void HandleAttack()
    {
        if (IsAttacked() && canTakeDamage)
        {
            int damage = Random.Range(weaponController.minDamage, weaponController.maxDamage + 1);
            TakeDamage(damage);
            StartCoroutine(WaitForNextDamage());
        }
    }

    bool IsAttacked()
    {
        Collider[] hitColliders = Physics.OverlapBox(enemyCollider.bounds.center, enemyCollider.bounds.extents, Quaternion.identity);

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Player Weapon"))
            {
                weaponController = col.GetComponent<WeaponController>();
                return true;
            }
        }

        return false;
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        isTakeDamage = true;
        if (health <= 0)
        {
            if (enemy != null)
            {
                //Destroy(enemy);
            }
        }
    }

    IEnumerator WaitForNextDamage()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(delayBeforeNextDamage);
        canTakeDamage = true;
    }
}
