extends KinematicBody

export var speedMoveMax = 9.0
#export var speedRot = 270.0
export var inputDistMin = 1.0
export var inputDistMax = 3.5

var mouseMove = false
var rotationVelocity = [ 0.0 ]

onready var Math = load("res://Scripts/Math.gd")

func _ready():
	pass

func _process(delta):
	var transform = get_global_transform()
	
	# mouse controls:
	var floorPlane = Plane(0, 1, 0, transform.origin.y)
	var mousePos = get_viewport().get_mouse_position()
	var camera = get_viewport().get_camera() # gets _3D_ camera, not 2D!
	var mousePos3D = floorPlane.intersects_ray(camera.project_ray_origin(mousePos), camera.project_ray_normal(mousePos))
	var dir = (mousePos3D - translation)
	var targetTransform = transform.looking_at(translation - dir, Vector3.UP)
	rotation.y = Math.smooth_damp_angle(rotation.y, targetTransform.basis.get_euler().y, rotationVelocity, 0.1, PI*2.0, delta)
	
	# final movement
	var speed = speedMoveMax * clamp(dir.length() / (inputDistMax - inputDistMin) - inputDistMin, 0.0, 1.0)
	var forward = transform.basis.z * speed if mouseMove else Vector3.ZERO
	var gravity = Vector3.DOWN * 5
	move_and_slide( forward + gravity, Vector3.UP, false, 4, PI/2 )

func _input(event):
	if event is InputEventMouseButton:
		var iemb = event as InputEventMouseButton
		if iemb.button_index == 1:
			mouseMove = iemb.is_pressed()