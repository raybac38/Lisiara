using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// This struct is an InterestNode
/// an abstract point in a chunk that have two rule :
/// 1 - every tile of the chunk must have a path to a InterestNode of the chunk
/// 2 - two InterestPoint must not be link by a path inside the chunk
/// </summary>
public class InterestNode
{
    public int3 chunkPosition;
    public int3 relativeCoordinate;
}
