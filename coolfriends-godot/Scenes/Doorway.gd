extends CSGBox

# Declare member variables here. Examples:
# var a = 2
# var b = "text"

onready var hole = $Hole

# Called when the node enters the scene tree for the first time.
func _ready():
	closeDoor()

func openDoor():
	hole.visible = true
	
func closeDoor():
	hole.visible = false
	
func isOpen():
	return hole.visible

# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass


func _on_Button_button_activated():
	openDoor()


func _on_Button_button_deactivated():
	closeDoor()
