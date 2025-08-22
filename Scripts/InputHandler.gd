extends Node

# Mouse presses
signal mouse_left
signal mouse_right

# Button presses
signal pressed_tab
signal pressed_shift
signal pressed_ctrl
signal pressed_esc

# Holding Properties
static var is_holding_tab: bool = false
static var is_holding_shift: bool = false
static var is_holding_ctrl: bool = false

func _input(event: InputEvent):
	# [Events]
	if event is InputEventKey:
		var key_event = event as InputEventKey

		match key_event.keycode:
			KEY_ESCAPE:
				# Pressed Escape
				if key_event.pressed: pressed_esc.emit()
			KEY_TAB:
				# Pressed Tab
				if key_event.pressed: pressed_tab.emit()

				# Holding Tab
				is_holding_tab = key_event.pressed
			KEY_SHIFT:
				# Pressed Tab
				if key_event.pressed: pressed_shift.emit()

				# Holding Tab
				is_holding_shift = key_event.pressed
			KEY_CTRL:
				# Pressed Ctrl
				if key_event.pressed: pressed_ctrl.emit()

				# Holdng Ctrl
				is_holding_ctrl = key_event.pressed
		
	# [Mouse]
	if event is InputEventMouseButton:
		var mouse_event = event as InputEventMouseButton
		
		if mouse_event.pressed:
			if mouse_event.button_index == MOUSE_BUTTON_LEFT:
				# Pressed LMB
				mouse_left.emit()
			
			if mouse_event.button_index == MOUSE_BUTTON_RIGHT:
				# Pressed RMB
				mouse_right.emit()