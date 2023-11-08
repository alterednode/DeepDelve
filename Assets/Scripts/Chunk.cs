using System.Collections.Generic;
using UnityEngine;

[System.Serializable] //temp to see things in unity inspector
public class Chunk
{
    World world;
    BigChunk bigChunk;
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

    byte[,,] voxelMap;


    public bool isVoxelMapPopulated = false;
    public Chunk(ChunkCoord coord, World world, BigChunk bigChunk)
    {
        this.coord = coord;
        this.world = world;
        this.bigChunk = bigChunk;
        voxelMap = new byte[world._chunkSize, world._chunkSize, world._chunkSize];
    }   

    public void Init()
    {

        chunkObject = new GameObject();
        meshFilter = chunkObject.AddComponent<MeshFilter>();
        meshRenderer = chunkObject.AddComponent<MeshRenderer>();

        materials[0] = world.material;
        materials[1] = world.transparentMaterial;
        meshRenderer.materials = materials;

        chunkObject.transform.SetParent(bigChunk.bigChunkObject.transform);
        chunkObject.transform.position = new Vector3(coord.x * world._chunkSize, coord.y * world._chunkSize, coord.z * world._chunkSize);
        chunkObject.name = "Chunk " + coord.x + ", " + coord.y + ", " + coord.z;


        PopulateVoxelMap();
        UpdateChunk();
    }

    void PopulateVoxelMap()
    {
        isVoxelMapPopulated = true;

        for (int x = 0; x < world._chunkSize; x++)
        {
            for (int z = 0; z < world._chunkSize; z++)
            {
                for (int y = 0; y < world._chunkSize; y++)
                {
                    voxelMap[x, y, z] = world.GenerateVoxel(new Vector3(x, y, z) + position);
                }
            }
        }

    }

    public void UpdateChunk()
    {

        ClearMeshData();

        for (int y = 0; y < world._chunkSize; y++)
        {
            for (int x = 0; x < world._chunkSize; x++)
            {
                for (int z = 0; z < world._chunkSize; z++)
                {

                    if (world._blocktypes[voxelMap[x, y, z]].isSolid)
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
        transparentTriangles.Clear();
        uvs.Clear();

    }

    //is this used anywhere? its similar to the middle of updateChunk
    void CreateMeshData()
    {

        for (int y = 0; y < world._chunkSize; y++)
        {
            for (int x = 0; x < world._chunkSize; x++)
            {
                for (int z = 0; z < world._chunkSize; z++)
                {

                    UpdateMeshData(new Vector3(x, y, z));

                }
            }
        }

    }
    public Vector3 position
    {
        get { return chunkObject.transform.position; }
    }


    public bool IsVoxelInChunk(int x, int y, int z)
    {
        if (x < 0 || x > world._chunkSize - 1 || y < 0 || y > world._chunkSize - 1 || z < 0 || z > world._chunkSize - 1)
            return false;
        else
            return true;

    }
    public bool IsVoxelInChunk(Vector3 pos)
    {
        return IsVoxelInChunk(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y), Mathf.FloorToInt(pos.z));
    }


    public void EditVoxel(Vector3 pos, byte newID)
    {

        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        yCheck -= Mathf.FloorToInt(chunkObject.transform.position.y);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;

        UpdateSurroundingVoxels(xCheck, yCheck, zCheck);

        UpdateChunk();

    }

    public void DirectlySetVoxel(Vector3 pos, byte newID)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        yCheck -= Mathf.FloorToInt(chunkObject.transform.position.y);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        voxelMap[xCheck, yCheck, zCheck] = newID;
    }

    public byte GetVoxel(Vector3 pos)
    {
        int xCheck = Mathf.FloorToInt(pos.x);
        int yCheck = Mathf.FloorToInt(pos.y);
        int zCheck = Mathf.FloorToInt(pos.z);

        xCheck -= Mathf.FloorToInt(chunkObject.transform.position.x);
        yCheck -= Mathf.FloorToInt(chunkObject.transform.position.y);
        zCheck -= Mathf.FloorToInt(chunkObject.transform.position.z);

        return voxelMap[xCheck, yCheck, zCheck];
    }

    void UpdateSurroundingVoxels(int x, int y, int z)
    {

        Vector3 thisVoxel = new Vector3(x, y, z);

        for (int p = 0; p < 6; p++)
        {

            Vector3 currentVoxel = thisVoxel + VoxelData.faceCheckVectors[p];
            
            if (!IsVoxelInChunk((int)currentVoxel.x, (int)currentVoxel.y, (int)currentVoxel.z))
            {
                if(world.IsPosInWorld(currentVoxel + position)) 
                world.GetChunkFromVector3(currentVoxel + position).UpdateChunk();

            }

        }

    }
    bool CheckVoxelSolid(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world._blocktypes[world.GetVoxel(pos + position)].isSolid;

        return world._blocktypes[voxelMap[x, y, z]].isSolid;

    }

    bool CheckVoxelUnimportant(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world._blocktypes[world.GetVoxel(pos + position)].unimportant;

        return world._blocktypes[voxelMap[x, y, z]].unimportant;

    }

    bool CheckVoxelTransparency_I_think(Vector3 pos)
    {
        //check if the voxl is solid using the blockdata in the world unity obj world script
        int x = Mathf.FloorToInt(pos.x);
        int y = Mathf.FloorToInt(pos.y);
        int z = Mathf.FloorToInt(pos.z);

        if (!IsVoxelInChunk(x, y, z))
            return world._blocktypes[world.GetVoxel(pos + position)].transparency_I_think;

        return world._blocktypes[voxelMap[x, y, z]].transparency_I_think;

    }



    void UpdateMeshData(Vector3 pos)
    {

        byte blockID = voxelMap[(int)pos.x, (int)pos.y, (int)pos.z];

       // Currently Unneeded:  bool solid = world.blocktypes[blockID].isSolid;

        bool transparency_I_think = world._blocktypes[blockID].transparency_I_think; //TODO: transparency properly?

        bool unimportant = world._blocktypes[blockID].unimportant;

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

                AddTexture(world._blocktypes[blockID].GetTextureID(p));

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