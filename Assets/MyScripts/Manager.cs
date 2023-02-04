using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public float nodeSpeed = 5;
    public bool isGameStarted = false;

    Camera myCam;
    Node followedNode;

    private void Start()
    {
        myCam = Camera.main;
        followedNode = FindObjectOfType<Node>();
    }

    private void Update()
    {
        Cam_Follow();
    }

    void Cam_Follow()
    {
        Vector3 nodeFol = Vector2.zero;
        nodeFol.y = followedNode.transform.position.y;
        nodeFol.z = -10;
        myCam.transform.position = nodeFol;
    }

    public void Set_Node_To_Follow(Node followedNode)
    {
        this.followedNode = followedNode;
    }
}
