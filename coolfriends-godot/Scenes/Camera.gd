extends Camera

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

var cameraTarget #Target object to follow
var offsetFromTarget #Desired relative position of camera from target
var lookAheadLength = 5

# Called when the node enters the scene tree for the first time.
func _ready():
	cameraTarget = $"../Player KinematicBody" #Can we assign this via the editor instead of hard coding a path?
	if cameraTarget:
		offsetFromTarget = cameraTarget.global_transform.origin - global_transform.origin
		print_debug( offsetFromTarget )
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if !cameraTarget: #early exit
		return
	var targetPosition = cameraTarget.global_transform.origin + cameraTarget.global_transform.basis.z * lookAheadLength - offsetFromTarget
	global_transform.origin = lerp( global_transform.origin, targetPosition, 0.02 )
		
	
		