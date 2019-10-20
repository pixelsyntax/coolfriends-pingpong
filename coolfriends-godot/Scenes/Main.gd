extends Node

signal signal_change_level(levelName)

func _ready():
	print("create Main")
	connect("signal_change_level", self, "on_change_level")

#func _process(delta):
#	pass

func on_change_level(targetScene):
	# https://godotengine.org/qa/24773/how-to-load-and-change-scenes
	if !targetScene:
		print("No target scene specified!")
		return
	# Remove the current level
	var levelParent = get_node("/root/Initial Testscene/CUR_LEVEL") # TODO better path...
	for curLevel in levelParent.get_children():
		curLevel.queue_free()
	# Add the next level
	var next_level = targetScene.instance()
	add_child(next_level)