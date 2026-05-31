local buildRegion = {}

buildRegion.name = "YaoiHelper/BuildRegion"
buildRegion.placements = {
	name = "main",
	data = {
		width = 16,
		height = 16,
		prevent_building_when_inside = false
	}
}

buildRegion.color = {0, 1, 1, 0.2}

return buildRegion;
