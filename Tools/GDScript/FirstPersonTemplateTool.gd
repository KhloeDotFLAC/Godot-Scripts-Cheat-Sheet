@tool
extends EditorScript
class_name FirstPersonTemplateTool

# Editable Variables
var player_name: String = "GeneratedPlayer"
var player_radius: float = 0.3
var player_height_in_meters: float = 1.8
var player_crouch_height_in_meters: float = 1.0
var player_class_name: String = "Player"
var player_scene_directory: String = "res://"
var player_script_directory: String = "res://"
var head_component_script_directory: String = "res://"

# Player Nodes
var player: CharacterBody3D
var standing_collision_shape: CollisionShape3D
var crouching_collision_shape: CollisionShape3D
var head: Node3D
var camera: Camera3D
var interaction_ray: RayCast3D
var crouch_ray: RayCast3D
var player_scene: PackedScene = PackedScene.new()

# Scripts Resource
var head_component_script: GDScript
var player_script: GDScript

var current_window: Window
var scene_path_preview_label: Label
var player_script_path_preview_label: Label
var head_component_script_path_preview_label: Label


func _run() -> void:
	create_window()


func create_window() -> void:
	current_window = Window.new()
	current_window.title = "Barebones Player Creator"
	current_window.min_size = Vector2i(455, 415)
	current_window.max_size = Vector2i(455, 415)
	current_window.position = DisplayServer.mouse_get_position()
	current_window.content_scale_mode = Window.CONTENT_SCALE_MODE_CANVAS_ITEMS
	current_window.content_scale_aspect = Window.CONTENT_SCALE_ASPECT_EXPAND
	current_window.close_requested.connect(func():close_window())

	create_window_gui()

	EditorInterface.popup_dialog(current_window)


func close_window() -> void:
	if current_window == null:
		return
	current_window.queue_free()


func create_window_gui() -> void:
	var base_gui := Control.new()
	base_gui.size = current_window.size
	
	var tab_window := TabContainer.new()
	tab_window.tab_alignment = TabBar.ALIGNMENT_CENTER
	tab_window.size = current_window.size
	
	var first_person_tab := TabBar.new()
	first_person_tab.name = "First Person or Third Person"
	
	var main_container := VBoxContainer.new()
	main_container.size = base_gui.size
	
	# Editable fields
	var player_scene_name_cb := ContextButton.new()
	player_scene_name_cb.label_text = "Object Name: "
	player_scene_name_cb.default_value = player_name
	
	var player_class_name_cb := ContextButton.new()
	player_class_name_cb.label_text = "Class Name:"
	player_class_name_cb.default_value = player_class_name
	
	var player_height_cb := ContextButton.new()
	player_height_cb.label_text = "Collision Height in cm: "
	player_height_cb.default_value = str(player_height_in_meters)
	
	var player_radius_cb := ContextButton.new()
	player_radius_cb.label_text = "Collision Radius in cm: "
	player_radius_cb.default_value = str(player_radius)
	
	var player_crouch_height_cb := ContextButton.new()
	player_crouch_height_cb.label_text = "Collision Crouch Height in cm: "
	player_crouch_height_cb.default_value = str(player_crouch_height_in_meters)
	
	var save_player_scene_cb := ContextButton.new()
	save_player_scene_cb.label_text = "Player Scene Path: "
	save_player_scene_cb.default_value = player_scene_directory
	
	var save_player_script_cb := ContextButton.new()
	save_player_script_cb.label_text = "Player Script Path: "
	save_player_script_cb.default_value = player_script_directory
	
	var save_head_script_cb := ContextButton.new()
	save_head_script_cb.label_text = "Head Component Path: "
	save_head_script_cb.default_value = head_component_script_directory
	
	scene_path_preview_label = Label.new()
	scene_path_preview_label.text = "Player Scene path: %s%s.tscn" % [player_scene_directory, player_name]
	
	player_script_path_preview_label = Label.new()
	player_script_path_preview_label.text = "Player Script path: %s%s.gd" % [player_script_directory, player_class_name]
	
	head_component_script_path_preview_label = Label.new()
	head_component_script_path_preview_label.text = "Head Component Script path: %sHeadComponent.gd" % head_component_script_directory
	
	var create_player_button := Button.new()
	create_player_button.text = "Create Player"
	
	# Creating Hierarchy
	base_gui.add_child(tab_window)
	tab_window.add_child(first_person_tab)
	first_person_tab.add_child(main_container)
	
	main_container.add_child(player_scene_name_cb)
	main_container.add_child(player_class_name_cb)
	main_container.add_child(player_height_cb)
	main_container.add_child(player_radius_cb)
	main_container.add_child(player_crouch_height_cb)
	main_container.add_child(save_player_scene_cb)
	main_container.add_child(save_player_script_cb)
	main_container.add_child(save_head_script_cb)
	
	main_container.add_child(scene_path_preview_label)
	main_container.add_child(player_script_path_preview_label)
	main_container.add_child(head_component_script_path_preview_label)
	
	main_container.add_child(create_player_button)
	
	# Signal connecting
	player_scene_name_cb.text_changed.connect(change_player_scene_name)
	player_class_name_cb.text_changed.connect(change_player_class_name)
	save_player_scene_cb.text_changed.connect(change_player_scene_folder_directory)
	save_player_script_cb.text_changed.connect(change_player_script_folder_directory)
	player_height_cb.text_changed.connect(change_player_height)
	player_radius_cb.text_changed.connect(change_player_radius)
	player_crouch_height_cb.text_changed.connect(change_player_crouch_height)
	create_player_button.button_down.connect(generate_player)
	
	current_window.add_child(base_gui)


func change_player_height(new_height: String) -> void:
	player_height_in_meters = new_height.to_float()


func change_player_radius(new_radius: String) -> void:
	player_radius = new_radius.to_float()


func change_player_crouch_height(new_crouch_height: String) -> void:
	player_crouch_height_in_meters = new_crouch_height.to_float()


func change_player_scene_name(new_name: String) -> void:
	player_name = new_name
	scene_path_preview_label.text = "Player Scene will be saved at: %s%s.tscn" % [player_scene_directory, player_name]


func change_player_class_name(new_name: String) -> void:
	player_class_name = new_name
	player_script_path_preview_label.text = "Player Script will be saved at: %s%s.gd" % [player_script_directory, player_class_name]


func change_player_scene_folder_directory(new_path: String) -> void:
	player_scene_directory = new_path
	scene_path_preview_label.text = "Player Scene will be saved at: %s%s.tscn" % [player_scene_directory, player_name]


func change_player_script_folder_directory(new_path: String) -> void:
	player_script_directory = new_path
	player_script_path_preview_label.text = "Player Script will be saved at: %s%s.gd" % [player_script_directory, player_class_name]


func generate_player() -> void:
	create_scripts()
	
	# Creating the collision shapes
	standing_collision_shape = CollisionShape3D.new()
	standing_collision_shape.name = "StandingCollisionShape"
	var standing_capsule := CapsuleShape3D.new()
	standing_capsule.height = player_height_in_meters
	standing_capsule.radius = player_radius
	standing_collision_shape.shape = standing_capsule
	standing_collision_shape.disabled = false
	
	crouching_collision_shape = CollisionShape3D.new()
	crouching_collision_shape.name = "CrouchingCollisionShape"
	var crouching_capsule := CapsuleShape3D.new()
	crouching_capsule.height = player_crouch_height_in_meters
	crouching_capsule.radius = player_radius
	crouching_collision_shape.shape = crouching_capsule
	crouching_collision_shape.position = Vector3(0, -player_height_in_meters / 2.0 + player_crouch_height_in_meters / 2.0, 0)
	crouching_collision_shape.disabled = true
	
	crouch_ray = RayCast3D.new()
	crouch_ray.name = "CrouchRay"
	crouch_ray.position = crouching_collision_shape.position
	crouch_ray.target_position = Vector3(0, player_height_in_meters - (player_crouch_height_in_meters / 2.0) - 0.5, 0)
	crouch_ray.enabled = false
	
	# Head
	head = SpringArm3D.new()
	head.name = "Head"
	head.spring_length = 0
	head.position = Vector3(0, (player_height_in_meters / 2.0) - 0.3, 0)
	
	interaction_ray = RayCast3D.new()
	interaction_ray.name = "InteractionRay"
	interaction_ray.target_position = Vector3(0, 0, -1.5)
	interaction_ray.enabled = false
	
	camera = Camera3D.new()
	camera.name = "PlayerCamera"
	
	head.add_child(camera)
	head.add_child(interaction_ray)
	
	# Player
	player = CharacterBody3D.new()
	player.name = player_name
	
	player.add_child(standing_collision_shape)
	player.add_child(crouching_collision_shape)
	player.add_child(head)
	player.add_child(crouch_ray)
	
	# Set owners
	camera.owner = player
	interaction_ray.owner = player
	crouch_ray.owner = player
	standing_collision_shape.owner = player
	crouching_collision_shape.owner = player
	head.owner = player
	
	head.set_script(load("%sHeadComponent.gd" % head_component_script_directory))
	player.set_script(load("%s%s.gd" % [player_script_directory, player_class_name]))
	
	print("[%s] Created Player with %d children." % [get_class(), player.get_child_count()])
	
	# Create Scene
	var try_pack := player_scene.pack(player)
	if try_pack != OK:
		printerr("[%s] Player packing failed. Error: %s" % [get_class(), try_pack])
	
	save_resource(player_scene, "%s%s.tscn" % [player_scene_directory, player_name])
	close_window()


func save_resource(what: Resource, path: String) -> void:
	var try_save := ResourceSaver.save(what, path)
	if try_save != OK:
		printerr("[%s][Save Resource] Resource save failed. Error: %s" % [get_class(), try_save])
		return
	print("[%s][Save Resource] Saved \"%s\" resource to \"%s\"" % [get_class(), what.get_class(), path])


func create_scripts() -> void:
	var head_component_code := """extends Node3D

# Setup
@export var rotate_parent: bool = true

# References
var current_parent: Node3D

# Camera
@export var mouse_sensitivity: float = 0.005
@export var max_vertical_angle_in_radians: float = 1.5


func _ready() -> void:
	if get_parent() != null:
		current_parent = get_parent() as Node3D
	else:
		printerr("[%s] Has no valid Parent Node3D." % name)


func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion:
		var mouse_motion := event as InputEventMouseMotion
		if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
			printerr("[%s] Mouse is not captured!" % name)
			return
		
		# Horizontal rotation
		if rotate_parent and current_parent != null:
			await get_tree().physics_frame
			current_parent.rotate_y(-mouse_motion.relative.x * mouse_sensitivity)
		else:
			await get_tree().physics_frame
			rotate_y(-mouse_motion.relative.x * mouse_sensitivity)
		
		# Vertical rotation (on self)
		var desired_x_angle := -mouse_motion.relative.y * mouse_sensitivity
		var new_x_rotation := rotation.x + desired_x_angle
		
		# Clamp rotation
		new_x_rotation = clamp(new_x_rotation, -max_vertical_angle_in_radians, max_vertical_angle_in_radians)
		rotation = Vector3(new_x_rotation, rotation.y, rotation.z)
"""
	
	var player_code := """extends CharacterBody3D

# Currently, with this setup, there's a noticeable jittering when circle strafing.
# I recommend turning on Physics Interpolations in "physics/common" for a smooth strafing feel.

const SPEED: float = 5.0
const JUMP_VELOCITY: float = 4.5

# Object References
var standing_shape: CollisionShape3D
var crouching_shape: CollisionShape3D
var crouch_ray: RayCast3D
var head: Node3D
var camera: Camera3D

# Properties
var head_position: Vector3
var head_lean_angle_in_radians: float = 0.05
var desired_velocity: Vector3
var head_crouching_position: Vector3
var head_standing_position: Vector3

var input_direction: Vector2:
	get:
		return Input.get_vector("ui_left", "ui_right", "ui_up", "ui_down")

var relative_direction: Vector3:
	get:
		return transform.basis * Vector3(input_direction.x, 0, input_direction.y).normalized()

var is_holding_jumping: bool:
	get:
		return Input.is_action_pressed("ui_accept")

var is_holding_crouching: bool:
	get:
		return Input.is_action_pressed("ui_accept")  # Change this if you want to implement crouching


func _ready() -> void:
	# Setting up references
	standing_shape = find_child("StandingCollisionShape") as CollisionShape3D
	crouching_shape = find_child("CrouchingCollisionShape") as CollisionShape3D
	crouch_ray = find_child("CrouchRay") as RayCast3D
	head = find_child("Head") as Node3D
	camera = find_child("PlayerCamera") as Camera3D
	
	# Head Position calculations
	head_crouching_position = Vector3(0, crouching_shape.position.y + crouching_shape.shape.get("height") / 2.0 - 0.3, 0)
	head_standing_position = Vector3(0, standing_shape.position.y + standing_shape.shape.get("height") / 2.0 - 0.3, 0)
	
	head_position = head_standing_position


func _physics_process(delta: float) -> void:
	handle_movement(delta)
	handle_jump()
	#handle_crouch()
	handle_camera_lerping(delta)
	apply_variable_gravity(delta)
	move_and_slide()


func handle_movement(delta: float) -> void:
	if relative_direction != Vector3.ZERO:
		desired_velocity.x = lerpf(desired_velocity.x, relative_direction.x * SPEED, delta / 0.05)
		desired_velocity.z = lerpf(desired_velocity.z, relative_direction.z * SPEED, delta / 0.05)
	else:
		desired_velocity.x = lerpf(velocity.x, 0.0, delta / 0.3)
		desired_velocity.z = lerpf(velocity.z, 0.0, delta / 0.3)
	
	velocity = desired_velocity


func handle_jump() -> void:
	if is_holding_jumping and is_on_floor():
		desired_velocity.y = JUMP_VELOCITY


func handle_crouch() -> void:
	if is_holding_crouching and is_on_floor():
		standing_shape.disabled = true
		crouching_shape.disabled = false
		head_position = head_crouching_position
	else:
		crouch_ray.force_raycast_update()
		if not crouch_ray.is_colliding():
			standing_shape.disabled = false
			crouching_shape.disabled = true
			head_position = head_standing_position


func handle_camera_lerping(delta: float) -> void:
	head.position = head.position.lerp(head_position, delta / 0.05)
	camera.rotation = camera.rotation.lerp(Vector3(0, 0, head_lean_angle_in_radians * input_direction.x), delta / 0.5)


func apply_variable_gravity(delta: float) -> void:
	if is_on_floor():
		return
	
	if velocity.y > 0:
		if not is_holding_jumping or is_on_ceiling():
			desired_velocity.y = 0.0
		else:
			desired_velocity += get_gravity() * delta
	else:
		desired_velocity += get_gravity() * delta * 2.0
""".replace("CLASSNAME", player_class_name)
	
	head_component_script = GDScript.new()
	head_component_script.source_code = head_component_code
	
	player_script = GDScript.new()
	player_script.source_code = player_code
	
	save_resource(head_component_script, "%sHeadComponent.gd" % head_component_script_directory)
	save_resource(player_script, "%s%s.gd" % [player_script_directory, player_class_name])


# ContextButton helper class
class ContextButton extends HBoxContainer:
	var _context_label := Label.new()
	var _line_edit := LineEdit.new()
	
	var label_text: String = "Undefined LabelText"
	var default_value: String = "Undefined DefaultValue"
	
	signal text_changed(new_text: String)
	signal text_submitted(new_text: String)
	
	func _ready() -> void:
		_context_label.text = label_text
		_line_edit.text = default_value
		_line_edit.expand_to_text_length = true
		size_flags_horizontal = Control.SIZE_EXPAND_FILL
		
		_line_edit.text_changed.connect(func(new_text: String): text_changed.emit(new_text))
		_line_edit.text_submitted.connect(func(new_text: String): text_submitted.emit(new_text))
		
		add_child(_context_label)
		add_child(_line_edit)