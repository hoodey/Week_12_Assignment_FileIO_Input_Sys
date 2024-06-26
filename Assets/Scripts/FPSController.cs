using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FPSController : MonoBehaviour
{
    // references
    CharacterController controller;
    [SerializeField] GameObject cam;
    [SerializeField] Transform gunHold;
    [SerializeField] Gun initialGun;

    // stats
    [SerializeField] float movementSpeed = 2.0f;
    [SerializeField] float lookSensitivityX = 1.0f;
    [SerializeField] float lookSensitivityY = 1.0f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;

    // private variables
    Vector3 origin;
    Vector3 velocity;
    bool grounded;
    float xRotation;
    List<Gun> equippedGuns = new List<Gun>();
    int gunIndex = 0;
    public Gun currentGun = null;
    bool autoFiring;
    bool sprinting;

    // properties
    public GameObject Cam { get { return cam; } }
    

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        InputManager.controls.Enable();
        InputManager.controls.Player.Jump.performed += Jump;
        InputManager.controls.Player.SwitchGun.performed += SwitchGun;
        InputManager.controls.Player.FireGun.performed += FireGun;
        InputManager.controls.Player.FireGun.canceled += FireGunCanceled;
        InputManager.controls.Player.Sprint.performed += Sprint;
        InputManager.controls.Player.Sprint.canceled += SprintCanceled;

    }



    private void OnDisable()
    {
        InputManager.controls.Disable();
        InputManager.controls.Player.Jump.performed -= Jump;
        InputManager.controls.Player.SwitchGun.performed -= SwitchGun;
        InputManager.controls.Player.FireGun.performed -= FireGun;
        InputManager.controls.Player.FireGun.canceled -= FireGunCanceled;
        InputManager.controls.Player.Sprint.performed -= Sprint;
        InputManager.controls.Player.Sprint.canceled -= SprintCanceled;
    }

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;

        // start with a gun
        if(initialGun != null)
            AddGun(initialGun);

        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        grounded = controller.isGrounded;
        Movement();
        Look();
        FireAutomaticGun();

        // always go back to "no velocity"
        // "velocity" is for movement speed that we gain in addition to our movement (falling, knockback, etc.)
        Vector3 noVelocity = new Vector3(0, velocity.y, 0);
        velocity = Vector3.Lerp(velocity, noVelocity, 5 * Time.deltaTime);
    }

    /*void OldMovement()
    {
        grounded = controller.isGrounded;

        if (grounded && velocity.y < 0)
        {
            velocity.y = -1;// -0.5f;
        }

        Vector2 movement = GetPlayerMovementVector();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        controller.Move(move * movementSpeed * (GetSprint() ? 2 : 1) * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            velocity.y += Mathf.Sqrt (jumpForce * -1 * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }*/

    /*void OldLook()
    {
        Vector2 looking = GetPlayerLook();
        float lookX = looking.x * lookSensitivityX * Time.deltaTime;
        float lookY = looking.y * lookSensitivityY * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }*/

    /*void HandleSwitchGun()
    {
        if (equippedGuns.Count == 0)
            return;

        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            gunIndex++;
            if (gunIndex > equippedGuns.Count - 1)
                gunIndex = 0;

            EquipGun(equippedGuns[gunIndex]);
        }

        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            gunIndex--;
            if (gunIndex < 0)
                gunIndex = equippedGuns.Count - 1;

            EquipGun(equippedGuns[gunIndex]);
        }
    }*/

    /*void OldFireGun()
    {
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;

        // pressed the fire button
        if(GetPressFire())
        {
            currentGun?.AttemptFire();
        }

        // holding the fire button (for automatic)
        else if(GetHoldFire())
        {
            if (currentGun.AttemptAutomaticFire())
                currentGun?.AttemptFire();
        }

        // pressed the alt fire button
        if (GetPressAltFire())
        {
            currentGun?.AttemptAltFire();
        }
    }*/

    public void EquipGun(Gun g)
    {
        // disable current gun, if there is one
        currentGun?.Unequip();
        currentGun?.gameObject.SetActive(false);

        // enable the new gun
        g.gameObject.SetActive(true);
        g.transform.parent = gunHold;
        g.transform.localPosition = Vector3.zero;
        currentGun = g;

        g.Equip(this);
    }

    // public methods

    public void AddGun(Gun g)
    {
        // add new gun to the list
        equippedGuns.Add(g);

        // our index is the last one/new one
        gunIndex = equippedGuns.Count - 1;

        // put gun in the right place
        EquipGun(g);
    }

    public void IncreaseAmmo(int amount)
    {
        currentGun.AddAmmo(amount);
    }

    public void Respawn()
    {
        transform.position = origin;
    }

    // Input methods

    /*bool GetPressFire()
    {
        return InputManager.controls.Player.SwitchGun.ReadValue<float>() > 0;
    }


    bool GetPressAltFire()
    {
        return InputManager.controls.Player.SwitchGun.ReadValue<float>() < 0;
    }

    Vector2 GetPlayerMovementVector()
    {
        return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    }

    /*Vector2 GetPlayerLook()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }*/
    public bool GetHoldFire()
    {
        return autoFiring;
    }

    bool GetSprint()
    {
        return sprinting;
    }

    // Collision methods

    // Character Controller can't use OnCollisionEnter :D thanks Unity
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.GetComponent<Damager>())
        {
            var collisionPoint = hit.collider.ClosestPoint(transform.position);
            var knockbackAngle = (transform.position - collisionPoint).normalized;
            velocity = (20 * knockbackAngle);
        }

        if (hit.gameObject.GetComponent <KillZone>())
        {
            Respawn();
        }
    }

    //New Input Functions

    public void Jump(InputAction.CallbackContext ctx)
    {
        if (grounded)
            velocity.y += Mathf.Sqrt(jumpForce * -1 * gravity);
    }

    public void Look()
    {
        Vector2 looking = InputManager.controls.Player.Look.ReadValue<Vector2>();
        float lookX = looking.x * lookSensitivityX * Time.deltaTime;
        float lookY = looking.y * lookSensitivityY * Time.deltaTime;

        xRotation -= lookY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cam.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * lookX);
    }

    public void SwitchGun(InputAction.CallbackContext ctx)
    {
        if (equippedGuns.Count == 0)
            return;

        if (InputManager.controls.Player.SwitchGun.ReadValue<float>() > 0)
        {
            gunIndex++;
            if (gunIndex > equippedGuns.Count - 1)
                gunIndex = 0;

            EquipGun(equippedGuns[gunIndex]);
        }

        else if (InputManager.controls.Player.SwitchGun.ReadValue<float>() < 0)
        {
            gunIndex--;
            if (gunIndex < 0)
                gunIndex = equippedGuns.Count - 1;

            EquipGun(equippedGuns[gunIndex]);
        }
    }

    public void Movement()
    {
        if (grounded && velocity.y < 0)
        {
            velocity.y = -1;// -0.5f;
        }

        Vector2 movement = InputManager.controls.Player.Movement.ReadValue<Vector2>();
        Vector3 move = transform.right * movement.x + transform.forward * movement.y;
        controller.Move(move * movementSpeed * (GetSprint() ? 2 : 1) * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void FireGun(InputAction.CallbackContext ctx)
    {
        autoFiring = true;
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;


        // pressed the fire button
        currentGun?.AttemptFire();

    }

    public void FireAutomaticGun()
    {
        if (GetHoldFire())
        {
            if (currentGun == null)
                return;
            if (currentGun.AttemptAutomaticFire())
                currentGun?.AttemptFire();
        }
    }

    public void AltFireGun(InputAction.CallbackContext ctx)
    {
        // don't fire if we don't have a gun
        if (currentGun == null)
            return;
        currentGun?.AttemptAltFire();
    }

    private void FireGunCanceled(InputAction.CallbackContext context)
    {
        autoFiring = false;
    }


    private void Sprint(InputAction.CallbackContext context)
    {
        sprinting = true;
    }
    private void SprintCanceled(InputAction.CallbackContext context)
    {
        sprinting = false;
    }
}
