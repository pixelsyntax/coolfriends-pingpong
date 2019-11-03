extends Camera

export(NodePath) var cameraTargePath
export var lookAheadLength = 5

var cameraTarget
var offsetFromTarget

func _ready():
	cameraTarget = get_node(cameraTargePath)
	if cameraTarget:
		offsetFromTarget = cameraTarget.global_transform.origin - global_transform.origin
		print_debug( offsetFromTarget )
	else:
		print_debug( "no camera target defined!" )
	$"/root/Main".connect("signal_level_changed", self, "onLevelChanged")
	
func _process(delta):
	if !cameraTarget: #early exit
		return
	var targetPosition = cameraTarget.global_transform.origin + cameraTarget.global_transform.basis.z * lookAheadLength - offsetFromTarget
	global_transform.origin = lerp( global_transform.origin, targetPosition, 0.02 )

func onLevelChanged():
	var targetPosition = cameraTarget.global_transform.origin + cameraTarget.global_transform.basis.z * lookAheadLength - offsetFromTarget
	global_transform.origin = targetPosition