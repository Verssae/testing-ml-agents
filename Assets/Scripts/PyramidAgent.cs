using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PyramidAgent : Agent
{
    public GameObject area;
    PyramidArea m_MyArea;
    Rigidbody m_AgentRb;
    PyramidSwitch m_SwitchLogic;
    public GameObject areaSwitch;
    public bool useVectorObs;

    [Header("Run Setting")]
    public float agentRunSpeed;

    [Header("Jump Setting")]
    public float agentJumpHeight;
    public float agentJumpVelocity;
    public float agentJumpVelocityMaxChange;

    public float jumpingTime;
    
    // This is a downward force applied when falling to make jumps look
    // less floaty
    public float fallingForce;

    // Use to check the coliding objects
    public Collider[] hitGroundColliders = new Collider[5];

    [Header("Trajectory Setting")]
    public float minDist = 0.1f;
    public bool record = true;

    Line path;
    //Tape goals;
    Tape bounds;

    Vector3 m_JumpTargetPos;
    Vector3 m_JumpStartingPos;


    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_MyArea = area.GetComponent<PyramidArea>();
        m_SwitchLogic = areaSwitch.GetComponent<PyramidSwitch>();

        path = new Line(minDist);
        //goals = GameObject.Find("Goals").GetComponent<Tape>();
        bounds = GameObject.Find("Bounds").GetComponent<Tape>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(m_SwitchLogic.GetState());
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
            sensor.AddObservation(DoGroundCheck());
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        var jumpAction = act[1];

        var isGrounded = DoGroundCheck();

        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }

        if (jumpAction == 1)
            if ((jumpingTime <= 0f) && isGrounded)
            {
                Jump();
            }
        
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * agentRunSpeed, ForceMode.VelocityChange);

        if (jumpingTime > 0f)
        {
            m_JumpTargetPos =
                new Vector3(m_AgentRb.position.x,
                    m_JumpStartingPos.y + agentJumpHeight,
                    m_AgentRb.position.z) + dirToGo;
            MoveTowards(m_JumpTargetPos, m_AgentRb, agentJumpVelocity,
                agentJumpVelocityMaxChange);
        }

        if (!(jumpingTime > 0f) && !isGrounded)
        {
            m_AgentRb.AddForce(
                Vector3.down * fallingForce, ForceMode.Acceleration);
        }
        jumpingTime -= Time.fixedDeltaTime;
    }
    public void Jump()
    {
        jumpingTime = 0.2f;
        m_JumpStartingPos = m_AgentRb.position;
    }

    void MoveTowards(
        Vector3 targetPos, Rigidbody rb, float targetVel, float maxVel)
    {
        var moveToPos = targetPos - rb.worldCenterOfMass;
        var velocityTarget = Time.fixedDeltaTime * targetVel * moveToPos;
        if (float.IsNaN(velocityTarget.x) == false)
        {
            rb.velocity = Vector3.MoveTowards(
                rb.velocity, velocityTarget, maxVel);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
        path.Add(transform.localPosition);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }

        discreteActionsOut[1] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    public override void OnEpisodeBegin()
    {
        var enumerable = Enumerable.Range(0, 8).OrderBy(x => Guid.NewGuid()).Take(8);
        var items = enumerable.ToArray();

        m_MyArea.CleanPyramidArea();
        m_AgentRb.velocity = Vector3.zero;
        m_MyArea.PlaceObject(gameObject, items[0]);
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        m_SwitchLogic.ResetSwitch(items[1], items[2]);
        path.Clear();

    }

    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("goal"))
        {
            SetReward(2f);
            //if (record)
            //{
            //    goals.lines.Add(path);
            //    goals.Save();
            //}
            
            EndEpisode();
        }
        
        if (collision.gameObject.CompareTag("switchOff"))
        {
            SetReward(1f);
        }

        if (collision.gameObject.CompareTag("bound"))
        {
            if (record)
            {
                bounds.lines.Add(path);
                bounds.Save();
            }
            
            EndEpisode();
        }
    }

    public bool DoGroundCheck()
    {
        hitGroundColliders = new Collider[5];
        var o = gameObject;
        Physics.OverlapBoxNonAlloc(
            o.transform.position + new Vector3(0, -0.05f * 0.25f, 0),
            new Vector3(0.95f / 2f * 2f, 0.5f * 2.5f, 0.95f / 2f * 2f),
            hitGroundColliders,
            o.transform.rotation);
        var grounded = false;
        foreach (var col in hitGroundColliders)
        {
            if (col != null && col.transform != transform &&
                (col.CompareTag("elevator") ||
                 col.CompareTag("ground") ||
                 col.CompareTag("stone")))
            {
                grounded = true; //then we're grounded
                break;
            }
        }
        return grounded;
    }
}
