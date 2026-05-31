return {
	name = "YaoiHelper/BetterTimerTrigger",
	triggerText = "Better Timer",
	placements = {
		name = "main",
		data = {
			time = 0.0,
			frames = 0,
			flagToSet = "flag_timer",
			unsetFlag = false,
			controlFlag = "",
			controlFlagInverted = false,
			mode = 0,
			resetTimerOnLeave = true,
			resetTimerOnControlFlagUnset = false,
			unsetOnRoomLoad = true,
			compareLOnly = false,
		},
	},
	fieldOrder = {
		"x", "y", "width", "height",
		"time", "frames", "flagToSet", "unsetFlag", "controlFlag", "controlFlagInverted", "mode",
		"resetTimerOnLeave", "resetTimerOnControlFlagUnset", "unsetOnRoomLoad", "compareLOnly",
	},
	fieldInformation = {
		frames = {
			fieldType = "integer",
		},
		mode = {
			fieldType = "integer",
			options = {
				{"DeltaTime", 0},
				{"RawDeltaTime", 1},
				{"Frame count", 2},
			},
		},
	},
}
