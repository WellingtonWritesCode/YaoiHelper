local forceInputsTrigger = {}

forceInputsTrigger.name = "YaoiHelper/ForceInputsTrigger"
forceInputsTrigger.triggerText = "Force Inputs"
forceInputsTrigger.placements = {
	name = "main",
	data = {
		inputs = "",
		flag = "",
		flagInverted = false,
		showStatus = false,
		statusPrefix = "force inputs: ",
		hideStatusInCutscenes = false,
		extraStatusVerticalPadding = 0,
	}
}
forceInputsTrigger.fieldOrder = {
	"x", "y", "width", "height",
	"inputs",
	"flag, flagInverted",
	"showStatus", "hideStatusInCutscenes", "extraStatusVerticalPadding",
}

return forceInputsTrigger
