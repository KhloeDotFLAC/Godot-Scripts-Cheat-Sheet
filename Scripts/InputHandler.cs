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
        if (@event is InputEventKey CurKey)
        {
            switch (CurKey.Keycode)
            {
                case Key.Escape:
                    // Pressed Escape
                    if (CurKey.Pressed) PressedEsc?.Invoke();
                    break;
                case Key.Tab:
                    // Pressed Tab
                    if (CurKey.Pressed) PressedTab?.Invoke();

                    // Holding Tab
                    IsHoldingTab = CurKey.Pressed;
                    break;
                case Key.Shift:
                    // Pressed Shift
                    if (CurKey.Pressed) PressedShift?.Invoke();

                    // Holding Shift
                    IsHoldingShift = CurKey.Pressed;
                    break;
                case Key.Ctrl:
                    // Pressed Ctrl
                    if (CurKey.Pressed) PressedCtrl?.Invoke();

                    // Holding Ctrl
                    IsHoldingCtrl = CurKey.Pressed;
                    break;
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