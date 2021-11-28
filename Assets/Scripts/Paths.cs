using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;

public class Paths : MonoBehaviour
{
    public List<float[][]> paths;
    public List<Trajectory> trajectories;
    public GameObject liner;
    public bool isTest;
    public bool showAll;
    public string tg;

    int index;

    public int inputIndex;
    
    public int Index
    {
        get
        {
            return index;
        }

        set
        {
            if (index == value) return;
            index = value;
            DrawLines(index);
        }
    }

    private void Update()
    {
        Index = inputIndex; 
    }

    public float[][][] Data
    {
        get
        {
            return paths.ToArray();
        }
    }

    private void Awake()
    {
        paths = new List<float[][]>();
        trajectories = new List<Trajectory>();
    }

    private void Start()
    {
        Load();
        Debug.Log(trajectories.Count);
        if (isTest && showAll)
        {
            
            
            for (int i = 0; i < trajectories.Count; i++)
            {
                DrawLines(i);
            }
            
        }
        else if (isTest && !showAll)
        {
            DrawLines(Index);
        }
    }


    public void AddPath(float[][] path)
    {
        
        if (!isTest)
        {
            paths.Add(path);
            if (paths.Count % 5 == 0)
            {
                Save();
            }
        }
    }

    public void Save()
    {
        Debug.Log($"{tg}: {Data.Length}");
        FileManager.SavePaths(Data, tg);
        
    }

    public void Load()
    {
        paths = FileManager.LoadPaths(tg).ToList();
        trajectories = FileManager.LoadPaths(tg).Select(traj => new Trajectory(traj.Select(pt => new Vector3(pt[0], pt[1], pt[2])).ToList())).ToList();
    }

    public void DrawLines(int index)
    {
        var instanceLiner = Instantiate(liner);
        var linePath = instanceLiner.GetComponent<LinePath>();
        if (trajectories.Count > 0)
        {
            linePath.points = trajectories[index].path;
            linePath.Show();
        }
    }

}

[System.Serializable]
public class Trajectory
{
    public List<Vector3> path;
    public Trajectory(List<Vector3> v)
    {
        path = v;
    }
}


