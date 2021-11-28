using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePath : MonoBehaviour
{
    public List<Vector3> points;
    public Color lineColor;
    public GameObject startSphere;
    public GameObject endSphere;

    LineRenderer line;
    // Start is called before the first frame update
    void Awake()
    {
        line = GetComponent<LineRenderer>();
        points = new List<Vector3>();
        line.enabled = false;
    }

    public void Show()
    {
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
        line.enabled = true;

        var startPoint = Instantiate(startSphere, points[0], Quaternion.identity, transform);
        var endPoint = Instantiate(endSphere, points[points.Count - 1], Quaternion.identity, transform);
        
    }

    public void ShowOff()
    { 
        line.enabled = false;
    }

}
