using System;
using UnityEngine;

[Flags]
enum VoxelType : byte
{
    Air,
    Stone,
    Grass
}

struct Voxel
{
    public VoxelType type;
}