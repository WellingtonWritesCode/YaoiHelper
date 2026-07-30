return {
	name = "YaoiHelper/ForceInputsTrigger",
	triggerText = "Force Inputs",
	placements = {
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
	},
	fieldOrder = {
		"x", "y", "width", "height",
		"inputs",
		"flag, flagInverted",
		"showStatus", "hideStatusInCutscenes", "extraStatusVerticalPadding",
	}
}
