extends Node

# Setup
var do_mouse_locking: bool = true

# Global References
var player_reference
var scene_tree_reference: SceneTree

func _ready():
	scene_tree_reference = get_tree()

	if not do_mouse_locking:
		Input.mouse_mode = Input.MOUSE_MODE_CAPTURED

func _input(event: InputEvent):
	if do_mouse_locking:
		# Handles mouse behaviour on First-Person/Third-Person games.
		if event.is_action_released("ui_cancel"):
			Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
		elif event is InputEventMouse:
			if event.is_pressed() and event.button_index == MOUSE_BUTTON_LEFT:
				Input.mouse_mode = Input.MOUSE_MODE_CAPTURED