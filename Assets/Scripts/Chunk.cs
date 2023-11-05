using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
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
    List<int> transparentTriangles = new List<int>();
    Material[] materials = new Material[2];
    List<Vector2> uvs = new List<Vector2>();

    byte[,,] voxelMap = new byte[VoxelData.ChunkWidth, VoxelData.ChunkHeight, VoxelData.ChunkWidth];

    World world;


    private bool _isActive;
    public bool isVoxelMapPopulated = false;
    public Chunk(ChunkCoord _coord, World _world)
    {
        coord = _coord;
        world = _world;

    }

    public void Init()
    {

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        materials[0] = world.material;
        materials[1] = world.transparentMaterial;
        meshRenderer.materials = materials;

        chunkObject.transform.SetParent(world.transform);
        chunkObject.transform.position = new Vector3(coord.x * VoxelData.ChunkWidth, coord.y * VoxelData.ChunkHeight, coord.z * VoxelData.ChunkWidth);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y + ", " + coord.z;


        PopulateVoxelMap();
        UpdateChunk();

    }

    void PopulateVoxelMap()
    {


        for (int x = 0; x < VoxelData.ChunkWidth; x++)
        {
            for (int z = 0; z < VoxelData.ChunkWidth; z++)
            {
                for (int y = 0; y < VoxelData.ChunkHeight; y++)
                {
                    voxelMap[x, y, z] = world.GetVoxel(new Vector3(x, y, z) + position);
                }
            }
        }

    }

    void UpdateChunk()
    {

        ClearMeshData();

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    if (world.blocktypes[voxelMap[x, y, z]].isSolid)
                        UpdateMeshData(new Vector3(x, y, z));

                }
            }
        }

        CreateMesh();

    }

    void ClearMeshData()
    {

        vertexIndex = 0;
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();

    }
    void CreateMeshData()
    {

        for (int y = 0; y < VoxelData.ChunkHeight; y++)
        {
            for (int x = 0; x < VoxelData.ChunkWidth; x++)
            {
                for (int z = 0; z < VoxelData.ChunkWidth; z++)
                {

                    UpdateMeshData(new Vector3(x, y, z));

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
    bool CheckVoxelSolid(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world.blocktypes[voxelMap[x, y, z]].isSolid;

    }

    bool CheckVoxelUnimportant(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.blocktypes[world.GetVoxel(pos + position)].unimportant;

        return world.blocktypes[voxelMap[x, y, z]].unimportant;

    }

    bool CheckVoxelTransparency_I_think(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world.blocktypes[world.GetVoxel(pos + position)].transparency_I_think;

        return world.blocktypes[voxelMap[x, y, z]].transparency_I_think;

    }



    void UpdateMeshData(Vector3 pos)
    {

        byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

       // Currently Unneeded:  bool solid = world.blocktypes[blockID].isSolid;

        bool transparency_I_think = world.blocktypes[blockID].transparency_I_think; //TODO: transparency properly?

        bool unimportant = world.blocktypes[blockID].unimportant;

        for (int p = 0; p < 6; p++)
        {
            bool checkSolid = CheckVoxelSolid(pos + VoxelData.faceCheckVectors[p]);
            bool checkUnimportant = CheckVoxelUnimportant(pos + VoxelData.faceCheckVectors[p]);

            /// Unimportant blocks dont need face rendered if anything but air is next to them 
            /// 
            /// ( air currently has no toggles set, so is technically not solid, impotant, and not transparent lol)
            /// 
            /// Important blocks need face rendered if anything but another important block next to them



            if (unimportant)
            {
                if (checkSolid || checkUnimportant) // checkUnimportant does not need to be here i think? leaving it in case of future problems
                {
                    continue;
                    // if the block is unimportant and the adjacent one is literally anything but air
                }
            }
            else{
                if(checkSolid && !checkUnimportant)
                {
                    continue;
                }
            }
            

                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 0]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 1]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 2]]);
                vertices.Add(pos + VoxelData.voxelVerts[VoxelData.voxelTris[p, 3]]);

                AddTexture(world.blocktypes[blockID].GetTextureID(p));

                if (!transparency_I_think && !unimportant)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 2);
                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + 3);
                }
                else
                {
                    transparentTriangles.Add(vertexIndex);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 2);
                    transparentTriangles.Add(vertexIndex + 1);
                    transparentTriangles.Add(vertexIndex + 3);
                }

                vertexIndex += 4;

            
        }

    }

    void CreateMesh()
    {

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();

        mesh.subMeshCount = 2;
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.SetTriangles(transparentTriangles.ToArray(), 1);

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
    public ChunkCoord(int _x, int _z) : //TODO: learn why this is the right way, chatGPT spat this out :)
        this(_x, 0, _z)
    {
    }
}