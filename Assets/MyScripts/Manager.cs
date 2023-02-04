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

    bool canZoomOut = false;

    const float zoomOutRange = 0.5f;

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
        Zoom_Logic(nodeIndex);
    }

    void Zoom_Logic(int nodeIndex)
    {
        int minKey = 0, maxKey = 0;
        Debug.Log("Zoom Logic !");

        foreach (KeyValuePair<int, Node> entry in spawnedNodes)
        {
            if (minKey > entry.Value.myIndex) minKey = entry.Value.myIndex;
            if (maxKey < entry.Value.myIndex) maxKey = entry.Value.myIndex;
            // do something with entry.Value or entry.Key
        }

        Debug.Log("Min Key Out ! " + minKey);
        Debug.Log("Max Key Out ! " + maxKey);

        if (minKey == spawnedNodes[nodeIndex].myIndex || spawnedNodes[nodeIndex].myIndex == maxKey)
        {
            Debug.Log("Zoom Out !");
            StartCoroutine(Zoom_Out_Cam());
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

    IEnumerator Zoom_Out_Cam()
    {
        float targetSize = myCam.orthographicSize + zoomOutRange;
        while (myCam.orthographicSize < targetSize)
        {
            myCam.orthographicSize = Mathf.Lerp(myCam.orthographicSize, targetSize, Time.deltaTime * 5);
            yield return new WaitForEndOfFrame();
        }

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
