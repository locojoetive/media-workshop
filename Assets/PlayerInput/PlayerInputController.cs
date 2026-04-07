using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerInputType
{
    LeftStick,
    RightStick,
    DPad,
    LeftTrigger,
    RightTrigger,
    ButtonEast,
    ButtonNorth,
    ButtonSouth,
    ButtonWest,
    LeftShoulder,
    RightShoulder,
    Select,
    Pause,
    MousePosition,
    MouseLeftButton,
}

public class PlayerInputController : MonoBehaviour
{
    public Vector2 leftStickDirection;
    public Vector2 rightStickDirection;
    public Vector2Int dPadDirection;
    public Vector2 mousePosition;

    public float leftTriggerValue;
    public float rightTriggerValue;

    public bool buttonEastPressed;
    public bool buttonNorthPressed;
    public bool buttonSouthPressed;
    public bool buttonWestPressed;
    public bool leftShoulderPressed;
    public bool rightShoulderPressed;
    public bool selectPressed;
    public bool pausePressed;
    public bool mouseLeftButtonPressed;

    public void OnLeftStick(InputValue inputValue)
    {
        leftStickDirection = inputValue.Get<Vector2>();
    }

    public void OnRightStick(InputValue inputValue)
    {
        rightStickDirection = inputValue.Get<Vector2>();
    }

    public void OnDPad(InputValue inputValue)
    {
        var dPadValue = inputValue.Get<Vector2>();
        dPadDirection = new Vector2Int(
            (int) dPadValue.x,
            (int) dPadValue.y
        );
    }

    public void OnLeftTrigger(InputValue inputValue)
    {
        leftTriggerValue = inputValue.Get<float>();
    }
    public void OnRightTrigger(InputValue inputValue)
    {
        rightTriggerValue = inputValue.Get<float>();
    }

    public void OnLeftShoulder(InputValue inputValue)
    {
        leftShoulderPressed = inputValue.isPressed;
    }

    public void OnRightShoulder(InputValue inputValue)
    {
        rightShoulderPressed = inputValue.isPressed;
    }

    public void OnButtonNorth(InputValue inputValue)
    {
        buttonNorthPressed = inputValue.isPressed;
    }

    public void OnButtonEast(InputValue inputValue)
    {
        buttonEastPressed = inputValue.isPressed;
    }

    public void OnButtonSouth(InputValue inputValue)
    {
        buttonSouthPressed = inputValue.isPressed;
    }

    public void OnButtonWest(InputValue inputValue)
    {
        buttonWestPressed = inputValue.isPressed;
    }

    public void OnSelect(InputValue inputValue)
    {
        selectPressed = inputValue.isPressed;
    }

    public void OnPause(InputValue inputValue)
    {
        pausePressed = inputValue.isPressed;
    }

    public void OnMousePosition(InputValue inputValue)
    {
        mousePosition = inputValue.Get<Vector2>();
    }

    public void OnMouseLeftButton(InputValue inputValue)
    {
        mouseLeftButtonPressed = inputValue.isPressed;
    }

    public T GetInputValue<T>(PlayerInputType inputType)
    {
        return inputType switch
        {
            PlayerInputType.LeftStick => (T)(object)leftStickDirection,
            PlayerInputType.RightStick => (T)(object)rightStickDirection,
            PlayerInputType.DPad => (T)(object)dPadDirection,
            PlayerInputType.LeftTrigger => (T)(object)leftTriggerValue,
            PlayerInputType.RightTrigger => (T)(object)rightTriggerValue,
            PlayerInputType.ButtonEast => (T)(object)buttonEastPressed,
            PlayerInputType.ButtonNorth => (T)(object)buttonNorthPressed,
            PlayerInputType.ButtonSouth => (T)(object)buttonSouthPressed,
            PlayerInputType.ButtonWest => (T)(object)buttonWestPressed,
            PlayerInputType.LeftShoulder => (T)(object)leftShoulderPressed,
            PlayerInputType.RightShoulder => (T)(object)rightShoulderPressed,
            PlayerInputType.Select => (T)(object)selectPressed,
            PlayerInputType.Pause => (T)(object)pausePressed,
            PlayerInputType.MousePosition => (T)(object)mousePosition,
            PlayerInputType.MouseLeftButton => (T)(object)mouseLeftButtonPressed,
            _ => throw new System.ArgumentException($"Unsupported input type: {inputType}")
        };
    }
}
