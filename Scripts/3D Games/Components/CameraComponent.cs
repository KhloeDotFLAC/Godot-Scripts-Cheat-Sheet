using Godot;

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

    public override void _Input(InputEvent @event)
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
                CurrentParent.RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            else
                this.RotateY(-mouseMotion.Relative.X * MouseSensitivity);

            // Vertical rotation (on self)
            float desiredXAngle = -mouseMotion.Relative.Y * MouseSensitivity;
            float newXRotation = Rotation.X + desiredXAngle;

            // Clamp rotation
            newXRotation = Mathf.Clamp(newXRotation, -MaxVerticalAngleInRadians, MaxVerticalAngleInRadians);
            Rotation = new Vector3(newXRotation, Rotation.Y, Rotation.Z);
        }
    }
}