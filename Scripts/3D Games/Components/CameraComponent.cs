using Godot;

/* Notes:
    Remember to, in "physics/common", change the Jitter Fix slide to 0, and activate Physics Interpolation.
    It helps with the camera jittering when circle-strafing due to the difference between process and physics frames.
*/

public partial class CameraComponent : Node3D
{
    // Setup
    [Export] private bool RotateParent = true;

    // References
    private Node3D CurrentParent;

    // Camera
    [Export] private float MouseSensitivity = 0.005f;
    [Export] private float MaxVerticalAngleInRadians = 1.5f;

    public override void _Ready()
    {
        if (GetParent() != null)
        {
            CurrentParent = (Node3D)GetParent();
        }
        else
        {
            GD.PrintErr($"[{Name}] Has no valid Parent Node3D.");
        }
    }

    public override async void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (Input.MouseMode != Input.MouseModeEnum.Captured)
            {
                GD.PrintErr($"[{Name}] Mouse is not captured!");
                return;
            }

            // Horizontal rotation
            if (RotateParent && CurrentParent != null)
            {
                await ToSignal(GetTree(), "physics_frame");
                CurrentParent.RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            }
            else
            {
                await ToSignal(GetTree(), "physics_frame");
                this.RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            }

            // Vertical rotation (on self)
            float desiredXAngle = -mouseMotion.Relative.Y * MouseSensitivity;
            float newXRotation = Rotation.X + desiredXAngle;

            // Clamp rotation
            newXRotation = Mathf.Clamp(newXRotation, -MaxVerticalAngleInRadians, MaxVerticalAngleInRadians);
            Rotation = new Vector3(newXRotation, Rotation.Y, Rotation.Z);
        }
    }
}