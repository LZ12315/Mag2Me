using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private PlayerInputControl inputControl;
    private Rigidbody2D rb;
    private EquipHolder equipHolder;

    [Header("移动参数")]
    [SerializeField] private float normalSpeed;
    [SerializeField] private bool canMove = true;

    [Header("输入设置")]
    [SerializeField] bool mouseControl;
    [SerializeField] float shootPressLimit = 0.5f;

    Vector2 moveInput;
    Vector2 LookInput;
    Vector2 lastLookDir = new Vector2(1, 0);
    Vector2 screenCenter;

    private void Awake()
    {
        inputControl = new PlayerInputControl();
        rb = GetComponent<Rigidbody2D>();
        equipHolder = this?.GetComponent<EquipHolder>();
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
    }

    private void Update()
    {
        Move();
        Look();
    }


    bool snapTrigger;
    void SnapStart(InputAction.CallbackContext text)
    {
        snapTrigger = true;
    }

    void SnapOver(InputAction.CallbackContext text)
    {
        snapTrigger = false;
    }

    float shootPressCount;
    void ShootPerformed(InputAction.CallbackContext text)
    {
        shootPressCount += Time.deltaTime;
    }

    void ShootOver(InputAction.CallbackContext text)
    {
        if(equipHolder != null)
            equipHolder.Shoot(lastLookDir);
        shootPressCount = 0;
    }

    private void Move()
    {
        moveInput = inputControl.Player.Move.ReadValue<Vector2>();

        Vector3 moveStep = moveInput.normalized * normalSpeed * Time.deltaTime;
        transform.position += moveStep;
    }

    private void Look()
    {
        if (!mouseControl)
            LookInput = inputControl.Player.Look.ReadValue<Vector2>();
        else
            LookInput = (Mouse.current.position.ReadValue() - screenCenter);
        lastLookDir = LookInput.normalized;
    }

    #region 其他

    private void OnEnable()
    {
        inputControl.Enable();
        inputControl.Player.Snap.started += SnapStart;
        inputControl.Player.Fire.performed += ShootPerformed;
        inputControl.Player.Snap.canceled += SnapOver;
        inputControl.Player.Fire.canceled += ShootOver;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.Player.Snap.started -= SnapStart;
        inputControl.Player.Fire.performed -= ShootPerformed;
        inputControl.Player.Snap.canceled -= SnapOver;
        inputControl.Player.Fire.canceled -= ShootOver;
    }

    public Vector2 LookDir => lastLookDir;
    public bool SnapTrigger => snapTrigger;

    #endregion

}
