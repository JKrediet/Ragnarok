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
    [SerializeField] float totalDamage, totalAttackSpeed, totalCritChance, totalLifeSteal, totalChanceToInflictBleed, totalHealthOnKill, totalExtraSpeed;
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
    [HideInInspector] public ChestInventory lastChest;
    [HideInInspector] public CraftingStation lastCratingStation;
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

    public GameObject nameOfPlayer;
    GameObject localplayerObject;

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
            nameOfPlayer.SetActive(false);

            mayAttack = true;
            camOriginpos.position = cam.transform.position;
        }
        else
        {
            Destroy(cam.gameObject);
            nameOfPlayer.SetActive(true);
            nameOfPlayer.GetComponentInChildren<TextMeshProUGUI>().text = PhotonNetwork.LocalPlayer.NickName;
        }
        FindObjectOfType<GameManager>().playerObjectList.Add(gameObject);
    }
    public void RecieveStats(float _damage, float _attackSpeed, float _critChance, float _lifesteal, float _bleedChance, float _healthOnKill, float _movementSpeed)
    {
        totalDamage = _damage;
        totalAttackSpeed = _attackSpeed;
        totalCritChance = _critChance;
        totalLifeSteal = _lifesteal;
        totalChanceToInflictBleed = _bleedChance;
        totalHealthOnKill = _healthOnKill;
        totalExtraSpeed = _movementSpeed;
    }
    private void Update()
    {
        if (pv.IsMine)
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
                if (!InventoryIsOpen)
                {
                    if (mayAttack)
                    {
                        mayAttack = false;
                        Anim_attack();
                    }
                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (groundCheck)
                {
                    Jump();
                }
            }
            //check for chestDistance
            if (lastChest != null)
            {
                float distance = Vector3.Distance(transform.position, lastChest.transform.position);
                if (distance > 5)
                {
                    lastChest.CloseChestInventory();
                    lastChest = null;
                }
            }
            if (lastCratingStation != null)
            {
                float distance = Vector3.Distance(transform.position, lastCratingStation.transform.position);
                if (distance > 5)
                {
                    lastCratingStation.CloseChestInventory();
                    lastCratingStation = null;
                }
            }
        }
        else
        {
            if(localplayerObject != null)
            {
                nameOfPlayer.transform.LookAt(localplayerObject.transform, transform.up);
            }
            else
            {
                for (int i = 0; i < FindObjectOfType<GameManager>().playerObjectList.Count; i++)
                {
                    if (FindObjectOfType<GameManager>().playerObjectList[i].GetComponent<PhotonView>().IsMine)
                    {
                        localplayerObject = FindObjectOfType<GameManager>().playerObjectList[i];
                    }
                }
            }
        }
    }
    //apply movement to character
    private void FixedUpdate()
    {
        if (pv.IsMine)
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
            combinedSpeed = speed + totalExtraSpeed;
            if (Input.GetButton("Sprint"))
            {
                if (staminaValue > 0)
                {
                    staminaValue = Mathf.Clamp(staminaValue -= staminaLossPerSec * Time.deltaTime, 0, maxStamina);
                    combinedSpeed = sprintSpeed + totalExtraSpeed;
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
            if (!mayAttack)
            {
                combinedSpeed *= 0.25f;
            }
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
        if (pv.IsMine)
        {
            Collider[] thingsHit = Physics.OverlapSphere(attackPos.position, attackRadius);

            //check hit things
            foreach (Collider hitObject in thingsHit)
            {
                if (hitObject.gameObject != gameObject)
                {
                    RaycastHit _hit;
                    if (Physics.Linecast(attackPos.position, hitObject.ClosestPoint(attackPos.position), out _hit))
                    {
                        Renderer rend = _hit.transform.GetComponent<Renderer>();
                        MeshCollider meshCollider = _hit.collider as MeshCollider;
                        GameObject tempObject = Instantiate(testGraph, hitObject.ClosestPoint(attackPos.position), Quaternion.identity);
                        Destroy(tempObject, 1);
                        if (rend == null || rend.sharedMaterial == null || rend.sharedMaterial.mainTexture == null || meshCollider == null)
                        {
                            tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", Color.black);
                        }
                        else
                        {
                            Texture2D tex = rend.material.mainTexture as Texture2D;
                            Vector2 pixelUV = _hit.textureCoord;
                            pixelUV.x *= tex.width;
                            pixelUV.y *= tex.height;

                            Color kleurtje = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

                            //particle color
                            tempObject.GetComponent<VisualEffect>().SetVector4("GivenColor", kleurtje);
                        }
                    }
                    //crit
                    float critDamage = 0;
                    bool inflictBleed = false;
                    float roll = Random.Range(0, 100);
                    if (roll < totalCritChance)
                    {
                        critDamage = totalDamage;
                    }
                    roll = Random.Range(0, 100);
                    if (roll < totalChanceToInflictBleed)
                    {
                        inflictBleed = true;
                    }
                    //actual hit
                    if (hitObject.GetComponent<HitableObject>())
                    {
                        //damage
                        if (heldItem != null)
                        {
                            hitObject.GetComponent<HitableObject>().HitByPlayer(totalDamage + critDamage, heldItem.equipment);
                        }
                        else
                        {
                            hitObject.GetComponent<HitableObject>().HitByPlayer(1, 0);
                        }
                    }
                    if (hitObject.GetComponent<EnemieHealth>())
                    {
                        //damage
                        if (heldItem != null)
                        {
                            hitObject.GetComponent<EnemieHealth>().TakeDamage(totalDamage + critDamage, inflictBleed);
                            if (totalLifeSteal > 0)
                            {
                                float healAmount = (totalDamage + critDamage - hitObject.GetComponent<EnemieHealth>().armor) * (totalLifeSteal / 100);
                                GetComponent<Health>().Health_Heal(healAmount);
                            }
                            if (totalHealthOnKill > 0)
                            {
                                if (hitObject.GetComponent<EnemieHealth>().health - (totalDamage + critDamage - hitObject.GetComponent<EnemieHealth>().armor) <= 0)
                                {
                                    GetComponent<Health>().Health_Heal(totalHealthOnKill);
                                }
                            }
                        }
                        else
                        {
                            hitObject.GetComponent<EnemieHealth>().TakeDamage(1, false);
                        }
                    }
                }
            }
        }
    }
    public void DoneAttacking()
    {
        mayAttack = true;
        animController.SetInteger("Attack", 0);
        animController.speed = 1;
    }
    public void LockCamera()
    {
        InventoryIsOpen = !InventoryIsOpen;
    }
    public void GiveItemStats(Item _itemType)
    {
        heldItem = _itemType;
    }
    void CheckForInfo()
    {
        if (pv.IsMine)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                RaycastHit _hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out _hit, 5))
                {
                    if (_hit.transform.GetComponent<ChestScript>())
                    {
                        if (_hit.transform.GetComponent<ChestScript>().canInteract)
                        {
                            if (playerBalance >= _hit.transform.GetComponent<ChestScript>().cost)
                            {
                                playerBalance -= _hit.transform.GetComponent<ChestScript>().cost;
                                _hit.transform.GetComponent<ChestScript>().Interact();
                            }
                        }
                    }
                    else if (_hit.transform.GetComponent<Totem>())
                    {
                        _hit.transform.GetComponent<Totem>().Interact();
                    }
                    else if (_hit.transform.GetComponent<ChestInventory>())
                    {
                        GetComponent<Inventory>().OpenActualInventory();
                        lastChest = _hit.transform.GetComponent<ChestInventory>();
                        lastChest.OpenChestInventory(GetComponent<CharacterStats>());
                    }
                    else if (_hit.transform.GetComponent<CraftingStation>())
                    {
                        GetComponent<Inventory>().OpenActualInventory();
                        lastCratingStation = _hit.transform.GetComponent<CraftingStation>();
                        lastCratingStation.OpenCratingInventory(GetComponent<CharacterStats>(), GetComponent<Inventory>());
                    }
                    else if (_hit.transform.GetComponent<ItemPickUp>())
                    {
                        _hit.transform.GetComponent<ItemPickUp>().DropItems();
                    }
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
        animController.speed = totalAttackSpeed;
    }
    #endregion
}
