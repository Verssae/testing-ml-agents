using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class Tape : MonoBehaviour
{
    public GameObject linePath;

    public List<Line> lines = new List<Line>();
    readonly BinaryFormatter formatter = new BinaryFormatter();

    public Slider slider;


    List<GameObject> lineObjects = new List<GameObject>();

    public void Save()
    {
        float[][][] serialized = lines.Select(path => path.PointsF).ToArray();
        string name = $"{Application.dataPath}/{gameObject.name}.tape";

        FileStream stream = new FileStream(name, FileMode.Create);
        formatter.Serialize(stream, serialized);
        stream.Close();
    }

    public void Load()
    {
        string name = $"{Application.dataPath}/{gameObject.name}.tape";
        if (File.Exists(name))
        {
            FileStream stream = new FileStream(name, FileMode.Open);
            float[][][] deserialized = formatter.Deserialize(stream) as float[][][];
            lines = deserialized.ToList().Select(data => new Line(data)).ToList();
            stream.Close();
        }
    }

    public void Draw(int[] indices)
    {
        foreach(var idx in indices)
        {
            if (idx < lines.Count)
            {
                var lineObj = Instantiate(linePath);
                var path = lineObj.GetComponent<LinePath>();
                lineObjects.Add(lineObj);
                path.points = lines[idx].Points;
                path.Show();
            }
        }
    }

    void ClearLineObjs()
    {
        foreach(var obj in lineObjects)
        {
            Destroy(obj);
        }
        lineObjects.Clear();
    }

    void Start()
    {
        Load();
        Debug.Log($"Load {lines.Count} {gameObject.name} paths");
    }

    void Update()
    {
        //if (lines.Count > 0 && (lines.Count % saveFrequency == 0) && notSaved)
        //{
        //    Save();
        //    notSaved = false;
        //    Debug.Log($"Save {lines.Count} {gameObject.name} paths");
        //} 
        //else
        //{
        //    notSaved = true;
        //}
    }

    public void DrawLines()
    {
        ClearLineObjs();
        var indices = Enumerable.Range(0, Mathf.RoundToInt(lines.Count * slider.value)).ToArray();
        foreach(var idx in indices)
        {
            Debug.Log(idx);
        }
        Draw(indices);
    }

}

public class Line
{
    List<Vector3> points;
    readonly float minDist;

    public Line(float minDist)
    {
        points = new List<Vector3>();
        this.minDist = minDist;
    }

    public Line(List<Vector3> points, float minDist=0.1f)
    {
        this.points = points;
        this.minDist = minDist;
    }

    public Line(float[][] points, float minDist=0.1f)
    {
        this.points = points.Select(pt => new Vector3(pt[0], pt[1], pt[2])).ToList();
        this.minDist = minDist;
    }



    public void Add(Vector3 point)
    {
        if (points.Count > 0 && (point - Last()).magnitude < minDist)
        {
            return;
        }
        points.Add(point);
    }

    public void Clear()
    {
        points.Clear();
    }

    public float[][] PointsF
    {
        get
        {
            return points.Select(pt => Vector3ToFloats(pt)).ToArray();
        }
    }

    public List<Vector3> Points
    {
        get
        {
            return points;
        }
    }

    Vector3 Last()
    {
        if (points == null)
        {
            return Vector3.zero;
        }
        return points[points.Count - 1];
    }

    float[] Vector3ToFloats(Vector3 pt)
    {
        return Enumerable.Range(0, 3).Select(i => pt[i]).ToArray();
    }
}