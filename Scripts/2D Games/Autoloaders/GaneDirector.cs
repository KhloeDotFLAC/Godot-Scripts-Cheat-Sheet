using Godot;

public partial class GameDirector : Node
{
    // Global References
    public static Player PlayerReference;

    public static SceneTree SceneTreeReference => GetTree();
}