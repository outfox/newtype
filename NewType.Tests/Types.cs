using System.Numerics;

namespace newtype.tests;

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

[newtype<EmailAddress>]
public readonly partial struct ContactEmail;

[newtype<Money>]
public readonly partial struct Price;

[newtype<Rgb>]
public readonly partial struct Tint;

// record struct variants
[newtype<int>]
public partial record struct Score;

[newtype<double>]
public readonly partial record struct Duration;

// class wrappers
[newtype<int>]
public partial class ClassEntityId;

[newtype<string>]
public partial class ClassName;

[newtype<EmailAddress>]
public partial class ClassContactEmail;

// record class wrappers
[newtype<int>]
public partial record RecordEntityId;

[newtype<double>]
public partial record RecordTimestamp;

[newtype<Rgb>]
public partial record RecordTint;