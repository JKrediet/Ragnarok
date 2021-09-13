using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;
    PhotonView pv;

    //attackStats
    [SerializeField] float totalDamage, totalAttackSpeed, totalCritChance;
    public Item heldItem;

    //movement
    [SerializeField] float speed, sprintSpeed, weight, jumpForce, combinedSpeed;
    //stamina base
    [SerializeField] float baseStaminaLossPerSec, baseStaminaGainedPerSec, maxStamina;
    //stats to use
    float gravity = -1, staminaValue, staminaLossPerSec, staminaGainedPerSec;

    bool groundCheck = false;
    float groundCheckTime;
    Vector3 movementSpeed, movementDirection;
    //ChestInventory lastChest;
    //CraftStation craftStation;

    //camera
    [SerializeField] GameObject head;
    [SerializeField] LayerMask playerAimMask;
    [SerializeField] Camera cam;
    [SerializeField] float mouseSensitivity, pitchDown, pitchUp;
    float cameraPitch;
    bool InventoryIsOpen;

    //third person
    bool isThirdPerson;
    [SerializeField] float pitchDownThird, pitchUpThird, turnSmoothTime = 0.1f, turnSmoothVelocity;
    [SerializeField] float distance, turnSpeed, minDistance, maxDistance;
    [SerializeField] Transform camOriginpos;

    //attacks
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius = 1;

    //UI
    [SerializeField] Slider staminaSlider;

    //visual graph test
    public GameObject testGraph;

    public bool mayAttack;

    public Animator animController;

    public LayerMask chestLayer;

    public int playerBalance;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        if (pv.IsMine)
        {
            controller = GetComponent<CharacterController>();
            //cursor off
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            //stamina
            staminaValue = maxStamina;
            staminaLossPerSec = baseStaminaLossPerSec;
            staminaGainedPerSec = baseStaminaGainedPerSec;

            //turn own head off so you do not see the clipping
            head.SetActive(false);
        }
        else
        {
            Destroy(cam.gameObject);
            enabled = false;
        }
        mayAttack = true;
        camOriginpos.position = cam.transform.position;
    }
    public void RecieveStats(float damage, float attackSpeed, float critChance)
    {
        totalDamage = damage;
        totalAttackSpeed = attackSpeed;
        totalCritChance = critChance;
    }
    private void Update()
    {
        Movement();
        Gravity();
        if (!InventoryIsOpen)
        {
            Rotation();
            CheckForInfo();
        }
        //andere onzin
        if (Input.GetButtonDown("Fire1"))
        {
            if(!InventoryIsOpen)
            {
                if(mayAttack)
                {
                    mayAttack = false;
                    Anim_attack();
                }
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            if(groundCheck)
            {
                Jump();
            }
        }
    }
    //apply movement to character
    private void FixedUpdate()
    {
        if (mayAttack)
        {
            ApplyMovement();
        }
    }
    void Movement()
    {
        movementSpeed = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movementSpeed.Normalize();
        if (movementSpeed.magnitude != 0)
        {
            combinedSpeed = speed;
            if (Input.GetButton("Sprint"))
            {
                if (staminaValue > 0)
                {
                    staminaValue = Mathf.Clamp(staminaValue -= staminaLossPerSec * Time.deltaTime, 0, maxStamina);
                    combinedSpeed = sprintSpeed;
                    Anim_sprint();
                }
                else
                {
                    Anim_movement();
                }
            }
            else
            {
                Anim_movement();
                staminaValue = Mathf.Clamp(staminaValue += staminaGainedPerSec * Time.deltaTime, 0, maxStamina);
            }
        }
        else
        {
            Anim_idle();
            staminaValue = Mathf.Clamp(staminaValue += staminaGainedPerSec * Time.deltaTime, 0, maxStamina);
        }
        staminaSlider.value = staminaValue;
        if (!isThirdPerson)
        {
            movementDirection = (transform.forward * movementSpeed.z + transform.right * movementSpeed.x) * combinedSpeed;
            //else is done in rotation
        }
    }
    void Gravity()
    {
        if(controller.isGrounded)
        {
            if(!groundCheck)
            {
                groundCheck = true;
                gravity = -0.3f;
            }
            groundCheckTime = Time.time;
        }
        else
        {
            if (Time.time >= groundCheckTime + 0.3f)
            {
                groundCheck = false;
            }
            gravity -= weight * Time.deltaTime;
            gravity = Mathf.Clamp(gravity, -100, 100);
        }
    }
    void ApplyMovement()
    {
        movementDirection.y = gravity;
        controller.Move(movementDirection * Time.deltaTime);
    }
    void Jump()
    {
        gravity = jumpForce;
        groundCheck = false;
    }
    void Rotation()
    {
        //distance = Mathf.Clamp(distance -= Input.mouseScrollDelta.y, minDistance, maxDistance);
        cam.transform.position = camOriginpos.position - new Vector3(0, 0, distance);
        isThirdPerson = distance < 0.5f ? false : true;
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        if (!isThirdPerson)
        {
            //player
            transform.Rotate(Vector3.up, mouseDelta.x * mouseSensitivity);

            //camera
            cameraPitch -= mouseDelta.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -pitchDown, pitchUp);
            cam.transform.localEulerAngles = Vector3.right * cameraPitch;
        }
        else
        {
            //player direction
            float targetDirection = Mathf.Atan2(movementDirection.x, movementDirection.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.rotation.y, targetDirection, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0, angle, 0);
            if(movementDirection.magnitude > 0)
            {
                movementDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward * combinedSpeed + movementSpeed;
                movementDirection.Normalize();
            }

            //camera moet 3rd worden
            cameraPitch -= mouseDelta.y * mouseSensitivity;
            cameraPitch = Mathf.Clamp(cameraPitch, -pitchDown, pitchUp);
            cam.transform.localEulerAngles = Vector3.right * cameraPitch;
        }
    }
    public void Attack()
    {
        Collider[] thingsHit = Physics.OverlapSphere(attackPos.position, attackRadius);

        //check hit things
        foreach(Collider hitObject in thingsHit)
        {
            if (hitObject.GetComponent<HitableObject>())
            {
                //particle color
                GameObject tempObject = Instantiate(testGraph, hitObject.ClosestPoint(attackPos.position), Quaternion.identity);
                Destroy(tempObject, 1);
                if (hitObject.GetComponent<Renderer>())
                {
                    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", hitObject.GetComponent<Renderer>().material.color);
                }
                else
                {
                    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", hitObject.GetComponentInChildren<Renderer>().material.color);
                }


                //crit
                float critDamage = 0;
                float roll = Random.Range(0, 100);
                if(roll < totalCritChance)
                {
                    critDamage = totalDamage;
                }
                //damage
                if(heldItem != null)
                {
                    if(heldItem as EquipableItem)
                    {
                        EquipableItem tempItem = heldItem as EquipableItem;
                        hitObject.GetComponent<HitableObject>().HitByPlayer(totalDamage + critDamage, gameObject, tempItem.equipment);
                    }
                }
                else
                {
                    hitObject.GetComponent<HitableObject>().HitByPlayer(1 + critDamage, gameObject, 0);
                }
            }
        }
    }
    public void DoneAttacking()
    {
        mayAttack = true;
        animController.SetInteger("Attack", 0);
    }
    public void LockCamera()
    {
        InventoryIsOpen = !InventoryIsOpen;
        if (!InventoryIsOpen)
        {
            //if (lastChest)
            //{
            //    lastChest = null;
            //}
        }
    }
    public void GiveItemStats(Item _itemType)
    {
        heldItem = _itemType;
    }
    void CheckForInfo()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            RaycastHit _hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, 5))
            {
                if (_hit.transform.GetComponent<ChestScript>())
                {
                    if (playerBalance >= _hit.transform.GetComponent<ChestScript>().cost)
                    {
                        playerBalance -= _hit.transform.GetComponent<ChestScript>().cost;
                        _hit.transform.GetComponent<ChestScript>().Interact();
                    }
                }
                else if (_hit.transform.GetComponent<Totem>())
				{
                    _hit.transform.GetComponent<Totem>().Interact();
				}
            }
        }
    }
    #region anim
    void Anim_idle()
    {
        animController.SetInteger("State", 0);
    }
    void Anim_movement()
    {
        animController.SetInteger("State", 1);
    }
    void Anim_sprint()
    {
        animController.SetInteger("State", 2);
    }
    void Anim_attack()
    {
        animController.SetInteger("Attack", 1);
    }
    #endregion
}
