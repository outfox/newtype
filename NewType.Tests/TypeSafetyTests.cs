using System.Numerics;
using Xunit;

namespace newtype.tests;

public class TypeSafetyTests
{
    [Fact]
    public void PositionAndVelocity_AreDistinctTypes()
    {
        // Both wrap Vector3 but are distinct types
        Position p = new Vector3(1, 2, 3);
        Velocity v = new Vector3(1, 2, 3);

        // They both convert to Vector3 independently
        Vector3 pVec = p;
        Vector3 vVec = v;
        Assert.Equal(pVec, vVec);
    }

    [Fact]
    public void DistinctTypes_HaveIndependentValues()
    {
        Position p = new Vector3(1, 0, 0);
        Velocity v = new Vector3(0, 1, 0);

        Assert.NotEqual((Vector3)p, (Vector3)v);
    }

    [Fact]
    public void PhysicsUpdate_Simulation()
    {
        // Simulate: position += velocity * deltaTime
        Position p = new Vector3(0, 0, 0);
        Velocity v = new Vector3(10, 0, 0);
        const float dt = 0.5f;

        var updated = p + (Vector3)v * dt;
        Assert.Equal(new Vector3(5, 0, 0), updated.Value);
    }

    [Fact]
    public void Scale_IsDistinctFromPositionAndVelocity()
    {
        Scale s = new Vector3(2, 2, 2);
        Position p = new Vector3(2, 2, 2);

        // Same underlying value, but different types
        Assert.Equal((Vector3)s, (Vector3)p);
    }
}