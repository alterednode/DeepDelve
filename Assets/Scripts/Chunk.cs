using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class Chunk
{

    //coordinates of the chunk IN CHUNKS
    public ChunkCoord coord;


    //Unity objects that we need to reference
    GameObject chunkObject;
    MeshRenderer meshRenderer;
    MeshFilter meshFilter;

    //Used for generating the mesh of this chunk
    int vertexIndex = 0;
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;

    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        meshRenderer.material = world.material;
        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, coord.y * VoxelData.ChunkHeight, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y + ", " + coord.z;





        PopulateVoxelMap();

        CreateMeshData();

        CreateMesh();
    }

    void PopulateVoxelMap()
    {

        
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                ///TODO: maybe calc perlin noise here, to calc once per x,y coord.
                /// this feels like the wrong spot to do it tho, but idk where else
                /// we could do it.
                

               // float maxHightNoise = Perlin.Noise(x, z);


                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {

                    voxelMap[x,y,z] = world.GetVoxel(new Vector3(x,y,z) + position);

                }
            }
        }

    }

    void CreateMeshData()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    AddVoxelDataToChunk(new Vector3(x, y, z));

                }
            }
        }

    }

    public bool isActive
    {
        get { return chunkObject.activeSelf; }
        set { chunkObject.SetActive(value); }
    }

    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }


    bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > VoxelData.ChunkWidth - 1 || y < 0 || y > VoxelData.ChunkHeight - 1 || z < 0 || z > VoxelData.ChunkWidth - 1)
            return false;
        else
            return true;

    }
    bool CheckVoxel(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world.blocktypes[voxelMap[x, y, z]].isSolid;

    }

    void AddVoxelDataToChunk(Vector3 pos)
    {

        for (int p = 0; p < 6; p++)
        {

            if (!CheckVoxel(pos + VoxelData.faceChecks[p]))
            {
                //doing some cheeky stuff to have less vertecies in world :)
                byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

                if (blockID != 0) // skip air 
                {

                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                    vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                    AddTexture(world.blocktypes[blockID].GetTextureID(p));

                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                    vertexIndex += 4;
                }
            }
        }

    }

    void CreateMesh()
    {
        //Create the mesh, the monster mesh

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();

        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

    }

    void AddTexture(int textureID)
    {
        //TODO: maybe update to accomadate texture atlas of not square.
        float y = textureID / VoxelData.TextureAtlasSizeInBlocks;
        float x = textureID - (y * VoxelData.TextureAtlasSizeInBlocks);

        x *= VoxelData.NormalizedBlockTextureSize;
        y *= VoxelData.NormalizedBlockTextureSize;

        y = 1f - y - VoxelData.NormalizedBlockTextureSize;

        uvs.Add(new Vector2(x, y));
        uvs.Add(new Vector2(x, y + VoxelData.NormalizedBlockTextureSize));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y));
        uvs.Add(new Vector2(x + VoxelData.NormalizedBlockTextureSize, y + VoxelData.NormalizedBlockTextureSize));


    }

}

public class ChunkCoord
{
    public int x;
    public int y;
    public int z;

    public ChunkCoord(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;


    }
    public ChunkCoord(int _x, int _z): //TODO: learn why this is the right way, chatGPT spat this out :)
        this(_x, 0, _z)
    {
    }
}