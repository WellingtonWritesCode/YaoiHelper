using System;
using System.Linq.Expressions;
using System.Reflection;
using MonoMod.ModInterop;

namespace Celeste.Mod.YaoiHelper.Interop;

// bit yucky but we do what we gotta do
[Bootstrap]
internal static class SpeedrunTool {
	public const int StateManagerWaitingState = 3;
	public static Func<int> GetStateManagerState;

	internal static void Init() {
		if (Everest.Loader.TryGetDependency(YaoiHelperModule.SRTModuleMetadata, out EverestModule srtModule)) {
			// TODO figure out if this depends on a version later than 3.16.1
			Assembly asm = srtModule.GetType().Assembly;
			Type smType = asm.GetType("Celeste.Mod.SpeedrunTool.SaveLoad.StateManager", throwOnError: true);
			PropertyInfo instanceProp = smType.GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
			PropertyInfo stateProp = smType.GetProperty("State", BindingFlags.Public | BindingFlags.Instance);

			// () => (int)StateManager.Instance.State
			MemberExpression instance = Expression.Property(null, instanceProp);
			MemberExpression state = Expression.Property(instance, stateProp);
			UnaryExpression cast = Expression.Convert(state, typeof(int));

			GetStateManagerState = Expression.Lambda<Func<int>>(cast).Compile();
		} else if (YaoiHelperModule.SRTLoaded) {
			throw new InvalidOperationException("SRT 3.16.1+ seems to be loaded but TryGetDependency failed...");
		}
	}
}

[Bootstrap]
internal static class SpeedrunToolSaveLoadImportsBootstrap {
	internal static void Init() {
		typeof(SpeedrunToolSaveLoadImports).ModInterop();
	}
}

#pragma warning disable CS0649 // field is never assigned to
[ModImportName("SpeedrunTool.SaveLoad")]
internal static class SpeedrunToolSaveLoadImports {
	public static Func<Type, string[], object> RegisterStaticTypes;
	public static Action<object> Unregister;
}
#pragma warning restore CS0649 // field is never assigned to
