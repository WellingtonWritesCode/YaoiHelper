using System;
using System.Linq.Expressions;
using System.Reflection;
using MonoMod.ModInterop;

namespace Celeste.Mod.YaoiHelper.Interop;

// bit yucky but we do what we gotta do
[Bootstrap]
internal static class SpeedrunTool {
	public const int StateManagerWaitingState = 3;
	public static Func<int> GetStateManagerState {
		get => field ?? throw new InvalidOperationException("Init() not called yet");
		private set;
	}

	internal static void Init() {
		if (Everest.Loader.TryGetDependency(YaoiHelperModule.SRTModuleMetadata, out EverestModule srtModule)) {
			// TODO figure out if this depends on a version later than 3.16.1
			const string stateManagerTypeName = "Celeste.Mod.SpeedrunTool.SaveLoad.StateManager";
			Assembly asm = srtModule.GetType().Assembly;
			Type smType = asm.GetType(stateManagerTypeName, throwOnError: true)!;
			PropertyInfo instanceProp = smType.GetProperty("Instance", BindingFlags.Static | BindingFlags.Public) ??
				throw new MissingMemberException(stateManagerTypeName, "Instance");
			PropertyInfo stateProp = smType.GetProperty("State", BindingFlags.Instance | BindingFlags.Public) ??
				throw new MissingMemberException(stateManagerTypeName, "State");

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

internal static class SpeedrunToolSaveLoadImports {
#pragma warning disable CS0649 // field is never assigned to
	[ModImportName("SpeedrunTool.SaveLoad")]
	private static class Inner {
		public static Func<Type, string[], object>? RegisterStaticTypes;
		public static Action<object>? Unregister;
	}
#pragma warning restore CS0649 // field is never assigned to

	[Bootstrap]
	private static class Bootstrap {
		internal static void Init() {
			typeof(Inner).ModInterop();
		}
	}

	public static object RegisterStaticTypes(Type type, params string[] memberNames) {
		if (Inner.RegisterStaticTypes is null)
			throw new InvalidOperationException("bootstrap Init() not called yet");
		return Inner.RegisterStaticTypes(type, memberNames);
	}

	public static void Unregister(object obj) {
		if (Inner.Unregister is null)
			throw new InvalidOperationException("bootstrap Init() not called yet");
		Inner.Unregister(obj);
	}
}
