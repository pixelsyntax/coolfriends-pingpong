extends KinematicBody

# Declare member variables here.:
var speed = 8.0
var rotSpeed = 270.0

func _ready():
	pass # Replace with function body.

func _process(delta):
	var fwd = get_global_transform().basis.z;
	if Input.is_action_pressed("ui_right"):
		rotate_y(deg2rad(rotSpeed) * -delta)
	if Input.is_action_pressed("ui_left"):
		rotate_y(deg2rad(rotSpeed) * delta)
	if Input.is_action_pressed("ui_up"):
		move_and_collide(fwd * delta * speed, true)
	if Input.is_action_pressed("ui_down"):
		move_and_collide(fwd * delta * -speed, true)