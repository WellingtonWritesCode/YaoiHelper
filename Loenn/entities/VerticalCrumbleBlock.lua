local drawableSprite = require("structs.drawable_sprite")
local utils = require("utils")

local verticalCrumbleBlock = {}

verticalCrumbleBlock.name = "YaoiHelper/VerticalCrumbleBlock"
verticalCrumbleBlock.depth = 0
verticalCrumbleBlock.fieldInformation = {
    texture = {
        options = textures,
    }
}
verticalCrumbleBlock.placements = {
	name = "main",
	data = {
		width = 8,
		height = 8,
	}
}

function verticalCrumbleBlock.sprite(room, entity)
	local sprites = {}
    local x, y = entity.x or 0, entity.y or 0
    local width = math.max(entity.width or 0, 8)
	local height = math.max(entity.height or 0, 8)
	local scaleY = height/8
	local texture = "objects/crumbleBlock/default"

	for i = 0, math.ceil(width/32)-1 do
		local currWidth = math.min(width-32*i, 32)
		local spriteOptions = {
			justification = {0,0},
			x = x + 32*i,
			y = y,
			scaleY = scaleY,
			width = currWidth,
			height = height
		}
		local sprite = drawableSprite.fromTexture(texture, spriteOptions)
		sprite:useRelativeQuad(0,0,currWidth,8,true,false)
		table.insert(sprites, sprite)
	end

    return sprites
end

function verticalCrumbleBlock.selection(room, entity)
    return utils.rectangle(entity.x or 0, entity.y or 0, math.max(entity.width or 0, 8), math.max(entity.height or 0, 8))
end

return verticalCrumbleBlock
