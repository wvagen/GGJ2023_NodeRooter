using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LineRenderer myLine;
    public TrailRenderer myTrail;
    public Manager gameMan;

    public GameObject myOutterCircle;

    public float outterCircleRadiusTarget = 0.09f;
    [SerializeField] private Color circleOutRangeCol, circleInRangeCol;

    public int myIndex = 0;

    [SerializeField] private float lineThickness = 0.1f;

    Rigidbody2D myRig;

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
            if (myOutterCircle.transform.localScale.x > 0.05f)
            {
                myOutterCircle.transform.localScale -= (Vector3.one * Time.deltaTime * gameMan.nodeShrinkSpeed);

                if (myOutterCircle.transform.localScale.x <= outterCircleRadiusTarget)
                {
                    myOutterCircle.GetComponent<SpriteRenderer>().color = circleInRangeCol;
                    isWinningClick = true;
                }
                else
                {
                    isWinningClick = false;
                }
            }
            else if (!isClicked)
            {
                LoseBehavior();
            }
        }
    }

    void LoseBehavior()
    {
        Debug.Log("You Loose");
        GetComponent<SpriteRenderer>().color = circleOutRangeCol;
        myOutterCircle.GetComponent<SpriteRenderer>().color = circleOutRangeCol;
        gameMan.Loose();
        canShrinkOutterCircle = false;
        isWinningClick = false;
    }

    public void Bit_Me()
    {
        canShrinkOutterCircle = true;
        isWinningClick = true;
        myOutterCircle.SetActive(true);
        myOutterCircle.GetComponent<SpriteRenderer>().color = circleOutRangeCol;
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
                if (isWinningClick)
                {
                    gameMan.Set_Node_Bit(myIndex);

                    if (myOutterCircle.transform.localScale.x > 0.08f)
                    {
                        gameMan.MotivationalWords(0);
                    }
                    else if (myOutterCircle.transform.localScale.x >= 0.072f && myOutterCircle.transform.localScale.x <= 0.077f)
                    {
                        gameMan.MotivationalWords(2);
                    }
                    else
                    {
                        gameMan.MotivationalWords(1);
                    }
                    
                    Split_Node();
                }
                else
                {
                   LoseBehavior();
                }
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
        gameMan.Set_Node_To_Follow(tempNode.GetComponent<Node>(), 0);
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
        gameMan.Insert_Node(Mathf.RoundToInt(rightNode.x), tempNodeScript);
        tempNodeScript.gameMan = gameMan;
        //tempNodeScript.Bit_Me();

        GameObject tempLeftNode = Instantiate(gameMan.node, leftNode, Quaternion.identity);
        myLine.positionCount += 2;
        myLine.SetPosition(2, transform.position);
        myLine.SetPosition(3, tempLeftNode.transform.position);

        tempNodeScript = tempLeftNode.GetComponent<Node>();
        gameMan.Set_Node_To_Follow(tempNodeScript, (int)leftNode.x);
        gameMan.Insert_Node(Mathf.RoundToInt(leftNode.x), tempNodeScript);
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
