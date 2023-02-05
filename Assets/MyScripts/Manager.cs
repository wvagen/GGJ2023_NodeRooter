using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class Manager : MonoBehaviour
{
    public MusicManager musicMan;
    public RawImage rawImg;

    public TextAsset[] textSyncMoments;
    public AudioClip[] musicList;

    public GameObject rightArrowBtn, leftArrowBtn;
    public TextMeshProUGUI levelTxt, bestTxt;
    int levelIndex = 0;

    public Animator myAnim;
    public Animator motivationalAnim;

    public GameObject gameOverMan, winPanel;

    public TextMeshProUGUI scoreTxt;

    public TextMeshProUGUI bestMarkTxt, currentmarkTxt;

    public Text sceneStat;
    public GameObject node;

    public float nodeSpeed = 5;
    public float nodeShrinkSpeed = 5;

    public bool isGameStarted = false;

    public bool isGameOver = false;

    string[] marks = {"A", "A+", "S" };

    Camera myCam;
    Node followedNode;

    Dictionary<int,Node> spawnedNodes = new Dictionary<int, Node>();
    public List<Node> duplicatedNodes = new List<Node>();

    const float zoomOutRange = 0.5f;

    const short noramlHitScore = 50;
    const short goodHitScore = 70;
    const short perfectHitScore = 100;

    int score = 0;

    private void Start()
    {
        myCam = Camera.main;
        followedNode = FindObjectOfType<Node>();
        scoreTxt.gameObject.SetActive(false);
        Game_Level_Behavior();

        if (MusicManager.isExperimentalScene) sceneStat.text = "Lab Scene";
        else sceneStat.text = "";
    }

    public void Can_Start()
    {
        musicMan.Can_Start();
        myAnim.Play("StartGame");
        scoreTxt.gameObject.SetActive(true);
        isGameStarted = true;
    }

    public void Insert_Node(int nodeIndex, Node newNode)
    {
        duplicatedNodes.Add(newNode);
        spawnedNodes[nodeIndex] = newNode;
    }

    public void RetryBtn()
    {
        MusicManager.isExperimentalScene = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Switch_Level(bool isRightBtn)
    {
        if (isRightBtn)
        {
            levelIndex++;
        }
        else
        {
            levelIndex--;
        }
        Game_Level_Behavior();
    }

    void Game_Level_Behavior()
    {
        leftArrowBtn.SetActive(true);
        rightArrowBtn.SetActive(true);

        if (levelIndex == 0) leftArrowBtn.SetActive(false);
        if (levelIndex == textSyncMoments.Length - 1) rightArrowBtn.SetActive(false);

        if (!PlayerPrefs.HasKey(levelIndex + "bestMark")) rightArrowBtn.SetActive(false);

        levelTxt.text = "Level: " + (levelIndex + 1).ToString();
        bestTxt.text = "Best: " + (PlayerPrefs.HasKey(levelIndex + "bestMark") ? marks[PlayerPrefs.GetInt(levelIndex + "bestMark")] : "N/A");

        musicMan.mySyncMomentsTxt = textSyncMoments[levelIndex];
        musicMan.myAudioClip = musicList[levelIndex];
        musicMan.Set_Sync();

    }

    public void HomeBtn()
    {
        MusicManager.isExperimentalScene = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void Empty_Slot(int nodeIndex)
    {
        //spawnedNodes.Remove(nodeIndex);
    }

    public void Bit_Node(int nodeIndex)
    {
        //spawnedNodes[nodeIndex].Bit_Me();
        foreach(Node node in duplicatedNodes)
        {
            if (node.myIndex == nodeIndex) node.Bit_Me();
        }
    }

    public void Loose()
    {
        musicMan.audioSource.pitch = 0.8f;
        isGameOver = true;
        gameOverMan.SetActive(true);
    }

    public void MotivationalWords(byte choice)
    {
        switch (choice)
        {
            case 1: motivationalAnim.Play("GoodJobAnim", -1, 0); score += goodHitScore; break;
            case 2: motivationalAnim.Play("PerfectAnim", -1, 0); score += perfectHitScore; break;
            default:
                score += noramlHitScore;break;
        }
        Update_UI();
    }

    void Update_UI()
    {
        scoreTxt.text = score.ToString();
        myAnim.Play("ScoreBump",0,0);
    } 

    public void Win()
    {
        winPanel.SetActive(true);
        isGameOver = true;
        Affect_Mark();
        StartCoroutine(Final_Zoom_Cam());
    }

    void Affect_Mark()
    {
        int markIndex = 0,bestMarkIndex = 0;
        if (score - 100 >= perfectHitScore * musicMan.timeMoments.Count)
        {
            markIndex = marks.Length - 1;
        }else if (score - 100 >= goodHitScore * musicMan.timeMoments.Count)
        {
            markIndex = marks.Length - 2;
        }
        else
        {
            markIndex = marks.Length - 3;
        }
        if (PlayerPrefs.HasKey(levelIndex + "bestMark"))
        {
            bestMarkIndex = PlayerPrefs.GetInt(levelIndex + "bestMark", 0);
        }

        if (bestMarkIndex <= markIndex)
        {
            bestMarkIndex = markIndex;
            PlayerPrefs.SetInt(levelIndex + "bestMark", bestMarkIndex);
        }

        bestMarkTxt.text = marks[bestMarkIndex];
        currentmarkTxt.text = marks[markIndex];

    }

    IEnumerator Final_Zoom_Cam()
    {
        Vector3 targetPos = myCam.transform.position;
        targetPos.y = -15;
        while (myCam.orthographicSize < 30)
        {
            myCam.orthographicSize += Time.deltaTime * 15;
            yield return new WaitForEndOfFrame();
        }
        //myCam.transform.position = targetPos;
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

        foreach (KeyValuePair<int, Node> entry in spawnedNodes)
        {
            if (minKey > entry.Value.myIndex) minKey = entry.Value.myIndex;
            if (maxKey < entry.Value.myIndex) maxKey = entry.Value.myIndex;
            // do something with entry.Value or entry.Key
        }

        if (minKey == spawnedNodes[nodeIndex].myIndex || spawnedNodes[nodeIndex].myIndex == maxKey)
        {
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
        Loopable_BG();
        Pause();
        //Log_Dictionnary();
    }

    private void Pause()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            Debug.Break();

    }

    void Loopable_BG()
    {
        if (isGameStarted)
            rawImg.uvRect = new Rect(rawImg.uvRect.x + Time.deltaTime * 0.01f, 0, 1, 1);
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
        if (!isGameOver)
        {
            Vector3 nodeFol = Vector2.zero;
            nodeFol.y = followedNode.transform.position.y;
            nodeFol.z = -10;
            myCam.transform.position = nodeFol;
        }
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
    }
}
