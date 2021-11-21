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
        if (isTest)
        {
            Load();
            Debug.Log(trajectories.Count);
            for (int i = 0; i < trajectories.Count; i++)
            {
                DrawLines(i);
            }
            //DrawLines(index);
        }
        
    }


    public void AddPath(float[][] path)
    {
        paths.Add(path);
        if (!isTest)
        {
            Save();
        }
    }

    public void Save()
    {
        FileManager.SavePaths(Data, tg);
    }

    public void Load()
    {
        trajectories = FileManager.LoadPaths(tg).Select(traj => new Trajectory(traj.Select(pt => new Vector3(pt[0], pt[1], pt[2])).ToList())).ToList();
    }

    public void DrawLines(int index)
    {
        var instanceLiner = Instantiate(liner);
        var linePath = instanceLiner.GetComponent<LinePath>();
        linePath.points = trajectories[index].path;
        linePath.Show();
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


