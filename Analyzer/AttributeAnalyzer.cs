using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Celeste.Mod.YaoiHelper.Analyzer;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AttributeAnalyzer : DiagnosticAnalyzer {
#pragma warning disable RS2008 // enable analyzer release tracking
	public static readonly DiagnosticDescriptor MissingInitMethod = new(
		id: "YAOI0001",
		title: "Missing Init method in [Bootstrap]",
		messageFormat: "[Bootstrap] class '{0}' must declare a static non-generic Init() that takes in no parameters",
		category: "Attribute",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public static readonly DiagnosticDescriptor MissingHooksMethod = new(
		id: "YAOI0002",
		title: "Missing required method in [Submodule]",
		messageFormat: "[Submodule] class '{0}' must declare a static non-generic method '{1}' that takes in no parameters",
		category: "Attribute",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);

	public static readonly DiagnosticDescriptor MissingSRTMethod = new(
		id: "YAOI0003",
		title: "Missing required SRT support method in [Submodule(HasSRTSupport = true)]",
		messageFormat: "[Submodule] class '{0}' with HasSRTSupport = true must declare a static non-generic method '{1}' that takes in no parameters",
		category: "Attribute",
		defaultSeverity: DiagnosticSeverity.Error,
		isEnabledByDefault: true
	);
#pragma warning restore RS2008 // enable analyzer release tracking

	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(
		MissingInitMethod,
		MissingHooksMethod,
		MissingSRTMethod
	);

	public override void Initialize(AnalysisContext context) {
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.EnableConcurrentExecution();
		context.RegisterSymbolAction(analyzeNamedType, SymbolKind.NamedType);
	}

	private static void analyzeNamedType(SymbolAnalysisContext ctx) {
		INamedTypeSymbol type = (INamedTypeSymbol)ctx.Symbol;
		if (type.TypeKind != TypeKind.Class)
			return;
		AttributeData? bootstrapAttr = getAttr(type, "Celeste.Mod.YaoiHelper.BootstrapAttribute");
		if (bootstrapAttr is not null)
			checkRequiredMethod(ctx, type, "Init", MissingInitMethod);
		AttributeData? submoduleAttr = getAttr(type, "Celeste.Mod.YaoiHelper.SubmoduleAttribute");
		if (submoduleAttr is not null) {
			checkRequiredMethod(ctx, type, "ApplyHooks", MissingHooksMethod);
			checkRequiredMethod(ctx, type, "RemoveHooks", MissingHooksMethod);
			if (hasSrt(submoduleAttr)) {
				checkRequiredMethod(ctx, type, "RegisterSRTSupport", MissingSRTMethod);
				checkRequiredMethod(ctx, type, "UnregisterSRTSupport", MissingSRTMethod);
			}
		}
	}

	private static AttributeData? getAttr(INamedTypeSymbol type, string fullName) {
		foreach (AttributeData attr in type.GetAttributes()) {
			INamedTypeSymbol? attrType = attr.AttributeClass;
			if (attrType is null)
				continue;
			if (attrType.ToDisplayString() == fullName)
				return attr;
		}
		return null;
	}

	private static bool hasSrt(AttributeData moduleAttr) {
		foreach (KeyValuePair<string, TypedConstant> kvp in moduleAttr.NamedArguments)
			if (kvp.Key == "HasSRTSupport" && kvp.Value.Value is bool b)
				return b;
		return false;
	}

	private static void checkRequiredMethod(SymbolAnalysisContext ctx, INamedTypeSymbol type, string methodName, DiagnosticDescriptor dd) {
		foreach (ISymbol member in type.GetMembers(methodName)) {
			if (member is not IMethodSymbol method)
				continue;
			if (method.MethodKind != MethodKind.Ordinary)
				continue;
			if (!method.IsStatic)
				continue;
			if (!method.TypeParameters.IsEmpty)
				continue;
			if (!method.Parameters.IsEmpty)
				continue;
			return;
		}
		Location loc = type.Locations.Length != 0 ? type.Locations[0] : Location.None;
		ctx.ReportDiagnostic(Diagnostic.Create(dd, loc, type.Name, methodName));
	}
}
