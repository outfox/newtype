using System.Numerics;
using System.Reflection;
using Xunit;

namespace newtype.tests;

public class ConstraintTests
{
    // ── StrictEntityId: NoImplicitWrap (no int → StrictEntityId) ──

    [Fact]
    public void StrictEntityId_NoWrapOperator()
    {
        var ops = typeof(StrictEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == typeof(StrictEntityId));

        Assert.Empty(ops);
    }

    [Fact]
    public void StrictEntityId_HasUnwrapOperator()
    {
        var ops = typeof(StrictEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == typeof(int));

        Assert.NotEmpty(ops);
    }

    [Fact]
    public void StrictEntityId_ConstructorAndValueWork()
    {
        var id = new StrictEntityId(42);
        Assert.Equal(42, id.Value);
    }

    // ── GuardedEntityId: NoImplicitUnwrap (no GuardedEntityId → int) ──

    [Fact]
    public void GuardedEntityId_NoUnwrapOperator()
    {
        var ops = typeof(GuardedEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == typeof(int));

        Assert.Empty(ops);
    }

    [Fact]
    public void GuardedEntityId_HasWrapOperator()
    {
        var ops = typeof(GuardedEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit" && m.ReturnType == typeof(GuardedEntityId));

        Assert.NotEmpty(ops);
    }

    [Fact]
    public void GuardedEntityId_ImplicitWrapAndValueWork()
    {
        GuardedEntityId id = 99;
        Assert.Equal(99, id.Value);
    }

    // ── RawPosition: NoConstructorForwarding (no (float,float,float) ctor) ──

    [Fact]
    public void RawPosition_NoForwardedConstructor()
    {
        // Vector3 has a (float, float, float) constructor.
        // With NoConstructorForwarding, it should not be forwarded.
        var ctor = typeof(RawPosition).GetConstructor([typeof(float), typeof(float), typeof(float)]);
        Assert.Null(ctor);
    }

    [Fact]
    public void RawPosition_HasPrimaryConstructor()
    {
        var ctor = typeof(RawPosition).GetConstructor([typeof(Vector3)]);
        Assert.NotNull(ctor);
    }

    [Fact]
    public void RawPosition_ImplicitConversionsWork()
    {
        var v = new Vector3(1, 2, 3);
        RawPosition pos = v;
        Vector3 back = pos;
        Assert.Equal(v, back);
    }

    // ── OpaqueId: NoImplicitConversions (no implicit conversions either way) ──

    [Fact]
    public void OpaqueId_NoImplicitOperators()
    {
        var ops = typeof(OpaqueId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit");

        Assert.Empty(ops);
    }

    [Fact]
    public void OpaqueId_ConstructorAndValueWork()
    {
        var id = new OpaqueId(7);
        Assert.Equal(7, id.Value);
    }

    [Fact]
    public void OpaqueId_ArithmeticWorks()
    {
        var a = new OpaqueId(10);
        var b = new OpaqueId(3);
        var result = a + b;
        Assert.Equal(13, result.Value);
    }

    // ── RelaxedEntityId: MethodImpl = default (no [MethodImpl] on generated members) ──

    [Fact]
    public void RelaxedEntityId_NoAggressiveInlining()
    {
        var addMethod = typeof(RelaxedEntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Addition");

        Assert.NotNull(addMethod);
        var flags = addMethod.MethodImplementationFlags;
        Assert.Equal(0, (int)(flags & MethodImplAttributes.AggressiveInlining));
    }

    [Fact]
    public void RelaxedEntityId_ArithmeticWorks()
    {
        var a = new RelaxedEntityId(5);
        var b = new RelaxedEntityId(3);
        var result = a + b;
        Assert.Equal(8, result.Value);
    }

    // ── EntityId (regression): default behavior unchanged ──

    [Fact]
    public void EntityId_HasBothImplicitOperators()
    {
        var ops = typeof(EntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => m.Name == "op_Implicit")
            .ToList();

        Assert.Equal(2, ops.Count);
        Assert.Contains(ops, m => m.ReturnType == typeof(EntityId));
        Assert.Contains(ops, m => m.ReturnType == typeof(int));
    }

    [Fact]
    public void EntityId_HasAggressiveInlining()
    {
        var addMethod = typeof(EntityId)
            .GetMethods(BindingFlags.Public | BindingFlags.Static)
            .FirstOrDefault(m => m.Name == "op_Addition");

        Assert.NotNull(addMethod);
        var flags = addMethod.MethodImplementationFlags;
        Assert.NotEqual(0, (int)(flags & MethodImplAttributes.AggressiveInlining));
    }
}
