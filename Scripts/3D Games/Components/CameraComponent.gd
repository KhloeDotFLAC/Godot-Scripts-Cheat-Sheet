extends Node3D
class_name CameraComponent

# Setup
@export var rotate_parent: bool = true

# References
var current_parent: Node3D

# Camera
@export var mouse_sensitivity: float = 0.005

@export var max_vertical_angle_in_radians: float = 1.5

func _ready():
	if get_parent() != null:
		current_parent = get_parent() as Node3D
	else:
		printerr("[%s] Has no valid Parent Node3D." % name)

func _input(event: InputEvent):
	if event is InputEventMouseMotion:
		var mouse_motion = event as InputEventMouseMotion
		
		if Input.mouse_mode != Input.MOUSE_MODE_CAPTURED:
			printerr("[%s] Mouse is not captured!" % name)
			return
		
		# Horizontal rotation
		if rotate_parent and current_parent != null:
			current_parent.rotate_y(-mouse_motion.relative.x * mouse_sensitivity)
		else:
			rotate_y(-mouse_motion.relative.x * mouse_sensitivity)
		
		# Vertical rotation (on self)
		var desired_x_angle = -mouse_motion.relative.y * mouse_sensitivity
		var new_x_rotation = rotation.x + desired_x_angle
		
		# Clamp rotation
		new_x_rotation = clamp(new_x_rotation, -max_vertical_angle_in_radians, max_vertical_angle_in_radians)
		rotation = Vector3(new_x_rotation, rotation.y, rotation.z)