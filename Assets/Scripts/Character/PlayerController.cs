using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
    private PhysicalCharacter physicalCharacter;
    private MagSource snapSource;
    private EquipHolder equipHolder;

    [Header("移动参数")]
    [SerializeField] private float normalSpeed;
    [SerializeField] private bool canMove = true;

    [Header("输入设置")]
    [SerializeField] bool mouseControl;
    [SerializeField] float shootPressLimit = 1.5f;

    Vector2 moveInput;
    Vector2 LookInput;
    Vector2 loookDir = new Vector2(1, 0);
    private Camera _mainCamera;


    private void Awake()
    {
        inputControl = new PlayerInputControl();
        equipHolder = this?.GetComponent<EquipHolder>();
        snapSource = this?.GetComponent<MagSource>();
        physicalCharacter = this?.GetComponent<PhysicalCharacter>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Move();
        Look();
    }

    void SnapStart(InputAction.CallbackContext text)
    {
        snapSource.isSnap = true;
    }

    void SnapOver(InputAction.CallbackContext text)
    {
        snapSource.isSnap = false;
    }

    float pressStartTime;
    void ShootPerformed(InputAction.CallbackContext text)
    {
        pressStartTime = Time.time;
    }

    void ShootOver(InputAction.CallbackContext text)
    {
        if (equipHolder == null) return;

        float pressTime = Time.time - pressStartTime;
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        if (pressTime >= shootPressLimit)
            equipHolder.Scatter((Vector2)mouseWorldPos);
        else
            equipHolder.Shoot(loookDir);

        pressStartTime = 0;
    }

    private void Move()
    {
        moveInput = inputControl.Player.Move.ReadValue<Vector2>();

        if(!Mathf.Approximately(moveInput.magnitude, 0))
            physicalCharacter.SetVelocity(moveInput.normalized, normalSpeed);
        else
            physicalCharacter.ToRoam();
        //Vector3 moveStep = moveInput.normalized * normalSpeed * Time.deltaTime;
        //transform.position += moveStep;
    }

    private void Look()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        if (!mouseControl)
            LookInput = inputControl.Player.Look.ReadValue<Vector2>();
        else
            LookInput = (Vector2)(mouseWorldPos - transform.position);
        loookDir = LookInput.normalized;

        snapSource.SnapDir = loookDir;
    }

    #region 其他

    private void OnEnable()
    {
        inputControl.Enable();
        inputControl.Player.Snap.started += SnapStart;
        inputControl.Player.Fire.started += ShootPerformed;
        inputControl.Player.Snap.canceled += SnapOver;
        inputControl.Player.Fire.canceled += ShootOver;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.Player.Snap.started -= SnapStart;
        inputControl.Player.Fire.started -= ShootPerformed;
        inputControl.Player.Snap.canceled -= SnapOver;
        inputControl.Player.Fire.canceled -= ShootOver;
    }

    #endregion

}
