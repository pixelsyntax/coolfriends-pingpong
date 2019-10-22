extends Node

onready var levelParent = get_node("/root/Initial Testscene/CUR_LEVEL") # TODO better path...
	
signal signal_change_level(levelName)

func _ready():
	print("create Main")
	connect("signal_change_level", self, "on_change_level")

#func _process(delta):
#	pass

func on_change_level(targetSceneName):
	# https://godotengine.org/qa/24773/how-to-load-and-change-scenes
	var targetSceneResource = load(targetSceneName)
	if !targetSceneResource:
		print("No target scene specified!")
		return
	# Remove the current level
	for curLevel in levelParent.get_children():
		curLevel.queue_free()
	# Add the next level
	var targetScene = targetSceneResource.instance()
	levelParent.add_child(targetScene)