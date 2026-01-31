using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

namespace newtype.tests;
public class RefTests
{
    [Fact]
    public void RefLocal_FromArray_ReadsCorrectValue()
    {
        var positions = new Position[]
        {
            new Vector3(1, 2, 3),
            new Vector3(4, 5, 6),
            new Vector3(7, 8, 9),
        };

        ref var pos = ref positions[1];

        Assert.Equal(4f, pos.X);
        Assert.Equal(5f, pos.Y);
        Assert.Equal(6f, pos.Z);
    }

    [Fact]
    public void RefLocal_FromArray_Reassignment_UpdatesArray()
    {
        var positions = new Position[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
        };

        ref var pos = ref positions[0];
        pos = new Vector3(10, 20, 30);

        Assert.Equal(new Vector3(10, 20, 30), positions[0].Value);
    }

    [Fact]
    public void RefLocal_FromSpan_ReadsCorrectValue()
    {
        var positions = new Position[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
        };

        Span<Position> span = positions;
        ref var pos = ref span[2];

        Assert.Equal(new Vector3(0, 0, 1), pos.Value);
    }

    [Fact]
    public void RefLocal_FromSpan_Reassignment_UpdatesStorage()
    {
        var positions = new Position[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
        };

        Span<Position> span = positions;
        ref var pos = ref span[1];
        pos = new Vector3(5, 10, 15);

        Assert.Equal(new Vector3(5, 10, 15), positions[1].Value);
    }

    [Fact]
    public void RefReturn_FromComponentStore_ReadsThrough()
    {
        var store = new ComponentStore<Position>(4)
        {
            [0] = new Vector3(1, 2, 3),
            [1] = new Vector3(4, 5, 6),
        };

        ref var pos = ref store.GetRef(0);

        Assert.Equal(1f, pos.X);
        Assert.Equal(2f, pos.Y);
        Assert.Equal(3f, pos.Z);
    }

    [Fact]
    public void RefReturn_FromComponentStore_WritesThrough()
    {
        var store = new ComponentStore<Position>(4)
        {
            [0] = new Vector3(0, 0, 0),
        };

        ref var pos = ref store.GetRef(0);
        pos = new Vector3(99, 88, 77);

        Assert.Equal(new Vector3(99, 88, 77), store[0].Value);
    }

    [Fact]
    public void RefReadonly_PreventsReassignment_ButAllowsReading()
    {
        var positions = new Position[]
        {
            new Vector3(3, 6, 9),
        };

        ref readonly var pos = ref positions[0];

        Assert.Equal(3f, pos.X);
        Assert.Equal(6f, pos.Y);
        Assert.Equal(9f, pos.Z);
        Assert.Equal(new Vector3(3, 6, 9), pos.Value);
    }

    [Fact]
    public void InParameter_PassesByReadonlyRef()
    {
        Position a = new Vector3(1, 2, 3);
        Position b = new Vector3(4, 5, 6);

        var result = AddPositions(in a, in b);

        Assert.Equal(new Vector3(5, 7, 9), result.Value);

        static Position AddPositions(in Position left, in Position right)
        {
            return left + right;
        }
    }

    [Fact]
    public void RefParameter_AllowsMutation()
    {
        Position pos = new Vector3(1, 0, 0);

        ApplyGravity(ref pos, dt: 1.0f);

        Assert.Equal(new Vector3(1, -9.81f, 0), pos.Value);

        static void ApplyGravity(ref Position p, float dt)
        {
            var gravity = new Vector3(0, -9.81f, 0);
            p = p + gravity * dt;
        }
    }

    [Fact]
    public void EcsUpdate_RefIteration_OverArray()
    {
        var positions = new Position[]
        {
            new Vector3(0, 0, 0),
            new Vector3(10, 0, 0),
            new Vector3(20, 0, 0),
        };

        var velocities = new Velocity[]
        {
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
        };

        const float dt = 0.5f;
        for (var i = 0; i < positions.Length; i++)
        {
            ref var pos = ref positions[i];
            ref readonly var vel = ref velocities[i];
            pos += (Vector3)vel * dt;
        }

        Assert.Equal(new Vector3(0.5f, 0, 0), positions[0].Value);
        Assert.Equal(new Vector3(10, 0.5f, 0), positions[1].Value);
        Assert.Equal(new Vector3(20, 0, 0.5f), positions[2].Value);
    }

    [Fact]
    public void EcsUpdate_RefIteration_OverSpan()
    {
        var posArr = new Position[]
        {
            new Vector3(0, 100, 0),
            new Vector3(0, 200, 0),
        };

        var velArr = new Velocity[]
        {
            new Vector3(0, -10, 0),
            new Vector3(0, -20, 0),
        };

        Span<Position> positions = posArr;
        ReadOnlySpan<Velocity> velocities = velArr;

        for (var i = 0; i < positions.Length; i++)
        {
            ref var pos = ref positions[i];
            var vel = velocities[i];
            pos = pos + vel;
        }

        Assert.Equal(new Vector3(0, 90, 0), posArr[0].Value);
        Assert.Equal(new Vector3(0, 180, 0), posArr[1].Value);
    }

    [Fact]
    public void UnsafeAs_ReinterpretCast_SameLayout()
    {
        // Position is a single-field struct wrapping Vector3,
        // so it has the same memory layout
        Position pos = new Vector3(1, 2, 3);
        ref var raw = ref Unsafe.As<Position, Vector3>(ref pos);

        Assert.Equal(1f, raw.X);
        Assert.Equal(2f, raw.Y);
        Assert.Equal(3f, raw.Z);
    }

    [Fact]
    public void UnsafeAs_ReinterpretCast_WritesThrough()
    {
        Position pos = new Vector3(0, 0, 0);
        ref var raw = ref Unsafe.As<Position, Vector3>(ref pos);
        raw = new Vector3(42, 43, 44);

        Assert.Equal(new Vector3(42, 43, 44), pos.Value);
    }

    [Fact]
    public void MemoryMarshal_Cast_SpanReinterpret()
    {
        var positions = new Position[]
        {
            new Vector3(1, 2, 3),
            new Vector3(4, 5, 6),
        };

        var vectors = MemoryMarshal.Cast<Position, Vector3>((Span<Position>)positions);

        Assert.Equal(2, vectors.Length);
        Assert.Equal(new Vector3(1, 2, 3), vectors[0]);
        Assert.Equal(new Vector3(4, 5, 6), vectors[1]);
    }

    [Fact]
    public void MemoryMarshal_Cast_MutationReflectsBack()
    {
        var positions = new Position[]
        {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
        };

        var vectors = MemoryMarshal.Cast<Position, Vector3>((Span<Position>)positions);
        vectors[0] = new Vector3(7, 8, 9);
        vectors[1] = new Vector3(10, 11, 12);

        Assert.Equal(new Vector3(7, 8, 9), positions[0].Value);
        Assert.Equal(new Vector3(10, 11, 12), positions[1].Value);
    }

    [Fact]
    public void RefReturn_ChainedAccess()
    {
        var store = new ComponentStore<Position>(8)
        {
            [0] = new Vector3(1, 0, 0),
            [1] = new Vector3(0, 1, 0),
            [2] = new Vector3(0, 0, 1),
        };

        // Swap two components via ref
        ref var a = ref store.GetRef(0);
        ref var b = ref store.GetRef(2);
        (a, b) = (b, a);

        Assert.Equal(new Vector3(0, 0, 1), store[0].Value);
        Assert.Equal(new Vector3(1, 0, 0), store[2].Value);
    }

    [Fact]
    public void ComponentStore_SpanAccess_BulkUpdate()
    {
        var store = new ComponentStore<Position>(4)
        {
            [0] = new Vector3(0, 0, 0),
            [1] = new Vector3(1, 1, 1),
            [2] = new Vector3(2, 2, 2),
            [3] = new Vector3(3, 3, 3),
        };

        var span = store.AsSpan();
        var offset = new Vector3(100, 0, 0);

        for (var i = 0; i < span.Length; i++)
        {
            ref var pos = ref span[i];
            pos = pos + offset;
        }

        Assert.Equal(100f, store[0].X);
        Assert.Equal(101f, store[1].X);
        Assert.Equal(102f, store[2].X);
        Assert.Equal(103f, store[3].X);
    }

    [Fact]
    public void MixedTypes_RefIteration_TypeSafety()
    {
        var positions = new Position[] { new Vector3(0, 0, 0) };
        var velocities = new Velocity[] { new Vector3(5, 5, 5) };
        var scales = new Scale[] { new Vector3(2, 2, 2) };

        ref var pos = ref positions[0];
        ref readonly var vel = ref velocities[0];
        ref readonly var scl = ref scales[0];

        // Apply velocity then scale — implicit conversions needed
        pos = new Position((Vector3)pos + (Vector3)vel);
        var scaled = new Position((Vector3)pos * (Vector3)scl);

        Assert.Equal(new Vector3(5, 5, 5), pos.Value);
        Assert.Equal(new Vector3(10, 10, 10), scaled.Value);
    }

    /// <summary>
    /// Minimal ECS-style component store that supports ref returns.
    /// </summary>
    private sealed class ComponentStore<T>(int capacity)
        where T : struct
    {
        private readonly T[] _data = new T[capacity];

        public ref T GetRef(int index) => ref _data[index];

        public T this[int index]
        {
            get => _data[index];
            // ReSharper disable once PropertyCanBeMadeInitOnly.Local
            set => _data[index] = value;
        }

        public Span<T> AsSpan() => _data.AsSpan();
    }
}
