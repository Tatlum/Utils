# Ermine Games Utils

Utility library for Unity development. Provides extension methods and helper classes for common tasks.

## Features

### CollectionUtils
Fast collection operations with O(1) element removal and convenience methods:
- `RemoveAtFast<T>()` - Remove element by swapping with last (O(1))
- `GetOrCreate<TKey, TValue>()` - Get or create dictionary value

### GameObjectUtils
GameObject manipulation utilities:
- `DestroyAllChildren()` - Destroy all child transforms

### LimitedSizeDictionary
Dictionary with capacity limits:
- Automatically removes oldest entries when capacity is exceeded

### NotifiedProperty
Observable property wrapper with change notifications:
- `OnChanged` event with old and new values

### PhysicsUtils  
Physics and collider utilities:
- `ToKmH()` / `ToMSec()` - Velocity conversion
- `CalculateAccelerationTime()` - Physics calculations
- `GetRandomPoint()` - Random points in colliders (Box, Sphere)

### RandomUtils
Thread-safe random utilities:
- `WeightRandom<T>()` - Weighted random selection
- `GetRandom<T>()` - Random list element
- `Shuffle<T>()` - Fisher-Yates shuffle
- `EnumRandom<TEnum>()` - Random enum value

### TimeUtils
Time conversion utilities:
- `ToMs()` - Convert seconds to milliseconds
- `ToSeconds()` - Convert milliseconds to seconds

### NavMeshUtils
Navigation utilities (requires Unity AI Navigation):
- `GetNavMeshPoint()` - Find closest point on NavMesh
- `PlaceToNavMesh()` - Place transform on NavMesh
- `HasReachedDestination()` - Check agent arrival

### SequentialIDGenerator
Simple sequential ID generation

### ReflectionUtils (Editor Only)
Reflection utilities for editor tools:
- `FindCallerInStack()` - Find calling type in stack trace
- `ReflectType()` - Deep reflection with caching
- Object graph traversal with cycle detection

## Installation

### For Unity (UPM)

The package is located in your project's `Packages` directory. To use in other projects:

1. Copy `Packages/com.ermine.utils` folder to your project's `Packages` directory
2. Or add to `Packages/manifest.json`:
```json
{
  "dependencies": {
    "com.ermine.utils": "file:../path/to/com.ermine.utils"
  }
}
```

## License

MIT License - See LICENSE file for details
