using System.Numerics;
using newtype;

namespace newtype.benchmark;

[newtype<int>]
public readonly partial struct EntityId;

[newtype<Vector2>]
public readonly partial struct Position2;

[newtype<Vector3>]
public readonly partial struct Position;

[newtype<Vector4>]
public readonly partial struct Position4;

[newtype<int>]
public partial record struct Score;
