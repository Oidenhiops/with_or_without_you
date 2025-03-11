using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CharacterInputs : MonoBehaviour
{
    [NonSerialized] public Character character;
    public GameObject[] mobileHUD;
    public CharacterActions characterActions;
    public CharacterActionsInfo characterActionsInfo;
    public GameObject attackDirection;
    public GameObject mousePos;
    public LayerMask layerMask;
    private float timeRestoreMovementMouse = 1f;
    public float restoreMovementMouse = 0;
    public bool isWebGlBuild;
    public static TypeDevice currentDevice;
    public TypeDevice currentDeviceInfo;
    private void Awake()
    {
        character = GetComponent<Character>();
        characterActions = new CharacterActions();
        characterActionsInfo = new CharacterActionsInfo();
    }
    public void OnEnable()
    {
        characterActions.Enable();
    }
    public void OnDisable()
    {
        characterActions.Disable();
    }
    void Update()
    {
        CurrentDevice();
        currentDeviceInfo = currentDevice;
        characterActionsInfo.mousePos = characterActions.CharacterInputs.MousePos.ReadValue<Vector2>();
        characterActionsInfo.movement = characterActions.CharacterInputs.Movement.ReadValue<Vector2>();
        characterActionsInfo.principalAttack = characterActions.CharacterInputs.PrincipalAttack;
        characterActionsInfo.unlockCamera = characterActions.CharacterInputs.UnlockCamera.IsInProgress();
        characterActionsInfo.moveCamera = DirectionPosition();
        characterActionsInfo.interact = characterActions.CharacterInputs.Interact;
        characterActionsInfo.enableSecondaryAction = characterActions.CharacterInputs.EnableSecondaryAction;
        characterActionsInfo.jump = characterActions.CharacterInputs.Jump;
        ValidateSecondaryAction();
        characterActionsInfo.changeObjectUp = characterActions.CharacterInputs.ChangeObjectUp;
        characterActionsInfo.changeObjectDown = characterActions.CharacterInputs.ChangeObjectDown;
        characterActionsInfo.changeSkillUp = characterActions.CharacterInputs.ChangeSkillUp;
        characterActionsInfo.changeSkillDown = characterActions.CharacterInputs.ChangeSkillDown;
        characterActionsInfo.changeObjectForTake = characterActions.CharacterInputs.ChangeObjectForTake.ReadValue<Vector2>();
        characterActionsInfo.changeObject1 = characterActions.CharacterInputs.ChangeObject1;
        characterActionsInfo.changeObject2 = characterActions.CharacterInputs.ChangeObject2;
        characterActionsInfo.changeObject3 = characterActions.CharacterInputs.ChangeObject3;
        characterActionsInfo.changeObject4 = characterActions.CharacterInputs.ChangeObject4;
        characterActionsInfo.changeObject5 = characterActions.CharacterInputs.ChangeObject5;
        characterActionsInfo.changeObject6 = characterActions.CharacterInputs.ChangeObject6;
        characterActionsInfo.useSkill = characterActions.CharacterInputs.UseSkill;
        characterActionsInfo.useObject = characterActions.CharacterInputs.UseObject;
        characterActionsInfo.openInventory = characterActions.CharacterInputs.OpenInventory;
        characterActionsInfo.lookEnemy = characterActions.CharacterInputs.LookEnemy;
        characterActionsInfo.pause = characterActions.CharacterInputs.Pause;
        ValidatePause();
    }
    void ValidatePause()
    {
        if (characterActionsInfo.pause.triggered && !SceneManager.GetSceneByName("OptionsScene").isLoaded)
        {
            SceneManager.LoadScene("OptionsScene", LoadSceneMode.Additive);
        }
    }
    void ValidateSecondaryAction()
    {
        if (currentDevice != TypeDevice.MOBILE)
        {
            if (characterActionsInfo.enableSecondaryAction.IsPressed())
            {
                characterActionsInfo.isSecondaryAction = true;
            }
            else
            {
                characterActionsInfo.isSecondaryAction = false;
            }
        }
    }
    public void SwapSecondaryAction()
    {
        characterActionsInfo.isSecondaryAction = !characterActionsInfo.isSecondaryAction;
    }
    void CurrentDevice()
    {
        if (!isWebGlBuild)
        {
            if (ValidateDeviceIsMobile())
            {
                currentDevice = TypeDevice.MOBILE;
                EnabledMobileHUD();
            }
            else if (IsGamepadInput())
            {
                currentDevice = TypeDevice.GAMEPAD;
                DisabledMobileHUD();
            }
            else if (ValidateDeviceIsPc())
            {
                currentDevice = TypeDevice.PC;
                DisabledMobileHUD();
            }
        }
        else
        {
            currentDevice = TypeDevice.PC;
            DisabledMobileHUD();
        }
    }
    bool ValidateDeviceIsMobile(){
        return Touchscreen.current != null;
    }
    bool ValidateDeviceIsPc(){
        return Keyboard.current.anyKey.wasPressedThisFrame ||
            Mouse.current.leftButton.wasPressedThisFrame ||
            Mouse.current.rightButton.wasPressedThisFrame ||
            Mouse.current.scroll.ReadValue() != Vector2.zero ||
            Mouse.current.delta.ReadValue() != Vector2.zero;
    }
    bool IsGamepadInput()
    {
        Gamepad gamepad = Gamepad.current;
        if (gamepad == null) return false;
        bool currentDeviceIsGamepad = Gamepad.current != null;
        bool validateAnyGamepadInput = gamepad.buttonSouth.wasPressedThisFrame ||
               gamepad.buttonNorth.wasPressedThisFrame ||
               gamepad.buttonEast.wasPressedThisFrame ||
               gamepad.buttonWest.wasPressedThisFrame ||
               gamepad.leftStick.ReadValue() != Vector2.zero ||
               gamepad.rightStick.ReadValue() != Vector2.zero ||
               gamepad.dpad.ReadValue() != Vector2.zero ||
               gamepad.leftTrigger.wasPressedThisFrame ||
               gamepad.rightTrigger.wasPressedThisFrame;
        return currentDeviceIsGamepad && validateAnyGamepadInput;
    }
    Vector2 DirectionPosition()
    {
        if (characterActions.CharacterInputs.UnlockCamera.IsInProgress())
        {
            return Vector2.zero;
        }
        if (characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>() != Vector2.zero || restoreMovementMouse > 0 && characterActionsInfo.principalAttack.triggered || restoreMovementMouse > 0 && characterActionsInfo.useSkill.triggered)
        {
            restoreMovementMouse = timeRestoreMovementMouse;
        }
        if (restoreMovementMouse > 0)
        {
            restoreMovementMouse -= Time.deltaTime;
            ValidateShowMouse(true);
            if (currentDevice == TypeDevice.PC)
            {
                Ray ray = Camera.main.ScreenPointToRay(character.characterInputs.characterActionsInfo.mousePos);
                if (Physics.Raycast(ray, out RaycastHit raycastHit, Mathf.Infinity, layerMask))
                {
                    mousePos.transform.position = raycastHit.point;
                    Vector2 direction = new Vector2(mousePos.transform.localPosition.x, mousePos.transform.localPosition.z).normalized;
                    return direction;
                }
            }
            else if (currentDevice == TypeDevice.GAMEPAD || currentDevice == TypeDevice.MOBILE)
            {
                if (characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>() != Vector2.zero)
                {
                    return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
                }
                else
                {
                    return characterActionsInfo.moveCamera;
                }
            }
        }
        else
        {
            ValidateShowMouse(false);
        }
        if (currentDevice == TypeDevice.GAMEPAD)
        {
            return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
        }
        if (currentDevice == TypeDevice.MOBILE && characterActionsInfo.principalAttack.IsPressed())
        {
            return characterActions.CharacterInputs.MoveCamera.ReadValue<Vector2>();
        }
        return Vector2.zero;
    }
    void ValidateShowMouse(bool showAttackDiection)
    {
        if (currentDevice == TypeDevice.GAMEPAD || currentDevice == TypeDevice.MOBILE)
        {
            Cursor.visible = false;
            if (showAttackDiection)
            {
                attackDirection.SetActive(true);
            }
            else
            {
                attackDirection.SetActive(false);
            }
        }
        else
        {
            if (showAttackDiection)
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
            attackDirection.SetActive(false);
        }
    }
    void EnabledMobileHUD()
    {
        foreach (GameObject hud in mobileHUD)
        {
            hud.SetActive(true);
        }
    }
    void DisabledMobileHUD()
    {
        foreach (GameObject hud in mobileHUD)
        {
            hud.SetActive(false);
        }
    }
    [Serializable]
    public class CharacterActionsInfo
    {
        public Vector2 movement = Vector2.zero;
        public Vector2 moveCamera = Vector2.zero;
        public bool unlockCamera = false;
        public Vector2 changeObjectForTake = Vector2.zero;
        public bool isSecondaryAction = false;
        public Vector2 mousePos;
        public InputAction principalAttack;
        public InputAction interact;
        public InputAction enableSecondaryAction;
        public InputAction jump;
        public InputAction changeSkillUp;
        public InputAction changeSkillDown;
        public InputAction changeObjectUp;
        public InputAction changeObjectDown;
        public InputAction changeObject1;
        public InputAction changeObject2;
        public InputAction changeObject3;
        public InputAction changeObject4;
        public InputAction changeObject5;
        public InputAction changeObject6;
        public InputAction useSkill;
        public InputAction useObject;
        public InputAction openInventory;
        public InputAction lookEnemy;
        public InputAction pause;
    }
    public enum TypeDevice
    {
        None,
        PC,
        GAMEPAD,
        MOBILE,
    }
}
