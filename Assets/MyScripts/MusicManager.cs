using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class MusicManager : MonoBehaviour
{
    public Manager gameMan;
    public TextAsset mySyncMomentsTxt;

    public AudioSource audioSource;
    public AudioClip myAudioClip;

    public List<float> timeMoments = new List<float>();
    public List<int> nodesIndexes = new List<int>();

    public static bool isExperimentalScene = false;

    [TextArea(3, 10)]
    public string syncMoments = "";

    private float timeValue;
    private short timerCount = 0;
    bool canStart = false;
    // Start is called before the first frame update

    public void Can_Start()
    {
        audioSource.PlayOneShot(myAudioClip);
        canStart = true;
    }

    void Start()
    {
        if (isExperimentalScene)
        {
            syncMoments = "";
        }
        else
        {
            timeMoments = GetTimeMoments();
        }

    }

    // Update is called once per frame
    void Update()
    {
        Sync();
    }

    void Sync()
    {
        if (canStart && !isExperimentalScene)
        {
            timeValue += Time.deltaTime;
            if (timeMoments.Count - 1 > timerCount && timeValue >= timeMoments[timerCount])
            {
                Next_Bit();
                timerCount++;
            }
        }else if (canStart)
        {
            timeValue += Time.deltaTime;
            if (!audioSource.isPlaying)
            {
                Debug.Log("Music Saved!");
                canStart = false;
                WriteString();
            }
        }
    }

    void WriteString()
    {
        string filePath = Application.dataPath + "/MySyncs/Aref_Syncs/0_aref.txt";
        Debug.Log("PAth " + filePath);
        File.WriteAllText(filePath, syncMoments);
    }

    void Next_Bit()
    {
        gameMan.Bit_Node(nodesIndexes[timerCount]);
    }

    public void Mark_Bit(int nodeIndex)
    {
        Debug.Log("HELLO!!!!!");
        syncMoments += timeValue.ToString() + " " + nodeIndex.ToString() + "\n";
    }


    private List<float> GetTimeMoments()
    {
        //string path = "Assets/words.txt";
        StreamReader reader = new StreamReader(new MemoryStream(mySyncMomentsTxt.bytes));
        List<float> timeMoments = new List<float>();

        string line = "";
        do
        {
            line = reader.ReadLine();
            if (line != null)
            {
                timeMoments.Add(float.Parse(line.Split(' ')[0].ToString()));
                nodesIndexes.Add(int.Parse(line.Split(' ')[1].ToString()));
            }
        } while (line != null);

        return (timeMoments);
    }
}
