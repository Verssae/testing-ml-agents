using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public float distance = 5f;
    public float speed = 2f;
    public float haltTime = 3f;

    public GameObject map;

    Vector3 bottomPos, topPos;
    bool isMoving = false;
    bool isMovingDown = false;
    float haltingTime = 0f;
    void Start()
    {
        bottomPos = transform.position;
        topPos = bottomPos + Vector3.up * distance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoving)
        {
            if (isMovingDown)
                if (transform.position != bottomPos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, bottomPos, speed * Time.deltaTime);
                }
                else
                {
                    isMoving = false;
                    haltingTime = haltTime;
                }
            else
                if (transform.position != topPos)
                {
                    transform.position = Vector3.MoveTowards(transform.position, topPos, speed * Time.deltaTime);
                }
                else
                {
                    isMoving = false;
                    haltingTime = haltTime;
                }
        }
        else
        {
            if (haltingTime <= 0)
            {
                isMoving = true;
                if (transform.position == bottomPos)
                {
                    isMovingDown = false;
                }
                else if (transform.position == topPos) 
                {
                    isMovingDown = true;
                }
                else
                {
                    Debug.Log("Not accurate!");
                }
            }
            haltingTime -= Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            collision.transform.parent = transform;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            collision.transform.parent = map.transform;
        }
    }
}
