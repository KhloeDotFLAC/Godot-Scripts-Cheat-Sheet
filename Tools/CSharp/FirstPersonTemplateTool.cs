using Godot;
using System;

[Tool]
[GlobalClass]
public partial class FirstPersonTemplateTool : EditorScript
{
	// Editable Variables
	private string PlayerName = "GeneratedPlayer";
	private float PlayerRadius = 0.3f;

	private float PlayerHeightInMeters = 1.8f;
	private float PlayerCrouchHeightInMeters = 1f;

	private string PlayerClassName = "Player";

	private string PlayerSceneDirectory = "res://";
	private string PlayerScriptDirectory = "res://";
	private string HeadComponentScriptDirectory = "res://";

	// Player Nodes
	private CharacterBody3D Player;
	private CollisionShape3D StandingCollisionShape3D;
	private CollisionShape3D CrouchingCollisionShape3D;
	private Node3D Head;
	private Camera3D Camera;
	private RayCast3D InteractionRay;
	private RayCast3D CrouchRay;

	private PackedScene PlayerScene = new PackedScene();

	// Scripts Resource
	private CSharpScript HeadComponentScript;
	private CSharpScript PlayerScript;

	private Window CurrentWindow;

	private Label ScenePathPreviewLabel;
	private Label PlayerScriptPathPreviewLabel;
	private Label HeadComponentScriptPathPreviewLabel;

	public override void _Run()
	{
		CreateWindow();
	}

	private void CreateWindow()
	{
		CurrentWindow = new Window()
		{
			Title = "Barebones Player Creator",
			MinSize = new Vector2I(455, 415),
			MaxSize = new Vector2I(455, 415),
			Position = DisplayServer.MouseGetPosition(),
			ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems,
			ContentScaleAspect = Window.ContentScaleAspectEnum.Expand,
		};
		CurrentWindow.CloseRequested += () => CloseWindow();

		CreateWindowGUI();

		EditorInterface.Singleton.PopupDialog(CurrentWindow);
    }
	
	private void CloseWindow()
	{
		if (CurrentWindow == null) return;
		CurrentWindow.QueueFree();
	}

    private void CreateWindowGUI()
	{
		Control BaseGUI = new Control()
		{
			Size = CurrentWindow.Size
		};

		TabContainer TabWindow = new TabContainer
		{
			TabAlignment = TabBar.AlignmentMode.Center,
			Size = BaseGUI.Size
		};

		TabBar FirstPersonTab = new TabBar()
		{
			Name = "First Person or Third Person"
		};

		VBoxContainer MainContainer = new VBoxContainer()
		{
			Size = CurrentWindow.Size
		};

		// Editable fields
		ContextButton PlayerSceneNameCB = new ContextButton()
		{
			LabelText = "Object Name: ",
			DefaultValue = PlayerName
		};

		ContextButton PlayerClassNameCB = new ContextButton()
		{
			LabelText = "Class Name:",
			DefaultValue = PlayerClassName
		};

		ContextButton PlayerHeightCB = new ContextButton()
		{
			LabelText = "Collision Height in cm: ",
			DefaultValue = PlayerHeightInMeters.ToString()
		};

		ContextButton PlayerRadiusCB = new ContextButton()
		{
			LabelText = "Collision Radius in cm: ",
			DefaultValue = PlayerRadius.ToString()
		};

		ContextButton PlayerCrouchHeightCB = new ContextButton()
		{
			LabelText = "Collision Crouch Height in cm: ",
			DefaultValue = PlayerCrouchHeightInMeters.ToString()
		};

		ContextButton SavePlayerSceneCB = new ContextButton()
		{
			LabelText = "Player Scene Path: ",
			DefaultValue = PlayerSceneDirectory
		};

		ContextButton SavePlayerScriptCB = new()
		{
			LabelText = "Player Script Path: ",
			DefaultValue = PlayerScriptDirectory
		};

		ContextButton SaveHeadScriptCB = new()
		{
			LabelText = "Head Component Path: ",
			DefaultValue = HeadComponentScriptDirectory
		};

		ScenePathPreviewLabel = new Label()
		{
			Text = $"Player Scene path: {PlayerSceneDirectory}{PlayerName}.tscn"
		};
		PlayerScriptPathPreviewLabel = new Label()
		{
			Text = $"Player Script path: {PlayerScriptDirectory}{PlayerClassName}.cs"
		};
		HeadComponentScriptPathPreviewLabel = new Label()
		{
			Text = $"Head Component Script path: {HeadComponentScriptDirectory}HeadComponent.cs"
		};

		Button CreatePlayerButton = new Button()
		{
			Text = "Create Player"
		};

		// Creating Hierarchy
		BaseGUI.AddChild(TabWindow);
		TabWindow.AddChild(FirstPersonTab);
		FirstPersonTab.AddChild(MainContainer);

		MainContainer.AddChild(PlayerSceneNameCB);
		MainContainer.AddChild(PlayerClassNameCB);
		MainContainer.AddChild(PlayerHeightCB);
		MainContainer.AddChild(PlayerRadiusCB);
		MainContainer.AddChild(PlayerCrouchHeightCB);
		MainContainer.AddChild(SavePlayerSceneCB);
		MainContainer.AddChild(SavePlayerScriptCB);
		MainContainer.AddChild(SaveHeadScriptCB);

		MainContainer.AddChild(ScenePathPreviewLabel);
		MainContainer.AddChild(PlayerScriptPathPreviewLabel);
		MainContainer.AddChild(HeadComponentScriptPathPreviewLabel);

		MainContainer.AddChild(CreatePlayerButton);

		// Signal connecting magic
		PlayerSceneNameCB.TextChanged += ChangePlayerSceneName;
		PlayerClassNameCB.TextChanged += ChangePlayerClassName;

		SavePlayerSceneCB.TextChanged += ChangePlayerSceneFolderDirectory;
		SavePlayerScriptCB.TextChanged += ChangePlayerScriptFolderDirectory;

		PlayerHeightCB.TextChanged += ChangePlayerHeight;
		PlayerRadiusCB.TextChanged += ChangePlayerRadius;

		PlayerCrouchHeightCB.TextChanged += ChangePlayerCrouchHeight;

		CreatePlayerButton.ButtonDown += GeneratePlayer;

		// Adding the GUI object to the Window

		CurrentWindow.AddChild(BaseGUI);
	}

    private void ChangePlayerHeight(string newHeight)
	{
		PlayerHeightInMeters = newHeight.ToFloat();
	}

	private void ChangePlayerRadius(string newRadius)
	{
		PlayerRadius = newRadius.ToFloat();
	}

	private void ChangePlayerCrouchHeight(string newCrouchHeight)
	{
		PlayerRadius = newCrouchHeight.ToFloat();
	}

	private void ChangePlayerSceneName(string newName)
	{
		PlayerName = newName;
		ScenePathPreviewLabel.Text = $"Player Scene will be saved at: {PlayerSceneDirectory}{PlayerName}.tscn";
	}

	private void ChangePlayerClassName(string newName)
	{
		PlayerClassName = newName;
		PlayerScriptPathPreviewLabel.Text = $"Player Script will be saved at: {PlayerScriptDirectory}{PlayerClassName}.cs";
	}

	private void ChangePlayerSceneFolderDirectory(string newPath)
	{
		PlayerSceneDirectory = newPath;
		ScenePathPreviewLabel.Text = $"Player Scene will be saved at: {PlayerSceneDirectory}{PlayerName}.tscn";
	}

	private void ChangePlayerScriptFolderDirectory(string newPath)
	{
		PlayerScriptDirectory = newPath;
		PlayerScriptPathPreviewLabel.Text = $"Player Script will be saved at: {PlayerScriptDirectory}{PlayerClassName}.tscn";
	}

	private void GeneratePlayer()
	{
		// Oops needed to create teh scripts first
		CreateScripts();

		// [Creating the Coli]
		StandingCollisionShape3D = new CollisionShape3D()
		{
			Name = "StandingCollisionShape",
			Shape = new CapsuleShape3D()
			{
				Height = PlayerHeightInMeters,
				Radius = PlayerRadius
			},

			Disabled = false
		};

		CrouchingCollisionShape3D = new CollisionShape3D()
		{
			Name = "CrouchingCollisionShape",
			Shape = new CapsuleShape3D()
			{
				Height = PlayerCrouchHeightInMeters,
				Radius = PlayerRadius
			},
			Position = new Vector3(0, -PlayerHeightInMeters / 2 + PlayerCrouchHeightInMeters / 2, 0),

			Disabled = true
		};

		CrouchRay = new RayCast3D()
		{
			Name = "CrouchRay",
			Position = CrouchingCollisionShape3D.Position,
			TargetPosition = new Vector3(0, PlayerHeightInMeters - (PlayerCrouchHeightInMeters / 2) - 0.5f, 0),
			Enabled = false
		};

		// [Head]
		Head = new SpringArm3D()
		{
			Name = "Head",
			SpringLength = 0,
			Position = new Vector3(0, (PlayerHeightInMeters / 2) - 0.3f, 0),
		};

		InteractionRay = new RayCast3D()
		{
			Name = "InteractionRay",
			TargetPosition = new Vector3(0, 0, -1.5f),
			Enabled = false
		};

		Camera = new Camera3D()
		{
			Name = "PlayerCamera"
		};

		Head.AddChild(Camera);
		Head.AddChild(InteractionRay);

		// [Player]
		Player = new CharacterBody3D()
		{
			Name = PlayerName
		};

		Player.AddChild(StandingCollisionShape3D);
		Player.AddChild(CrouchingCollisionShape3D);
		Player.AddChild(Head);
		Player.AddChild(CrouchRay);

		// Not realising I had to set the owner manually gave me MAD headache. I hate this.
		Camera.Owner = Player;
		InteractionRay.Owner = Player;
		CrouchRay.Owner = Player;
		StandingCollisionShape3D.Owner = Player;
		CrouchingCollisionShape3D.Owner = Player;
		Head.Owner = Player;

		Head.SetScript(GD.Load($"{HeadComponentScriptDirectory}HeadComponent.cs"));
		Player.SetScript(GD.Load(PlayerScriptDirectory + PlayerClassName + ".cs"));

		GD.Print($"[{GetClass()}] Created Player with {Player.GetChildCount()} childrens.");

		// [Create Scene]
		Error TryPack = PlayerScene.Pack(Player);
		if (TryPack != Error.Ok)
		{
			GD.PrintErr($"[{GetClass()}] Player packing failed. Error: {TryPack}");
		}

		SaveResource(PlayerScene, PlayerSceneDirectory + PlayerName + ".tscn");

		CloseWindow();
	}

	private void SaveResource(Resource What, string Path)
	{
		Error TrySave = ResourceSaver.Save(What, Path);
		if (TrySave != Error.Ok)
		{
			GD.PrintErr($"[{GetClass()}][Save Resource] Player Scene save failed. Error: {TrySave}");
			return;
		}
		GD.Print($"[{GetClass()}][Save Resource] Saved \"{What.GetClass()}\" resource to \"{Path}\"");
	}

	private void CreateScripts()
	{
		string SpringArmCode = @"using Godot;

public partial class HeadComponent : Node3D
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
            GD.PrintErr($""[{Name}] Has no valid Parent Node3D."");
        }
    }

    public override async void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseMotion mouseMotion)
        {
            if (Input.MouseMode != Input.MouseModeEnum.Captured)
            {
                GD.PrintErr($""[{Name}] Mouse is not captured!"");
                return;
            }

            // Horizontal rotation
            if (RotateParent && CurrentParent != null)
            {
                await ToSignal(GetTree(), ""physics_frame"");
                CurrentParent.RotateY(-mouseMotion.Relative.X * MouseSensitivity);
            }
            else
            {
                await ToSignal(GetTree(), ""physics_frame"");
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
}";
		string PlayerCode = $@"using Godot;

// Currently, with this setup, there's a noticeable jittering when circle strafing.
// I recommend turning on Physics Interpolations in ""physics/common"" for a smooth strafing feel.

public partial class {PlayerClassName} : CharacterBody3D
{{
	public const float SPEED = 5.0f;
	public const float JUMPVELOCITY = 4.5f;

	// Object References
	private CollisionShape3D StandingShape;
	private CollisionShape3D CrouchingShape;
	private RayCast3D CrouchRay;
	private Node3D Head;
	private Camera3D Camera;

	// Properties
	public Vector3 HeadPosition;
	public double HeadLeanAngleInRadians = 0.05;

	private Vector3 DesiredVelocity;

	private Vector3 HeadCrouchingPosition;
	private Vector3 HeadStandingPosition;

	// As the little blue robot said: You should probably replace UI actions with your own custom gameplay actions.
	private Vector2 InputDiretion => Input.GetVector(""ui_left"", ""ui_right"", ""ui_up"", ""ui_down"");

	public Vector3 RelativeDirection => Transform.Basis * new Vector3(InputDiretion.X, 0, InputDiretion.Y).Normalized();

	public bool IsHoldingJumping => Input.IsActionPressed(""ui_accept"");
	public bool IsHoldingCrouching => Input.IsActionPressed(""ui_accept""); // Change this if you want to implement crouching.

	public override void _Ready()
	{{
		// Setting up references.
		StandingShape = (CollisionShape3D)FindChild(""StandingCollisionShape"");
		CrouchingShape = (CollisionShape3D)FindChild(""CrouchingCollisionShape"");

		CrouchRay = (RayCast3D)FindChild(""CrouchRay"");

		Head = (Node3D)FindChild(""Head"");
		Camera = (Camera3D)FindChild(""PlayerCamera"");

		// Head Position calculations. You can override this to make your own. 
		HeadCrouchingPosition = new Vector3(0, CrouchingShape.Position.Y + (float)CrouchingShape.Shape.Get(""height"") / 2 - 0.3f, 0);
		HeadStandingPosition = new Vector3(0, StandingShape.Position.Y + (float)StandingShape.Shape.Get(""height"") / 2 - 0.3f, 0);

		HeadPosition = HeadStandingPosition;
	}}

	public override void _PhysicsProcess(double delta)
    {{
        HandleMovement(delta);
        HandleJump();
		//HandleCrouch();
		HandleCameraLerping(delta);

		ApplyVariableGravity(delta);

        MoveAndSlide();
    }}

    private void HandleMovement(double delta)
    {{
        if (RelativeDirection != Vector3.Zero)
        {{
            DesiredVelocity.X = Mathf.Lerp(DesiredVelocity.X, RelativeDirection.X * SPEED, (float)delta / 0.05f);
			DesiredVelocity.Z = Mathf.Lerp(DesiredVelocity.Z, RelativeDirection.Z * SPEED, (float)delta / 0.05f);
        }}
        else
        {{
            DesiredVelocity.X = Mathf.Lerp(Velocity.X, 0, (float)delta / 0.3f);
            DesiredVelocity.Z = Mathf.Lerp(Velocity.Z, 0, (float)delta / 0.3f);
        }}

        Velocity = DesiredVelocity;
    }}

	private void HandleJump()
	{{
		if (IsHoldingJumping && IsOnFloor())
		{{
			DesiredVelocity.Y = JUMPVELOCITY;
		}}
	}}

    private void HandleCrouch()
	{{
		if (IsHoldingCrouching && IsOnFloor())
		{{
			StandingShape.Disabled = true;
			CrouchingShape.Disabled = false;
			HeadPosition = HeadCrouchingPosition;
		}}
		else
		{{
			CrouchRay.ForceRaycastUpdate();
			if (!CrouchRay.IsColliding())
			{{
				StandingShape.Disabled = false;
				CrouchingShape.Disabled = true;
				HeadPosition = HeadStandingPosition;
			}}
		}}
	}}

	private void HandleCameraLerping(double delta)
	{{
		Head.Position = Head.Position.Lerp(HeadPosition, (float)delta / 0.05f);
		Camera.Rotation =Camera.Rotation.Lerp(new Vector3(0, 0, (float)HeadLeanAngleInRadians * InputDiretion.X), (float)delta/ 0.5f);
	}}

	private void ApplyVariableGravity(double delta)
	{{
		if (IsOnFloor()) return;

		if (Velocity.Y > 0)
		{{
			if (IsHoldingJumping == false || IsOnCeiling())
			{{
				// A very simple variable jump height implementation and ceiling fix.
				DesiredVelocity.Y = 0f;
			}}
			else
			{{
				DesiredVelocity += GetGravity() * (float)delta;
			}}
		}}
		else
		{{
			DesiredVelocity += GetGravity() * (float)delta * 2;
		}}
	}}
}}";

		HeadComponentScript = new CSharpScript()
		{
			SourceCode = SpringArmCode
		};

		PlayerScript = new CSharpScript()
		{
			SourceCode = PlayerCode
		};


		SaveResource(HeadComponentScript, $"{HeadComponentScriptDirectory}HeadComponent.cs");
		SaveResource(PlayerScript, $"{PlayerScriptDirectory}{PlayerClassName}.cs");
	}
}

// ContextButton helper class
public partial class ContextButton : HBoxContainer
{
	private Label _ContextLabel = new();
	private LineEdit _LineEdit = new();

	public string LabelText = "Undefined LabelText";
	public string DefaultValue = "Undefined DefaultValue";

	public event Action<string> TextChanged;
	public event Action<string> TextSubmitted;

	public override void _Ready()
	{
		_ContextLabel.Text = LabelText;

		_LineEdit.Text = DefaultValue;
		_LineEdit.ExpandToTextLength = true;
		SizeFlagsHorizontal = SizeFlags.ExpandFill;

		_LineEdit.TextChanged += (string newText) => TextChanged?.Invoke(newText);
		_LineEdit.TextSubmitted += (string newText) => TextSubmitted?.Invoke(newText);

		AddChild(_ContextLabel);
		AddChild(_LineEdit);
	}
}

