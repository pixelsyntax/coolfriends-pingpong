extends Camera

export(NodePath) var cameraTargePath #Target object to follow
export var lookAheadLength = 5

var cameraTarget
var offsetFromTarget #Desired relative position of camera from target

# Called when the node enters the scene tree for the first time.
func _ready():
	cameraTarget = get_node(cameraTargePath)
	if cameraTarget:
		offsetFromTarget = cameraTarget.global_transform.origin - global_transform.origin
		print_debug( offsetFromTarget )
	else:
		print_debug( "no camera target defined!" )
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if !cameraTarget: #early exit
		return
	var targetPosition = cameraTarget.global_transform.origin + cameraTarget.global_transform.basis.z * lookAheadLength - offsetFromTarget
	global_transform.origin = lerp( global_transform.origin, targetPosition, 0.02 )
		
	
		