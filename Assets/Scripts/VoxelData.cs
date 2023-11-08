using UnityEngine;

public static class VoxelData
{
    //Read these from a config file?
    //maybe not chunk hieght/width


    //all of these ints should be powers of 2

    // amount of chunks to load at first
    public static readonly int startAreaWidth = 2; // should be power of 2
    public static readonly int startAreaHeight = 2;

    public static readonly int ChunkWidth = 16; // Width of chunk in blocks
    public static readonly int ChunkHeight = 16; //Height of chunk in blocks  //IDEA: each chunk layer could unlock new thing, or n layers of chunks
    
    public static readonly int WorldWidthChunks = 128;  //TODO: update this
    public static readonly int WorldHeightChunks = 128; //TODO: update this

    public static readonly float PosPerlinScaling = 9.3f;
    public static readonly float oreTreshold = 0.7f; // used with perlin [-1,1] noise
    public static int WorldWidthInVoxels
    {
        get { return ChunkWidth * WorldWidthChunks; }
    }
    public static int WorldHeightInVoxels
    {
        get { return ChunkHeight * WorldHeightChunks; }
    }

    //texure atlas is n blocks square of same width blocks
    public static readonly int TextureAtlasSizeInBlocks = 4;
    public static float NormalizedBlockTextureSize
    {
        get { return 1f / (float)TextureAtlasSizeInBlocks; }
    }

    /// <summary>
    /// Vertecies of a 1x1x1 cube
    /// </summary>
    public static readonly Vector3[] voxelVerts = new Vector3[8] {

        new Vector3(0.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 0.0f, 1.0f),
        new Vector3(1.0f, 1.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 1.0f),

    };
    
    /// <summary>
    /// Directions of each face of a cube
    /// </summary>
    public static readonly Vector3[] faceCheckVectors = new Vector3[6] {

        new Vector3(0.0f, 0.0f, -1.0f),
        new Vector3(0.0f, 0.0f, 1.0f),
        new Vector3(0.0f, 1.0f, 0.0f),
        new Vector3(0.0f, -1.0f, 0.0f),
        new Vector3(-1.0f, 0.0f, 0.0f),
        new Vector3(1.0f, 0.0f, 0.0f)

    };

    /// <summary>
    /// nessecary triangles to make a cube
    /// </summary>
    public static readonly int[,] voxelTris = new int[6, 4] {

        // Back, Front, Top, Bottom, Left, Right

		// 0 1 2 2 1 3
		{0, 3, 1, 2}, // Back Face
		{5, 6, 4, 7}, // Front Face
		{3, 7, 2, 6}, // Top Face
		{1, 5, 0, 4}, // Bottom Face
		{4, 7, 0, 3}, // Left Face
		{1, 2, 5, 6} // Right Face

	};

    /// <summary>
    /// Uvs vertecies needed for a face
    /// </summary>
    public static readonly Vector2[] voxelUvs = new Vector2[4] {

        new Vector2 (0.0f, 0.0f),
        new Vector2 (0.0f, 1.0f),
        new Vector2 (1.0f, 0.0f),
        new Vector2 (1.0f, 1.0f)

    };


}