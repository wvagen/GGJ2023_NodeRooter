using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class MusicManager : MonoBehaviour
{
    public Manager gameMan;
    public TextAsset mySyncMomentsTxt;

    public List<float> timeMoments = new List<float>();
    public List<int> nodesIndexes = new List<int>();

    private float timeValue;
    private short timerCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        timeMoments = GetTimeMoments();
    }

    // Update is called once per frame
    void Update()
    {
        Sync();
    }

    void Sync()
    {
        timeValue += Time.fixedDeltaTime;
        if (timeValue >= timeMoments[timerCount])
        {
            timerCount++;
            Next_Bit();
        }
    }

    void Next_Bit()
    {
        gameMan.Bit_Node(timerCount);
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
