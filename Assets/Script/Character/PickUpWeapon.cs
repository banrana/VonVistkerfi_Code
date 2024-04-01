using SickscoreGames.HUDNavigationSystem;
using System.Collections;
using System.Collections.Generic;
using TrailsFX;
using UnityEngine;

public class PickUpWeapon : MonoBehaviour
{
    private WeaponController weaponController;
    private Rigidbody weaponRigidbody;
    private GravityBody weaponGravity;
    private BoxCollider weaponCollider;
    private HUDNavigationElement HUD;
    public Transform player;  
    public Transform weaponHolder;
    public PlayerHealth playerHealth;
    public PauseMenu pauseMenu;
    public float pickUpRange = 2.0f;
    
    public bool equipped;
    public static bool slotFull;

    private void Start()
    {
        weaponController = GetComponent<WeaponController>();
        weaponRigidbody = GetComponent<Rigidbody>();
        weaponGravity = GetComponent<GravityBody>();
        weaponCollider = GetComponent<BoxCollider>();
        HUD = GetComponentInChildren<HUDNavigationElement>();
        //Setup
        if (!equipped)
        {
            weaponController.enabled = false;
            weaponRigidbody.isKinematic = false;
            weaponCollider.isTrigger = false;
            weaponCollider.enabled= true;
            weaponGravity.enabled= true;
            HUD.showIndicator = true;
            HUD.hideInRadar= true;
        }
        if (equipped)
        {
            weaponController.enabled = true;
            weaponRigidbody.isKinematic = true;
            weaponCollider.isTrigger = true;
            weaponCollider.enabled = false;
            weaponGravity.enabled= false;
            HUD.showIndicator = false;
            HUD.hideInRadar = false;
            slotFull = true;
        }
    }

    private void Update()
    {
        //Kiểm tra nếu người chơi ở trong tầm và nhấn "E"
        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull) PickUp();

        //Bỏ vũ khí nếu equipped và nhấn "Q"
        if (equipped && Input.GetKeyDown(KeyCode.Q)) Drop();

        DropWeaponReason();
    }

    private void PickUp()
    {
        equipped = true;
        slotFull = true;
        //Set vũ khí tới vị trí weaponHolder
        transform.SetParent(weaponHolder);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        //Rigidbody kinematic và BoxCollider trigger
        weaponRigidbody.isKinematic = true;
        weaponCollider.isTrigger = true;
        weaponCollider.enabled = false;
        weaponGravity.enabled= false;
        HUD.showIndicator = false;
        HUD.hideInRadar = false;

        weaponController.enabled = true;
    }

    public void Drop()
    {
        equipped = false;
        slotFull = false;

        //Set parent thành null
        transform.SetParent(null);

        //Rigidbody tắt kinematic và BoxCollider về trạng thái bình thường
        weaponRigidbody.isKinematic = false;
        weaponCollider.isTrigger = false;
        weaponCollider.enabled= true;
        weaponGravity.enabled = true;
        HUD.showIndicator = true;
        HUD.hideInRadar = true;

        weaponController.enabled = false;
    }
    private void DropWeaponReason()
    {
        if (equipped && playerHealth.health <= 0)
        {
            Drop();
        }
        if(equipped && pauseMenu.IsRestarting)
        {
            Drop();
        }
    }
}
