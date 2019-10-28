extends PathFollow

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

export var segmentSpeed : float
export var segmentOffset : float


# Called when the node enters the scene tree for the first time.
func _ready():
	unit_offset = segmentOffset

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	unit_offset += segmentSpeed * delta
