using MessagePack;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[MessagePackObject(keyAsPropertyName: true)]
[System.Serializable] public class SaveGameVector3Int
{
    public int x;
    public int y;
    public int z;
    
    public SaveGameVector3Int()
    {
        x = 0; y = 0; z = 0;
    }

    public SaveGameVector3Int(Vector3Int v3)
    {
        x = v3.x; y = v3.y; z = v3.z;
    }

    public Vector3Int GetVector3Int()
    {
        return new Vector3Int(x, y, z);
    }

}