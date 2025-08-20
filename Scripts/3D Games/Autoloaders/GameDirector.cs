using Godot;

public partial class GameDirector : Node
{
    // Setup
    private static bool DoMouseLocking = true; 

    // Global References
    public static Variant PlayerReference;

    public SceneTree SceneTreeReference => GetTree();

    public override void _Input(InputEvent @event)
    {
        if (DoMouseLocking)
        {
            // Handles mouse behaviour on First-Person/Third-Person games.
            if (@event.IsActionReleased("ui_cancel"))
            {
                Input.MouseMode = Input.MouseModeEnum.Visible;
            }
            else if (@event is InputEventMouse CurMouseEvent)
            {
                if (CurMouseEvent.IsPressed() && CurMouseEvent.ButtonMask == MouseButtonMask.Left)
                    Input.MouseMode = Input.MouseModeEnum.Captured;
            }
        }
    }
}