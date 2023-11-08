using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BigChunk : MonoBehaviour
{
    public readonly int _bigChunkWidth;
    public readonly int _bigChunkHeight;

    public Chunk[,,] chunks;

    public bool isLoaded = false; // maybe change name of this to something more accurate.


    



    //TODO: needs a thing to render a face on the side facing a loaded chunk

    BigChunk(int bigChunkWidth, int bigChunkHeight)
    {
        _bigChunkWidth = bigChunkWidth;
        _bigChunkHeight = bigChunkHeight;

        chunks = new Chunk[bigChunkWidth, bigChunkHeight, bigChunkWidth];


    }













}
