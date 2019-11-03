extends KinematicBody

export var greeting = "Hallo Friend!"

var isGreeting = false

func _ready():
	$DetectArea.connect("body_entered", self, "on_body_entered")

#func _process(delta):
#	pass

func on_body_entered(body):
	if body.is_in_group("Players") and !isGreeting:
		$"/root/Main".createMessage($SpeechbubbleDummy, greeting)
		isGreeting = true