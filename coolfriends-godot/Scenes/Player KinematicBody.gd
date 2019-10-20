extends KinematicBody

export var speedMove = 8.0
export var speedRot = 270.0

var mouseMove = false

func _ready():
	pass

func _process(delta):
	var transform = get_global_transform()
	
	# mouse controls:
	var floorPlane = Plane(0, 1, 0, transform.origin.y)
	var mousePos = get_viewport().get_mouse_position()
	var camera = get_viewport().get_camera() # gets _3D_ camera, not 2D!
	var mousePos3D = floorPlane.intersects_ray(camera.project_ray_origin(mousePos), camera.project_ray_normal(mousePos))
	var targetTransform = transform.looking_at(translation - (mousePos3D - translation), Vector3.UP)
	targetTransform = transform.interpolate_with(targetTransform, 0.1) # TODO make it more frame-independet
	rotation = targetTransform.basis.get_euler()
	
	var forward = transform.basis.z * speedMove if mouseMove else Vector3.ZERO
	var gravity = Vector3.DOWN * 5
	move_and_slide( forward + gravity, Vector3.UP, false, 4, PI/2 )

func _input(event):
	if event is InputEventMouseButton:
		var iemb = event as InputEventMouseButton
		if iemb.button_index == 1:
			mouseMove = iemb.is_pressed()