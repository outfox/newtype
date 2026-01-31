using System.Numerics;
using newtype;

namespace newtype.benchmark;

[newtype<int>]
public readonly partial struct EntityId;

[newtype<Vector3>]
public readonly partial struct Position;

[newtype<int>]
public partial record struct Score;
