using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace newtype.generator;

/// <summary>
/// Fully-extracted, equatable model representing a newtype alias.
/// Contains only strings, bools, plain enums, and EquatableArrays — no Roslyn symbols.
/// </summary>
internal readonly record struct AliasModel(
    // Type being declared
    string TypeName,
    string Namespace,
    Accessibility DeclaredAccessibility,
    bool IsReadonly,
    bool IsClass,
    bool IsRecord,
    bool IsRecordStruct,

    // Aliased type
    string AliasedTypeFullName,
    string AliasedTypeMinimalName,
    SpecialType AliasedTypeSpecialType,
    bool AliasedTypeIsValueType,

    // Interface flags
    bool ImplementsIComparable,
    bool ImplementsIFormattable,
    bool HasNativeEqualityOperator,

    // Pre-computed file name
    string TypeDisplayString,

    // Whether the aliased type has any public static non-operator members
    // (used to emit the #region even when no property/readonly-field members survive filtering)
    bool HasStaticMemberCandidates,

    // Constraint parameters from [newtype] attribute
    bool SuppressImplicitWrap,
    bool SuppressImplicitUnwrap,
    bool SuppressConstructorForwarding,
    int MethodImplValue,

    // Members
    EquatableArray<BinaryOperatorInfo> BinaryOperators,
    EquatableArray<UnaryOperatorInfo> UnaryOperators,
    EquatableArray<StaticMemberInfo> StaticMembers,
    EquatableArray<InstanceFieldInfo> InstanceFields,
    EquatableArray<InstancePropertyInfo> InstanceProperties,
    EquatableArray<InstanceMethodInfo> InstanceMethods,
    EquatableArray<ConstructorInfo> ForwardedConstructors
);

internal readonly record struct BinaryOperatorInfo(
    string Name,
    string LeftTypeFullName,
    string RightTypeFullName,
    string ReturnTypeFullName,
    bool LeftIsAliasedType,
    bool RightIsAliasedType,
    bool ReturnIsAliasedType
) : IEquatable<BinaryOperatorInfo>;

internal readonly record struct UnaryOperatorInfo(
    string Name,
    string ReturnTypeFullName,
    bool ReturnIsAliasedType
) : IEquatable<UnaryOperatorInfo>;

internal readonly record struct StaticMemberInfo(
    string Name,
    string TypeFullName,
    bool TypeIsAliasedType,
    bool IsProperty,
    bool IsReadonlyField
) : IEquatable<StaticMemberInfo>;

internal readonly record struct InstanceFieldInfo(
    string Name,
    string TypeFullName,
    bool TypeIsAliasedType
) : IEquatable<InstanceFieldInfo>;

internal readonly record struct InstancePropertyInfo(
    string Name,
    string TypeFullName,
    bool TypeIsAliasedType,
    bool HasGetter
) : IEquatable<InstancePropertyInfo>;

internal readonly record struct InstanceMethodInfo(
    string Name,
    string ReturnTypeFullName,
    bool ReturnsVoid,
    bool ReturnIsAliasedType,
    bool SkipReturnWrapping,
    EquatableArray<MethodParameterInfo> Parameters
) : IEquatable<InstanceMethodInfo>;

internal readonly record struct MethodParameterInfo(
    string Name,
    string TypeFullName,
    RefKind RefKind,
    bool IsAliasedType
) : IEquatable<MethodParameterInfo>;

internal readonly record struct ConstructorInfo(
    EquatableArray<ConstructorParameterInfo> Parameters
) : IEquatable<ConstructorInfo>;

internal readonly record struct ConstructorParameterInfo(
    string Name,
    string TypeFullName,
    RefKind RefKind,
    bool IsParams,
    string? DefaultValueLiteral
) : IEquatable<ConstructorParameterInfo>;
