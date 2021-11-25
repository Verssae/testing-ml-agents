using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : MonoBehaviour
{
    // Start is called before the first frame update
    bool m_State;
    GameObject m_Area;
    PyramidArea m_AreaComponent;
    int m_PyramidIndex;

    public bool GetState()
    {
        return m_State;
    }

    void Start()
    {
        m_Area = gameObject.transform.parent.gameObject;
        m_AreaComponent = m_Area.GetComponent<PyramidArea>();
    }

    public void ResetSwitch(int spawnAreaIndex, int pyramidSpawnIndex)
    {
        m_AreaComponent.PlaceObject(gameObject, spawnAreaIndex);
        m_State = false;
        m_PyramidIndex = pyramidSpawnIndex;
        tag = "switchOff";
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    // Update is called once per frame
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("agent") && m_State == false)
        {
            m_State = true;
            m_AreaComponent.CreatePyramid(1, m_PyramidIndex);
            tag = "switchOn";
            
        }
    }
}
