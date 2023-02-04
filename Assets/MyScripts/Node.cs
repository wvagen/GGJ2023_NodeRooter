using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public GameObject node;
    public LineRenderer myLine;
    public TrailRenderer myTrail;
    public Manager gameMan;

    [SerializeField] private float lineThickness = 0.2f;

    Rigidbody2D myRig;

    bool isClicked = false;
    const byte margin = 2;

    private void Start()
    {
        if (gameMan.isGameStarted)
        {
            Init();
        }
    }

    void Init()
    {
        myLine.startWidth = lineThickness;
        myLine.endWidth = lineThickness;

        myRig = GetComponent<Rigidbody2D>();
        myRig.velocity = Vector2.down * gameMan.nodeSpeed;
    }

    void OnMouseDown()
    {
        if (!isClicked)
        {
            isClicked = true;

            if (gameMan.isGameStarted)
                Split_Node();
            else
                Start_Node();
        }
    }

    void Start_Node()
    {
        Vector3 rightNode = transform.position;

        GameObject tempNode = Instantiate(node, rightNode, Quaternion.identity);
        tempNode.GetComponent<Manager>();
        gameMan.Set_Node_To_Follow(tempNode.GetComponent<Node>());

        myLine.startWidth = lineThickness;
        myLine.endWidth = lineThickness;

        myRig = tempNode.GetComponent<Rigidbody2D>();
        myRig.velocity = Vector2.down * gameMan.nodeSpeed;

        gameMan.isGameStarted = true;
    }

    void Split_Node()
    {
        Vector3 rightNode = transform.position + Vector3.right * margin;
        Vector3 leftNode = transform.position - Vector3.right * margin;

        myRig.velocity = Vector2.zero;

        //rightNode.y -= margin;
        //leftNode.y -= margin;

        GameObject tempNode = Instantiate(node, rightNode, Quaternion.identity);
        myLine.positionCount = 2;
        myLine.SetPosition(0, transform.position);
        myLine.SetPosition(1, tempNode.transform.position);
        tempNode.GetComponent<Manager>();
        gameMan.Set_Node_To_Follow(tempNode.GetComponent<Node>());

        GameObject tempNode2 = Instantiate(node, leftNode, Quaternion.identity);
        myLine.positionCount += 2;
        myLine.SetPosition(2, transform.position);
        myLine.SetPosition(3, tempNode2.transform.position);
        tempNode2.GetComponent<Manager>();


    }


}
