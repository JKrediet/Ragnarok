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
    [SerializeField] float totalDamage;
    [SerializeField] Item heldItem;
    //0= nothin, 1=axe

    //movement
    [SerializeField] float speed, sprintSpeed, weight, jumpForce;
    //stamina base
    [SerializeField] float baseStaminaLossPerSec, baseStaminaGainedPerSec, maxStamina;
    //stats to use
    float gravity = -1, staminaValue, staminaLossPerSec, staminaGainedPerSec;
    //nonsense(still being used tho);
    bool groundCheck = false;
    Vector3 movementSpeed, movementDirection;

    //camera
    [SerializeField] LayerMask playerAimMask;
    [SerializeField] Camera cam;
    [SerializeField] float mouseSensitivity, pitchDown, pitchUp;
    float cameraPitch;
    bool InventoryIsOpen;

    //attacks
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius = 1;

    //UI
    [SerializeField] Slider staminaSlider;

    //visual graph test
    public GameObject testGraph;

    public bool mayAttack;

    public Animator animController;

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
        }
        else
        {
            Destroy(cam.gameObject);
            enabled = false;
        }
        mayAttack = true;
    }

    private void Update()
    {
        Movement();
        Gravity();
        if (!InventoryIsOpen)
        {
            Rotation();
        }
            //andere onzin
        if (Input.GetButtonDown("Fire1"))
        {
            if(!InventoryIsOpen)
            {
                if(mayAttack)
                {
                    mayAttack = false;
                    Attack();
                }
            }
        }
        if (Input.GetButtonDown("Jump"))
        {
            if(controller.isGrounded)
            {
                Jump();
            }
        }
    }
    //apply movement to character
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    void Movement()
    {
        movementSpeed = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movementSpeed.Normalize();
        float combinedSpeed = speed;
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
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
        movementDirection = (transform.forward * movementSpeed.z + transform.right * movementSpeed.x) * combinedSpeed;
    }
    void Gravity()
    {
        if(controller.isGrounded)
        {
            if(!groundCheck)
            {
                groundCheck = true;
                gravity = -0.1f;
            }
        }
        else
        {
            groundCheck = false;
            gravity -= weight * Time.deltaTime;
            gravity = Mathf.Clamp(gravity, -10, 10);
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
    }
    void Rotation()
    {
        Vector2 mouseDelta = new Vector2 (Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        //player
        transform.Rotate(Vector3.up, mouseDelta.x * mouseSensitivity);

        //camera
        cameraPitch -= mouseDelta.y * mouseSensitivity;
        cameraPitch = Mathf.Clamp(cameraPitch, -pitchDown, pitchUp);
        cam.transform.localEulerAngles = Vector3.right * cameraPitch;
    }
    void Attack()
    {
        Anim_attack();
        Collider[] thingsHit = Physics.OverlapSphere(attackPos.position, attackRadius);

        //check hit things
        foreach(Collider hitObject in thingsHit)
        {
            if (hitObject.GetComponent<HitableObject>())
            {
                GameObject tempObject = Instantiate(testGraph, hitObject.ClosestPoint(attackPos.position), Quaternion.identity);
                Destroy(tempObject, 1);
                if(hitObject.GetComponent<Renderer>())
                {
                    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", hitObject.GetComponent<Renderer>().material.color);
                }
                else
                {
                    tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", hitObject.GetComponentInChildren<Renderer>().material.color);
                }

                //damage
                if(heldItem != null)
                {
                    hitObject.GetComponent<HitableObject>().HitByPlayer(heldItem.itemDamage, gameObject, heldItem.itemType);
                }
                else
                {
                    hitObject.GetComponent<HitableObject>().HitByPlayer(1, gameObject, 0);
                }
            }
        }
    }
    public void LockCamera()
    {
        InventoryIsOpen = !InventoryIsOpen;
    }
    public void GiveItemStats(Item _itemType)
    {
        heldItem = _itemType;
    }
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
        //mayAttack must be set to true again at end of attack animation
    }
}
