extends Spatial

#export(PackedScene) var targetScene
export(String, FILE, "*.tscn,*.scn") var targetSceneName setget set_file_path
export(String) var targetNodePath = "TeleporterTarget A"

#https://www.reddit.com/r/godot/comments/9rh5tt/exporting_a_scene_path/
func set_file_path(p_value):
	if typeof(p_value) == TYPE_STRING and p_value.get_extension() in ["tscn", "scn"]:
		var d = Directory.new()
		if not d.file_exists(p_value):
			return
		targetSceneName = p_value

func _ready():
	$TeleportArea.connect("body_entered", self, "on_body_entered")

#func _process(delta):
#	pass

func on_body_entered(body):
	#print("entering teleporter")
	$"/root/Main".emit_signal("signal_change_level", targetSceneName, targetNodePath)