extends Node

export(String, FILE, "*.tscn,*.scn") var messageScenePath setget set_file_path

onready var messageSceneResource = load("res://Scenes/GUI/Speechbubble.tscn")
onready var levelParent = get_node("/root/Initial Testscene/CUR_LEVEL") # TODO better path...

#https://www.reddit.com/r/godot/comments/9rh5tt/exporting_a_scene_path/
func set_file_path(p_value):
	if typeof(p_value) == TYPE_STRING and p_value.get_extension() in ["tscn", "scn"]:
		if Directory.new().file_exists(p_value): messageScenePath = p_value

signal signal_level_changed()

#func _ready():
#	print("create Main")

#func _process(delta):
#	pass

# Change the level (scene where the player walks around)
func changeLevel(targetSceneName, targetNodePath):
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
	# Has target node? Place player there
	var target = get_node(str(targetScene.get_path()) + "/" + targetNodePath)
	if target:
		$"/root/Initial Testscene/Player KinematicBody".translation = target.translation
	emit_signal("signal_level_changed")

# Show a short message
func createMessage(targetNode, text):
	#print("test: " + str(messageScenePath))
	#var messageSceneResource = load(messageScenePath)
	if !messageSceneResource:
		print("No message scene specified!")
		return
	var message = messageSceneResource.instance()
	targetNode.add_child(message)
	message.setText(text)
	#$SpeechbubbleDummy.translation, greeting)
	return message