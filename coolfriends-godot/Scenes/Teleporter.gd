extends Spatial

export(PackedScene) var targetScene

func _ready():
	$TeleportArea.connect("body_entered", self, "on_body_entered")

#func _process(delta):
#	pass

func on_body_entered(body):
	#print("entering teleporter")
	$"/root/Main".emit_signal("signal_change_level", targetScene)