using System.Collections.Immutable;
using System.Globalization;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace newtype.generator;

/// <summary>
/// Extracts all symbol data into an <see cref="AliasModel"/> during the transform step,
/// before the incremental pipeline caches the result. After extraction, no Roslyn symbols are retained.
/// </summary>
internal static class AliasModelExtractor
{
    public static AliasModel? Extract(GeneratorAttributeSyntaxContext context, ITypeSymbol aliasedType)
    {
        var typeDecl = (TypeDeclarationSyntax)context.TargetNode;
        var typeSymbol = (INamedTypeSymbol)context.TargetSymbol;

        var typeName = typeSymbol.Name;
        var ns = typeSymbol.ContainingNamespace;
        var namespaceName = ns is {IsGlobalNamespace: false} ? ns.ToDisplayString() : "";

        var isReadonly = typeDecl.Modifiers.Any(SyntaxKind.ReadOnlyKeyword);
        var isClass = typeDecl is ClassDeclarationSyntax
                      || (typeDecl is RecordDeclarationSyntax rds
                          && !rds.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword));
        var isRecord = typeDecl is RecordDeclarationSyntax;
        var isRecordStruct = isRecord && !isClass;

        var aliasedTypeFullName = aliasedType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        var aliasedTypeMinimalName = aliasedType.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);

        var implementsIComparable = ImplementsInterface(aliasedType, "System.IComparable`1");
        var implementsIFormattable = ImplementsInterface(aliasedType, "System.IFormattable");

        var binaryOperators = ExtractBinaryOperators(aliasedType);
        var unaryOperators = ExtractUnaryOperators(aliasedType);
        var hasNativeEquality = HasNativeEqualityOperator(aliasedType, binaryOperators);

        var staticMembers = ExtractStaticMembers(aliasedType);
        var hasStaticMemberCandidates = HasStaticMemberCandidates(aliasedType);
        var instanceFields = ExtractInstanceFields(aliasedType);
        var instanceProperties = ExtractInstanceProperties(aliasedType);
        var instanceMethods = ExtractInstanceMethods(aliasedType);
        var constructors = ExtractForwardableConstructors(typeSymbol, aliasedType);

        var typeDisplayString = typeSymbol.ToDisplayString();

        return new AliasModel(
            TypeName: typeName,
            Namespace: namespaceName,
            DeclaredAccessibility: typeSymbol.DeclaredAccessibility,
            IsReadonly: isReadonly,
            IsClass: isClass,
            IsRecord: isRecord,
            IsRecordStruct: isRecordStruct,
            AliasedTypeFullName: aliasedTypeFullName,
            AliasedTypeMinimalName: aliasedTypeMinimalName,
            AliasedTypeSpecialType: aliasedType.SpecialType,
            AliasedTypeIsValueType: aliasedType.IsValueType,
            ImplementsIComparable: implementsIComparable,
            ImplementsIFormattable: implementsIFormattable,
            HasNativeEqualityOperator: hasNativeEquality,
            TypeDisplayString: typeDisplayString,
            HasStaticMemberCandidates: hasStaticMemberCandidates,
            BinaryOperators: binaryOperators,
            UnaryOperators: unaryOperators,
            StaticMembers: staticMembers,
            InstanceFields: instanceFields,
            InstanceProperties: instanceProperties,
            InstanceMethods: instanceMethods,
            ForwardedConstructors: constructors
        );
    }

    private static EquatableArray<BinaryOperatorInfo> ExtractBinaryOperators(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<BinaryOperatorInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is IMethodSymbol {MethodKind: MethodKind.UserDefinedOperator, Parameters.Length: 2} method)
            {
                builder.Add(new BinaryOperatorInfo(
                    Name: method.Name,
                    LeftTypeFullName: method.Parameters[0].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    RightTypeFullName: method.Parameters[1].Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ReturnTypeFullName: method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    LeftIsAliasedType: SymbolEqualityComparer.Default.Equals(method.Parameters[0].Type, aliasedType),
                    RightIsAliasedType: SymbolEqualityComparer.Default.Equals(method.Parameters[1].Type, aliasedType),
                    ReturnIsAliasedType: SymbolEqualityComparer.Default.Equals(method.ReturnType, aliasedType)
                ));
            }
        }

        return new EquatableArray<BinaryOperatorInfo>(builder.ToImmutable());
    }

    private static EquatableArray<UnaryOperatorInfo> ExtractUnaryOperators(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<UnaryOperatorInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is IMethodSymbol {MethodKind: MethodKind.UserDefinedOperator, Parameters.Length: 1} method)
            {
                builder.Add(new UnaryOperatorInfo(
                    Name: method.Name,
                    ReturnTypeFullName: method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    ReturnIsAliasedType: SymbolEqualityComparer.Default.Equals(method.ReturnType, aliasedType)
                ));
            }
        }

        return new EquatableArray<UnaryOperatorInfo>(builder.ToImmutable());
    }

    /// <summary>
    /// Matches the old code's filter: any public static non-operator member that is IPropertySymbol or IFieldSymbol.
    /// This includes const fields which don't survive the readonly filter but cause the empty region to be emitted.
    /// </summary>
    private static bool HasStaticMemberCandidates(ITypeSymbol aliasedType)
    {
        foreach (var member in aliasedType.GetMembers())
        {
            if (!member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (member.Name.StartsWith("op_"))
                continue;
            if (member is IPropertySymbol or IFieldSymbol)
                return true;
        }
        return false;
    }

    private static EquatableArray<StaticMemberInfo> ExtractStaticMembers(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<StaticMemberInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (!member.IsStatic || member.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (member.Name.StartsWith("op_"))
                continue;

            if (member is IPropertySymbol {GetMethod: not null} prop)
            {
                builder.Add(new StaticMemberInfo(
                    Name: prop.Name,
                    TypeFullName: prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    TypeIsAliasedType: SymbolEqualityComparer.Default.Equals(prop.Type, aliasedType),
                    IsProperty: true,
                    IsReadonlyField: false
                ));
            }
            else if (member is IFieldSymbol {IsReadOnly: true} field)
            {
                builder.Add(new StaticMemberInfo(
                    Name: field.Name,
                    TypeFullName: field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    TypeIsAliasedType: SymbolEqualityComparer.Default.Equals(field.Type, aliasedType),
                    IsProperty: false,
                    IsReadonlyField: true
                ));
            }
        }

        return new EquatableArray<StaticMemberInfo>(builder.ToImmutable());
    }

    private static EquatableArray<InstanceFieldInfo> ExtractInstanceFields(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<InstanceFieldInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is IFieldSymbol {IsStatic: false, IsImplicitlyDeclared: false, DeclaredAccessibility: Accessibility.Public} field)
            {
                builder.Add(new InstanceFieldInfo(
                    Name: field.Name,
                    TypeFullName: field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    TypeIsAliasedType: SymbolEqualityComparer.Default.Equals(field.Type, aliasedType)
                ));
            }
        }

        return new EquatableArray<InstanceFieldInfo>(builder.ToImmutable());
    }

    private static EquatableArray<InstancePropertyInfo> ExtractInstanceProperties(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<InstancePropertyInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is IPropertySymbol {IsStatic: false, DeclaredAccessibility: Accessibility.Public, IsIndexer: false, GetMethod: not null} prop)
            {
                builder.Add(new InstancePropertyInfo(
                    Name: prop.Name,
                    TypeFullName: prop.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    TypeIsAliasedType: SymbolEqualityComparer.Default.Equals(prop.Type, aliasedType),
                    HasGetter: prop.GetMethod != null
                ));
            }
        }

        return new EquatableArray<InstancePropertyInfo>(builder.ToImmutable());
    }

    private static EquatableArray<InstanceMethodInfo> ExtractInstanceMethods(ITypeSymbol aliasedType)
    {
        var builder = ImmutableArray.CreateBuilder<InstanceMethodInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is not IMethodSymbol method)
                continue;
            if (method.IsStatic || method.DeclaredAccessibility != Accessibility.Public)
                continue;
            if (method.MethodKind != MethodKind.Ordinary)
                continue;
            if (!method.CanBeReferencedByName)
                continue;
            if (method.Name.StartsWith("get_") || method.Name.StartsWith("set_"))
                continue;
            if (method.Name is "GetHashCode" or "Equals" or "ToString" or "CompareTo")
                continue;

            var skipReturnWrapping = aliasedType.IsValueType &&
                                     aliasedType.SpecialType != SpecialType.None;

            var paramBuilder = ImmutableArray.CreateBuilder<MethodParameterInfo>();
            foreach (var p in method.Parameters)
            {
                paramBuilder.Add(new MethodParameterInfo(
                    Name: p.Name,
                    TypeFullName: p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    RefKind: p.RefKind,
                    IsAliasedType: SymbolEqualityComparer.Default.Equals(p.Type, aliasedType)
                ));
            }

            builder.Add(new InstanceMethodInfo(
                Name: method.Name,
                ReturnTypeFullName: method.ReturnType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                ReturnsVoid: method.ReturnsVoid,
                ReturnIsAliasedType: SymbolEqualityComparer.Default.Equals(method.ReturnType, aliasedType),
                SkipReturnWrapping: skipReturnWrapping,
                Parameters: new EquatableArray<MethodParameterInfo>(paramBuilder.ToImmutable())
            ));
        }

        return new EquatableArray<InstanceMethodInfo>(builder.ToImmutable());
    }

    private static EquatableArray<ConstructorInfo> ExtractForwardableConstructors(
        INamedTypeSymbol typeSymbol, ITypeSymbol aliasedType)
    {
        var userSignatures = GetUserDefinedConstructorSignatures(typeSymbol);
        var builder = ImmutableArray.CreateBuilder<ConstructorInfo>();

        foreach (var member in aliasedType.GetMembers())
        {
            if (member is not IMethodSymbol m)
                continue;
            if (m.MethodKind != MethodKind.Constructor || m.IsStatic)
                continue;
            if (m.DeclaredAccessibility != Accessibility.Public || m.IsImplicitlyDeclared)
                continue;
            if (m.Parameters.Length == 0)
                continue;

            // Skip copy constructor (single param of the aliased type itself)
            if (m.Parameters.Length == 1 &&
                SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, aliasedType))
                continue;

            // Skip constructors with pointer parameters
            var hasPointer = false;
            foreach (var p in m.Parameters)
            {
                if (p.Type.TypeKind == TypeKind.Pointer)
                {
                    hasPointer = true;
                    break;
                }
            }
            if (hasPointer) continue;

            // Skip if user already defined a constructor with the same signature
            var sig = GetConstructorSignature(m);
            if (userSignatures.Contains(sig))
                continue;

            var paramBuilder = ImmutableArray.CreateBuilder<ConstructorParameterInfo>();
            foreach (var p in m.Parameters)
            {
                paramBuilder.Add(new ConstructorParameterInfo(
                    Name: p.Name,
                    TypeFullName: p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat),
                    RefKind: p.RefKind,
                    IsParams: p.IsParams,
                    DefaultValueLiteral: p.HasExplicitDefaultValue ? FormatDefaultValue(p) : null
                ));
            }

            builder.Add(new ConstructorInfo(
                Parameters: new EquatableArray<ConstructorParameterInfo>(paramBuilder.ToImmutable())
            ));
        }

        return new EquatableArray<ConstructorInfo>(builder.ToImmutable());
    }

    private static HashSet<string> GetUserDefinedConstructorSignatures(INamedTypeSymbol typeSymbol)
    {
        var signatures = new HashSet<string>();
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IMethodSymbol {MethodKind: MethodKind.Constructor, IsImplicitlyDeclared: false} ctor)
            {
                signatures.Add(GetConstructorSignature(ctor));
            }
        }
        return signatures;
    }

    private static string GetConstructorSignature(IMethodSymbol ctor)
    {
        return string.Join(",", ctor.Parameters.Select(p =>
        {
            var refModifier = p.RefKind switch
            {
                RefKind.Ref => "ref ",
                RefKind.Out => "out ",
                RefKind.In => "in ",
                _ => ""
            };
            return refModifier + p.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        }));
    }

    private static string FormatDefaultValue(IParameterSymbol param)
    {
        var value = param.ExplicitDefaultValue;

        if (value is null)
            return "default";

        if (value is string s)
            return SymbolDisplay.FormatLiteral(s, true);

        if (value is char c)
            return SymbolDisplay.FormatLiteral(c, true);

        if (value is bool b)
            return b ? "true" : "false";

        if (value is float f)
            return f.ToString("R", CultureInfo.InvariantCulture) + "f";

        if (value is double d)
            return d.ToString("R", CultureInfo.InvariantCulture) + "d";

        if (value is decimal m)
            return m.ToString(CultureInfo.InvariantCulture) + "m";

        return string.Format(CultureInfo.InvariantCulture, "{0}", value);
    }

    private static bool HasNativeEqualityOperator(ITypeSymbol type,
        EquatableArray<BinaryOperatorInfo> binaryOperators)
    {
        // User-defined operator ==
        foreach (var op in binaryOperators)
        {
            if (op.Name == "op_Equality")
                return true;
        }

        // Built-in == for primitives
        return type.SpecialType is
            SpecialType.System_Boolean or
            SpecialType.System_Byte or SpecialType.System_SByte or
            SpecialType.System_Int16 or SpecialType.System_UInt16 or
            SpecialType.System_Int32 or SpecialType.System_UInt32 or
            SpecialType.System_Int64 or SpecialType.System_UInt64 or
            SpecialType.System_Single or SpecialType.System_Double or
            SpecialType.System_Decimal or SpecialType.System_Char or
            SpecialType.System_String;
    }

    private static bool ImplementsInterface(ITypeSymbol type, string interfaceFullName)
    {
        if (interfaceFullName.Contains("`"))
        {
            var baseName = interfaceFullName.Split('`')[0];
            return type.AllInterfaces.Any(i =>
                i.OriginalDefinition.ToDisplayString().StartsWith(baseName));
        }

        return type.AllInterfaces.Any(i => i.ToDisplayString() == interfaceFullName);
    }
}
