extends CSGBox

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

#declare some custom button signals
signal button_activated
signal button_deactivated

# Called when the node enters the scene tree for the first time.
func _ready():
	$ActivationArea.connect( "body_entered", self, "on_body_entered" )
	$ActivationArea.connect( "body_exited", self, "on_body_exited" )

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func on_body_entered( body ):
	emit_signal( "button_activated" )
	
func on_body_exited( body ):
	emit_signal( "button_deactivated" )
