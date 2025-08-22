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
		
		if key_event.pressed and key_event.keycode == KEY_ESCAPE:
			# Pressed Escape
			pressed_esc.emit()
		
		if key_event.pressed and key_event.keycode == KEY_TAB:
			# Pressed Tab
			pressed_tab.emit()
		
		if key_event.pressed and key_event.keycode == KEY_SHIFT:
			# Pressed Shift
			pressed_shift.emit()
		
		if key_event.pressed and key_event.keycode == KEY_CTRL:
			# Pressed Ctrl
			pressed_ctrl.emit()
	
	# [Holds]
	if event is InputEventKey:
		var cur_key = event as InputEventKey
		
		if cur_key.pressed and cur_key.keycode == KEY_TAB:
			# Holding Tab key
			is_holding_tab = true
		elif not cur_key.pressed and cur_key.keycode == KEY_TAB:
			# Released Tab key
			is_holding_tab = false
		
		if cur_key.pressed and cur_key.keycode == KEY_SHIFT:
			# Holding Shift key
			is_holding_shift = true
		elif not cur_key.pressed and cur_key.keycode == KEY_SHIFT:
			# Released Shift key
			is_holding_shift = false
		
		if cur_key.pressed and cur_key.keycode == KEY_CTRL:
			# Holding Ctrl key
			is_holding_ctrl = true
		elif not cur_key.pressed and cur_key.keycode == KEY_CTRL:
			# Released Ctrl key
			is_holding_ctrl = false
	
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