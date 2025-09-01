using System;
using UnityEngine;

[Flags]
public enum VoxelType : byte
{
    Air,
    Stone,
    Grass
}

public struct Voxel
{
    public VoxelType type;
}