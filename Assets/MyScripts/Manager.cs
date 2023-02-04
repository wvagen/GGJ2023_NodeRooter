using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    public MusicManager musicMan;

    public Text sceneStat;
    public GameObject node;

    public float nodeSpeed = 5;
    public float nodeShrinkSpeed = 5;

    public bool isGameStarted = false;

    Camera myCam;
    Node followedNode;

    Dictionary<int,Node> spawnedNodes = new Dictionary<int, Node>();
    public List<Node> duplicatedNodes = new List<Node>();

    private void Start()
    {
        myCam = Camera.main;
        followedNode = FindObjectOfType<Node>();

        if (MusicManager.isExperimentalScene) sceneStat.text = "Lab Scene";
        else sceneStat.text = "";
    }

    public void Can_Start()
    {
        musicMan.Can_Start();
        isGameStarted = true;
    }

    public void Insert_Node(int nodeIndex, Node newNode)
    {
        duplicatedNodes.Add(newNode);
        spawnedNodes[nodeIndex] = newNode;
    }

    public void Empty_Slot(int nodeIndex)
    {
        spawnedNodes.Remove(nodeIndex);

    }

    public void Bit_Node(int nodeIndex)
    {
        spawnedNodes[nodeIndex].Bit_Me();
    }

    public void Set_Node_Bit(int nodeIndex)
    {
        if (MusicManager.isExperimentalScene)
        {
            musicMan.Mark_Bit(nodeIndex);
        }
    }

    public bool Is_Node_Exists(int nodeIndex)
    {
        return spawnedNodes.ContainsKey(nodeIndex);
    }

    private void Update()
    {
        Cam_Follow();
        Restart_Scene_Trigger();
        //Log_Dictionnary();
    }

    void Log_Dictionnary()
    {
        foreach (KeyValuePair<int, Node> entry in spawnedNodes)
        {
            Debug.Log(entry.Key + " : " + entry.Value);
            // do something with entry.Value or entry.Key
        }
        
    }

    void Restart_Scene_Trigger()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            MusicManager.isExperimentalScene = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            MusicManager.isExperimentalScene = true;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    void Cam_Follow()
    {
        Vector3 nodeFol = Vector2.zero;
        nodeFol.y = followedNode.transform.position.y;
        nodeFol.z = -10;
        myCam.transform.position = nodeFol;
    }

    public void Set_Node_To_Follow(Node followedNode,int nodeIndex)
    {
        this.followedNode = followedNode;
        followedNode.myIndex = nodeIndex;
    }

    public void Stop_All_Duplicated_Nodes(Node node)
    {
        for (int i = 0; i < duplicatedNodes.Count; i++)
        {
            if (duplicatedNodes[i].myIndex == node.myIndex)
            {
                duplicatedNodes[i].Stop_Node();
            }
        }
        //duplicatedNodes.Remove(node);
    }
}
