using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.UI;

[BurstCompile]
public struct VoxelMeshJob : IJob
{
    [ReadOnly]
    public NativeArray<Voxel> voxels;

    public NativeList<float3> vertices;
    public NativeList<int> triangles;
    public NativeList<float2> uvs;

    const int sizeX = MapData.CHUNK_SIZE_X;
    const int sizeY = MapData.CHUNK_SIZE_Y;
    const int sizeZ = MapData.CHUNK_SIZE_Z;

    public void Execute()
    {
        GenerateTopFace();
        GenerateBottomFace();
        GenerateFrontFace();
        GenerateBackFace();
        GenerateRightFace();
        GenerateLeftFace();
    }

    private Voxel Get(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0) return new Voxel { type = VoxelType.Air };
        if (x >= sizeX || y >= sizeY || z >= sizeZ) return new Voxel { type = VoxelType.Air };
        return voxels[x + sizeX * (y + sizeY * z)];
    }

    private bool IsFaceVisible(int posX, int posY, int posZ, int normalX, int normalY, int normalZ)
    {
        Voxel current = Get(posX, posY, posZ);
        if (current.type == VoxelType.Air) return false;
        Voxel next = Get(posX + normalX, posY + normalY, posZ + normalZ);
        return next.type == VoxelType.Air;
    }

    private void GenerateTopFace()
    {
        NativeArray<bool> mask = new (sizeX * sizeY, Allocator.Temp);
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeZ; z++)
                    mask[x + sizeX * z] = IsFaceVisible(x, y, z, 0, 1, 0);

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (!mask[x + sizeX * z]) continue;
                    int startX = x, startZ = z, endX = x, endZ = z;

                    while (endX < sizeX && mask[endX + sizeX * endZ]) endX++;

                    bool done = false;
                    while (!done)
                    {
                        int newZ = endZ + 1;
                        if (newZ >= sizeZ) break;
                        for (int innerX = startX; innerX < endX; innerX++)
                        {
                            if (!mask[innerX + sizeX * newZ])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endZ++;
                    }
                    endZ++;

                    for (int innerX = startX; innerX < endX; innerX++)
                        for (int innerZ = startZ; innerZ < endZ; innerZ++)
                            mask[innerX + sizeX * innerZ] = false;

                    int index = vertices.Length;
                    // up
                    vertices.Add(new float3(startX, y + 1, startZ));
                    vertices.Add(new float3(startX, y + 1, endZ));
                    vertices.Add(new float3(endX, y + 1, endZ));
                    vertices.Add(new float3(endX, y + 1, startZ));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(startX, startZ));
                    uvs.Add(new float2(startX, endZ));
                    uvs.Add(new float2(endX, endZ));
                    uvs.Add(new float2(endX, startZ));
                }
            }
        }
        mask.Dispose();
    }
    private void GenerateBottomFace()
    {
        NativeArray<bool> mask = new (sizeX * sizeZ, Allocator.Temp);
        for (int y = 0; y < sizeY; y++)
        {
            for (int x = 0; x < sizeX; x++)
                for (int z = 0; z < sizeZ; z++)
                    mask[x + sizeX * z] = IsFaceVisible(x, y, z, 0, -1, 0);

            for (int x = 0; x < sizeX; x++)
            {
                for (int z = 0; z < sizeZ; z++)
                {
                    if (!mask[x + sizeX * z]) continue;
                    int startX = x, startZ = z, endX = x, endZ = z;

                    while (endX < sizeX && mask[endX + sizeX * endZ]) endX++;

                    bool done = false;
                    while (!done)
                    {
                        int newZ = endZ + 1;
                        if (newZ >= sizeZ) break;
                        for (int innerX = startX; innerX < endX; innerX++)
                        {
                            if (!mask[innerX + sizeX * newZ])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endZ++;
                    }
                    endZ++;

                    for (int innerX = startX; innerX < endX; innerX++)
                        for (int innerZ = startZ; innerZ < endZ; innerZ++)
                            mask[innerX + sizeX * innerZ] = false;

                    int index = vertices.Length;
                    // down
                    vertices.Add(new float3(startX, y, startZ));
                    vertices.Add(new float3(endX, y, startZ));
                    vertices.Add(new float3(endX, y, endZ));
                    vertices.Add(new float3(startX, y, endZ));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(startX, startZ));
                    uvs.Add(new float2(endX, startZ));
                    uvs.Add(new float2(endX, endZ));
                    uvs.Add(new float2(startX, endZ));
                }
            }
        }
        mask.Dispose();
    }

    private void GenerateFrontFace()
    {
        NativeArray<bool> mask = new (sizeX * sizeY, Allocator.Temp);
        for (int z = 0; z < sizeZ; z++)
        {
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    mask[x + sizeX * y] = IsFaceVisible(x, y, z, 0, 0, 1);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!mask[x + sizeX * y]) continue;
                    int startX = x, startY = y, endX = x, endY = y;

                    while (endX < sizeX && mask[endX + sizeX * endY]) endX++;

                    bool done = false;
                    while (!done)
                    {
                        int newY = endY + 1;
                        if (newY >= sizeY) break;
                        for (int innerX = startX; innerX < endX; innerX++)
                        {
                            if (!mask[innerX + sizeX * newY])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endY++;
                    }
                    endY++;

                    for (int innerX = startX; innerX < endX; innerX++)
                        for (int innerY = startY; innerY < endY; innerY++)
                            mask[innerX + sizeX * innerY] = false;

                    int index = vertices.Length;
                    // forward
                    vertices.Add(new float3(startX, startY, z + 1));
                    vertices.Add(new float3(endX, startY, z + 1));
                    vertices.Add(new float3(endX, endY, z + 1));
                    vertices.Add(new float3(startX, endY, z + 1));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(startX, startY));
                    uvs.Add(new float2(endX, startY));
                    uvs.Add(new float2(endX, endY));
                    uvs.Add(new float2(startX, endY));
                }
            }
        }
        mask.Dispose();
    }
    private void GenerateBackFace()
    {
        NativeArray<bool> mask = new (sizeX * sizeY, Allocator.Temp);
        for (int z = 0; z < sizeZ; z++)
        {
            for (int x = 0; x < sizeX; x++)
                for (int y = 0; y < sizeY; y++)
                    mask[x + sizeX * y] = IsFaceVisible(x, y, z, 0, 0, -1);

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!mask[x + sizeX * y]) continue;
                    int startX = x, startY = y, endX = x, endY = y;

                    while (endX < sizeX && mask[endX + sizeX * endY]) endX++;

                    bool done = false;
                    while (!done)
                    {
                        int newY = endY + 1;
                        if (newY >= sizeY) break;
                        for (int innerX = startX; innerX < endX; innerX++)
                        {
                            if (!mask[innerX + sizeX * newY])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endY++;
                    }
                    endY++;

                    for (int innerX = startX; innerX < endX; innerX++)
                        for (int innerY = startY; innerY < endY; innerY++)
                            mask[innerX + sizeX * innerY] = false;

                    int index = vertices.Length;
                    // l'arrière
                    vertices.Add(new float3(endX, startY, z));
                    vertices.Add(new float3(startX, startY, z));
                    vertices.Add(new float3(startX, endY, z));
                    vertices.Add(new float3(endX, endY, z));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(endX, startY));
                    uvs.Add(new float2(startX, startY));
                    uvs.Add(new float2(startX, endY));
                    uvs.Add(new float2(endX, endY));
                }
            }
        }
        mask.Dispose();
    }
    private void GenerateRightFace()
    {
        NativeArray<bool> mask = new (sizeZ * sizeY, Allocator.Temp);
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
                for (int y = 0; y < sizeY; y++)
                    mask[z + sizeZ * y] = IsFaceVisible(x, y, z, 1, 0, 0);

            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!mask[z + sizeZ * y]) continue;
                    int startZ = z, startY = y, endZ = z, endY = y;

                    while (endZ < sizeZ && mask[endZ + sizeZ * endY]) endZ++;

                    bool done = false;
                    while (!done)
                    {
                        int newY = endY + 1;
                        if (newY >= sizeY) break;
                        for (int innerZ = startZ; innerZ < endZ; innerZ++)
                        {
                            if (!mask[innerZ + sizeZ * newY])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endY++;
                    }
                    endY++;

                    for (int innerZ = startZ; innerZ < endZ; innerZ++)
                        for (int innerY = startY; innerY < endY; innerY++)
                            mask[innerZ + sizeZ * innerY] = false;

                    int index = vertices.Length;
                    /// vue de droite
                    vertices.Add(new float3(x + 1, startY, endZ));
                    vertices.Add(new float3(x + 1, startY, startZ));
                    vertices.Add(new float3(x + 1, endY, startZ));
                    vertices.Add(new float3(x + 1, endY, endZ));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(endZ, startY));
                    uvs.Add(new float2(startZ, startY));
                    uvs.Add(new float2(startZ, endY));
                    uvs.Add(new float2(endZ, endY));
                }
            }
        }
        mask.Dispose();
    }

    private void GenerateLeftFace()
    {
        NativeArray<bool> mask = new(sizeZ * sizeY, Allocator.Temp);
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
                for (int y = 0; y < sizeY; y++)
                    mask[z + sizeZ * y] = IsFaceVisible(x, y, z, -1, 0, 0);

            for (int z = 0; z < sizeZ; z++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (!mask[z + sizeZ * y]) continue;
                    int startZ = z, startY = y, endZ = z, endY = y;

                    while (endZ < sizeZ && mask[endZ + sizeZ * endY]) endZ++;

                    bool done = false;
                    while (!done)
                    {
                        int newY = endY + 1;
                        if (newY >= sizeY) break;
                        for (int innerZ = startZ; innerZ < endZ; innerZ++)
                        {
                            if (!mask[innerZ + sizeZ * newY])
                            {
                                done = true;
                                break;
                            }
                        }
                        if (!done) endY++;
                    }
                    endY++;

                    for (int innerZ = startZ; innerZ < endZ; innerZ++)
                        for (int innerY = startY; innerY < endY; innerY++)
                            mask[innerZ + sizeZ * innerY] = false;

                    int index = vertices.Length;
                    /// Vue de gauche
                    vertices.Add(new float3(x, startY, startZ));
                    vertices.Add(new float3(x, startY, endZ));
                    vertices.Add(new float3(x, endY, endZ));
                    vertices.Add(new float3(x, endY, startZ));

                    triangles.Add(index);
                    triangles.Add(index + 1);
                    triangles.Add(index + 2);
                    triangles.Add(index);
                    triangles.Add(index + 2);
                    triangles.Add(index + 3);

                    uvs.Add(new float2(startZ, startY));
                    uvs.Add(new float2(endZ, startY));
                    uvs.Add(new float2(endZ, endY));
                    uvs.Add(new float2(startZ, endY));
                }
            }
        }
        mask.Dispose();
    }
    
}
