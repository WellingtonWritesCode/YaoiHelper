using System;
using System.Runtime.InteropServices;

namespace Celeste.Mod.YaoiHelper.Utils.Imports;

public static partial class SDLImports {
#pragma warning disable CA1401 // P/Invokes should not be visible
	[LibraryImport("SDL2")]
	public static partial void SDL_SetWindowPosition(IntPtr window, int x, int y);
	[LibraryImport("SDL2")]
	public static partial void SDL_MinimizeWindow(IntPtr window);
#pragma warning restore CA1401 // P/Invokes should not be visible
}
