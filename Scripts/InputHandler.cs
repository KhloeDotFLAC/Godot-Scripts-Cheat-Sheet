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
            switch (pKey.Keycode)
            {
                case Key.Escape:
                    // Pressed Escape
                    PressedEsc?.Invoke();            
                    break;
                case Key.Tab:
                    // Pressed Tab
                    PressedTab?.Invoke();

                    // Holding Tab
                    IsHoldingTab = pKey.Pressed;
                    GD.Print("Holding tab" + IsHoldingTab);
                    break;
                case Key.Shift:
                    // Pressed Shift
                    PressedShift?.Invoke();

                    // Holding Shift
                    IsHoldingShift = pKey.Pressed;
                    break;
                case Key.Ctrl:
                    // Pressed Ctrl
                    PressedCtrl?.Invoke();

                    // Holding Ctrl
                    IsHoldingCtrl = pKey.Pressed;
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