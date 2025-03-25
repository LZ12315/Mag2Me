using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour,IMagSourceControl
{
    private PlayerInputControl inputControl;
    private PhysicalCharacter physicalCharacter;
    private MagSource magSource;
    private EquipHolder equipHolder;
    private MagAnimation magAnimation;

    [Header("移动参数")]
    [SerializeField] private float normalSpeed;
    [SerializeField] private bool canMove = true;

    [Header("输入设置")]
    [SerializeField] bool mouseControl;
    [SerializeField] float shootPressLimit = 1.5f;
    [SerializeField] private float lowFrequency = 0.5f;  // 低频震动
    [SerializeField] private float highFrequency = 0.5f;  // 高频震动
    [SerializeField] private float duration = 0.5f;  // 震动持续时间

    Vector2 moveInput;
    Vector2 LookInput;
    [SerializeField] Vector2 loookDir = new Vector2(1, 0);
    private Camera _mainCamera;


    private void Awake()
    {
        inputControl = new PlayerInputControl();
        equipHolder = this?.GetComponent<EquipHolder>();
        magSource = this?.GetComponent<MagSource>();
        physicalCharacter = this?.GetComponent<PhysicalCharacter>();
        magAnimation = GetComponentInChildren<MagAnimation>();
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        Move();
        Look();
    }

    void AttractStart(InputAction.CallbackContext text)
    {
        magSource.ExcuteSnap(this);
        magAnimation.PullVFX(magSource, true);
    }

    void AttractOver(InputAction.CallbackContext text)
    {
        magSource.SnapStop(this);
        magAnimation.PullVFX(magSource, false);
    }

    public void SnapObject(MagSource source)
    {
        if(source != magSource) return;
        if (Gamepad.current != null)
            StartCoroutine(VibrateController());
        magAnimation.SnapVFX(magSource);
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

        if (Gamepad.current != null)
            StartCoroutine(VibrateController());
        magAnimation.PushVFX(magSource, true);
        pressStartTime = 0;
    }

    IEnumerator VibrateController()
    {
        if (Gamepad.current != null)
        {
            // 设置手柄震动
            Gamepad.current.SetMotorSpeeds(lowFrequency, highFrequency);

            // 等待指定的持续时间
            yield return new WaitForSeconds(duration);

            // 停止震动
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    private void Move()
    {
        moveInput = inputControl.Player.Move.ReadValue<Vector2>();

        if(!Mathf.Approximately(moveInput.magnitude, 0))
            physicalCharacter.SetVelocity(moveInput.normalized, normalSpeed);
        else
            physicalCharacter.Idle();
    }

    private void Look()
    {
        if (!mouseControl)
            LookInput = inputControl.Player.Look.ReadValue<Vector2>();
        else
        {
            Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            LookInput = (Vector2)(mouseWorldPos - transform.position);
        }

        loookDir = LookInput.normalized;
        if(Mathf.Approximately(loookDir.magnitude,0))
            loookDir = physicalCharacter.Orientation;

        magSource.SetSnapDir(this, loookDir);
    }

    #region 其他

    public Vector2 LookDir => loookDir;

    private void OnEnable()
    {
        inputControl.Enable();
        inputControl.Player.Snap.started += AttractStart;
        inputControl.Player.Fire.started += ShootPerformed;
        inputControl.Player.Snap.canceled += AttractOver;
        inputControl.Player.Fire.canceled += ShootOver;
    }

    private void OnDisable()
    {
        inputControl.Disable();
        inputControl.Player.Snap.started -= AttractStart;
        inputControl.Player.Fire.started -= ShootPerformed;
        inputControl.Player.Snap.canceled -= AttractOver;
        inputControl.Player.Fire.canceled -= ShootOver;
    }

    #endregion

}
