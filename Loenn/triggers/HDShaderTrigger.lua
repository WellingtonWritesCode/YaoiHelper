local HDShaderTrigger = {}

HDShaderTrigger.name = "YaoiHelper/HDShader"
HDShaderTrigger.placements = {
	name = "main",
	data = {
		effects = "",
		textures = "",
		target_register = "",
		flag = "",
		always_active=false,
		priority = 0
	}
}

HDShaderTrigger.fieldInformation = {
	priority = {
		fieldType = "integer"
	}
}

return HDShaderTrigger
