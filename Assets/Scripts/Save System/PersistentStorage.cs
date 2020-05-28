using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    BinaryWriter writer;

    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Write(int value)
    {
        writer.Write(value);
    }

}

public class GameDataReader
{
    BinaryReader reader;

    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }
}


public class PersistentStorage : MonoBehaviour
{
    string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "saveFile");
    }

    public void SaveLevel(LevelGenerator o)
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            o.Save(new GameDataWriter(writer));
        }
    }

    public int[,] LoadLevel(LevelGenerator o)
    {
        byte[] data = File.ReadAllBytes(savePath);

        BinaryReader reader = new BinaryReader(new MemoryStream(data));
        return o.Load(new GameDataReader(reader));
    }
}
