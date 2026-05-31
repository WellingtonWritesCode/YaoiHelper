local showStatusTrigger = {}

showStatusTrigger.name = "YaoiHelper/ShowStatusTrigger"
showStatusTrigger.triggerText = "Show Status"
showStatusTrigger.placements = {
	name = "main",
	data = {
		text = "",
		hideInCutscenes = false,
		extraVerticalPadding = 0,
		instantRender = false,
	},
}

return showStatusTrigger
