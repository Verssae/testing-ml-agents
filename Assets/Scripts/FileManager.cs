using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class FileManager
{
   public static void SavePaths (float[][][] paths, string tag)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/trajectories." + tag;
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, paths);
        stream.Close();
    }

    public static float[][][] LoadPaths(string tag)
    {
        string path = Application.dataPath + "/trajectories." + tag;
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            float[][][] paths = formatter.Deserialize(stream) as float[][][];
            stream.Close();

            return paths;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
