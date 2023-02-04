using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LineRenderer myLine;
    public TrailRenderer myTrail;
    public Manager gameMan;

    public GameObject myOutterCircle;

    public float outterCircleRadiusTarget = 0.075f;
    [SerializeField] private Color circleOutRangeCol,circleInRangeCol;

    public int myIndex = 0;

    [SerializeField] private float lineThickness = 0.2f;

    Rigidbody2D myRig;
    SpriteRenderer outterCircleSpriteRend;

    bool isClicked = false, isWinningClick = false;
    bool canShrinkOutterCircle = false;
    const byte margin = 2;

    private void Start()
    {
        if (gameMan.isGameStarted)
        {
            Init();
        }
    }

    private void Update()
    {
        Shrink_Outter_Circle();
    }

    void Shrink_Outter_Circle()
    {
        if (canShrinkOutterCircle)
        {
            if (myOutterCircle.transform.localScale.x > 0)
            {
                myOutterCircle.transform.localScale -= (Vector3.one * Time.deltaTime * gameMan.nodeShrinkSpeed);

                if (myOutterCircle.transform.localScale.x <= outterCircleRadiusTarget)
                {
                    outterCircleSpriteRend.color = circleInRangeCol;
                    isWinningClick = true;
                }
            }
            else
            {
                Debug.Log("You Loose");
                canShrinkOutterCircle = false;
                isWinningClick = false;
            }
        }
    }

    public void Bit_Me()
    {
        canShrinkOutterCircle = true;
        myOutterCircle.SetActive(true); 
        outterCircleSpriteRend = myOutterCircle.GetComponent<SpriteRenderer>();
        outterCircleSpriteRend.color = circleOutRangeCol;
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
            {
                Split_Node();
                gameMan.Set_Node_Bit(myIndex);
            }
            else
                Start_Node();
        }
    }

    void Start_Node()
    {
        Vector3 rightNode = transform.position;

        GameObject tempNode = Instantiate(gameMan.node, rightNode, Quaternion.identity);
        tempNode.GetComponent<Manager>();
        gameMan.Set_Node_To_Follow(tempNode.GetComponent<Node>(),0);
        gameMan.Insert_Node((int)rightNode.x, tempNode.GetComponent<Node>());
        tempNode.GetComponent<Node>().gameMan = gameMan;

        myLine.startWidth = lineThickness;
        myLine.endWidth = lineThickness;

        myRig = tempNode.GetComponent<Rigidbody2D>();
        myRig.velocity = Vector2.down * gameMan.nodeSpeed;

        gameMan.Can_Start();
    }

    void Split_Node()
    {
        Vector3 rightNode = transform.position + Vector3.right * margin;
        Vector3 leftNode = transform.position - Vector3.right * margin;

        Stop_Node();
        gameMan.Stop_All_Duplicated_Nodes(this);

        //rightNode.y -= margin;
        //leftNode.y -= margin;

        GameObject tempRightNode = Instantiate(gameMan.node, rightNode, Quaternion.identity); myLine.positionCount = 2;
        myLine.SetPosition(0, transform.position);
        myLine.SetPosition(1, tempRightNode.transform.position);

        Node tempNodeScript = tempRightNode.GetComponent<Node>();
        gameMan.Set_Node_To_Follow(tempNodeScript, (int)rightNode.x);
        gameMan.Insert_Node((int)rightNode.x, tempNodeScript);
        tempNodeScript.gameMan = gameMan;
        //tempNodeScript.Bit_Me();

        GameObject tempLeftNode = Instantiate(gameMan.node, leftNode, Quaternion.identity);
        myLine.positionCount += 2;
        myLine.SetPosition(2, transform.position);
        myLine.SetPosition(3, tempLeftNode.transform.position);

        tempNodeScript = tempLeftNode.GetComponent<Node>();
        gameMan.Set_Node_To_Follow(tempNodeScript, (int)leftNode.x);
        gameMan.Insert_Node((int)leftNode.x, tempNodeScript);
        tempNodeScript.gameMan = gameMan;
        //tempNodeScript.Bit_Me();
    }

    public void Stop_Node()
    {
        myRig.velocity = Vector2.zero;
        isClicked = true;
        gameMan.Empty_Slot((int)transform.position.x);
    }

}
