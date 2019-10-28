extends Area

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

onready var animationPlayerToActivate = $"../AnimationPlayer"

# Called when the node enters the scene tree for the first time.
func _ready():
	connect( "body_entered", self, "on_body_entered" )

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	rotate_y( PI * delta )

func on_body_entered( body ):
	animationPlayerToActivate.play("ElevatorMotion")
	queue_free()