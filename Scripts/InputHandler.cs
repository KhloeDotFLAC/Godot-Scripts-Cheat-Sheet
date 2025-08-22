using System;
using Godot;

public partial class InputHandler : Node
{
    // Mouse presses
    public static Action MouseLeft;
    public static Action MouseRight;

    // Button presses
    public static Action PressedTab;
    public static Action PressedShift;
    public static Action PressedCtrl;
    public static Action PressedEsc;

    // Holding Properties
    public static bool IsHoldingTab = false;
    public static bool IsHoldingShift = false;
    public static bool IsHoldingCtrl = false;

    public override void _Input(InputEvent @event)
    {
        // [Events]
        if (@event is InputEventKey pKey)
        {
            if (pKey.IsPressed() && (pKey.Keycode == Key.Escape))
            {
                // Pressed Escape
                PressedEsc?.Invoke();
            }

            if (pKey.IsPressed() && (pKey.Keycode == Key.Tab))
            {
                // Pressed Tab
                PressedTab?.Invoke();
            }

            if (pKey.IsPressed() && (pKey.Keycode == Key.Shift))
            {
                // Pressed Shift
                PressedShift?.Invoke();
            }

            if (pKey.IsPressed() && (pKey.Keycode == Key.Ctrl))
            {
                // Pressed Ctrl
                PressedCtrl?.Invoke();
            }
        }

        // [Holds]
        if (@event is InputEventKey curKey)
        {
            if (curKey.IsPressed() && (curKey.Keycode == Key.Tab))
            {
                // Holding Tab key
                IsHoldingTab = true;
            }
            else if (curKey.IsReleased() && (curKey.Keycode == Key.Tab))
            {
                // Released Tab key
                IsHoldingTab = false;
            }

            if (curKey.IsPressed() && (curKey.Keycode == Key.Shift))
            {
                // Holding Shift key
                IsHoldingShift = true;
            }
            else if (curKey.IsReleased() && (curKey.Keycode == Key.Shift))
            {
                // Released Shift key
                IsHoldingShift = false;
            }

            if (curKey.IsPressed() && (curKey.Keycode == Key.Ctrl))
            {
                // Holding Shift key
                IsHoldingCtrl = true;
            }
            else if (curKey.IsReleased() && (curKey.Keycode == Key.Ctrl))
            {
                // Released Shift key
                IsHoldingCtrl = false;
            }
        }

        // [Mouse]
        if (@event is InputEventMouse CurMouseEvent)
        {
            if (CurMouseEvent.IsPressed())
            {
                if (CurMouseEvent.ButtonMask == MouseButtonMask.Left)
                {
                    // Pressed LMB
                    MouseLeft?.Invoke();
                }

                if (CurMouseEvent.ButtonMask == MouseButtonMask.Right)
                {
                    // Pressed RMB
                    MouseRight?.Invoke();
                }
            }

        }
    }
}