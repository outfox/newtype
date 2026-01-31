using System.Numerics;
using newtype;

[newtype<Vector3>]
public readonly partial struct Position;

[newtype<Vector3>]
public readonly partial struct Velocity;

[newtype<Vector3>]
public readonly partial struct Scale;

[newtype<Quaternion>]
public readonly partial struct Rotation;

[newtype<Matrix4x4>]
public readonly partial struct Transform;

[newtype<int>]
public readonly partial struct EntityId;

[newtype<float>]
public readonly partial struct Health;

[newtype<double>]
public readonly partial struct Timestamp;

[newtype<bool>]
public readonly partial struct IsActive;

[newtype<string>]
public readonly partial struct Name;
