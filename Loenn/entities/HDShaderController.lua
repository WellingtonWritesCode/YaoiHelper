local HDShaderController = {}

HDShaderController.name = "YaoiHelper/HDShaderController"
HDShaderController.texture = "LoennSprites/Entities/hd_shader_controller"
HDShaderController.depth = 8998
HDShaderController.justification = {0.5, 0.5}
HDShaderController.placements = {
    name = "main",
	data = {
		render_player_over = false,
		render_level_over = false,
	}
}

return HDShaderController
