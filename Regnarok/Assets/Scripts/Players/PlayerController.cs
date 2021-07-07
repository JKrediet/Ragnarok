using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    CharacterController controller;

    //attackStats
    [SerializeField] float axePower;

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
    [SerializeField] float mouseSensitivity;
    float cameraPitch;
    bool InventoryIsOpen;

    //attacks
    [SerializeField] Transform attackPos;
    [SerializeField] float attackRadius = 1;

    //UI
    [SerializeField] Slider staminaSlider;

    private void Awake()
    {
        if(GetComponent<Health>().PV.IsMine)
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
            enabled = false;
        }
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
                Attack();
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
        if(Input.GetButton("Sprint"))
        {
            if (staminaValue > 0)
            {
                staminaValue = Mathf.Clamp(staminaValue -= staminaLossPerSec * Time.deltaTime, 0, maxStamina);
                combinedSpeed = sprintSpeed;
            }
        }
        else
        {
            if (staminaValue < maxStamina)
            {
                staminaValue = Mathf.Clamp(staminaValue += staminaGainedPerSec * Time.deltaTime, 0, maxStamina);
            }
        }
        staminaSlider.value = staminaValue;
        movementDirection = (transform.forward * movementSpeed.z + transform.right * movementSpeed.x) * combinedSpeed * Time.deltaTime;
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
        controller.Move(movementDirection);
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
        cameraPitch = Mathf.Clamp(cameraPitch, -70, 70);
        cam.transform.localEulerAngles = Vector3.right * cameraPitch;
    }
    void Attack()
    {
        Collider[] thingsHit = Physics.OverlapSphere(attackPos.position, attackRadius);

        //check hit things
        foreach(Collider hitObject in thingsHit)
        {
            if(hitObject.CompareTag("Tree"))
            {
                hitObject.GetComponent<Tree>().HitByPlayer(axePower, gameObject);
            }
        }
    }
    public void LockCamera()
    {
        InventoryIsOpen = !InventoryIsOpen;
    }
}
