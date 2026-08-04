return {
	name = "YaoiHelper/HDShader",

	placements = {
		name = "main",
		data = {
			effects = "",
			textures = "",
			target_register = "",
			flag = "",
			always_active=false,
			priority = 0
		}
	},

	fieldOrder = {
		"x", "y",
		"width", "height",
		"effects", "textures",
		"target_register", "priority",
		"flag", "always_active"
	},

	fieldInformation = {
		priority = {
			fieldType = "integer"
		}
	}
}
