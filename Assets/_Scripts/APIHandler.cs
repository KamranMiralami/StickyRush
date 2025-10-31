using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LevelResults
{
    public float final_score;
    public int tinyRewards;
    public float time;
    public int spikes_removed;
    public bool playerQuit;
    public int moves;
    public bool win;
    public bool isMlAgent;
    public int Q1;
    public int Q2;
    public int Q3;
    public int Q4;
    public int Q5;
    public int Q6;
    public int Q7;
    public int Q8;
    public int Q9;
    public int Q10;
    public int Q11;
    public int levelNumber;
    public LevelResults() { }
    public LevelResults(int final_score, int tinyRewards, float time, int spikes_removed, 
        bool playerQuit, int moves, bool win, bool isMlAgent, int levelNumber)
    {
        this.final_score = final_score;
        this.tinyRewards = tinyRewards;
        this.time = time;
        this.spikes_removed = spikes_removed;
        this.playerQuit = playerQuit;
        this.moves = moves;
        this.win = win;
        this.isMlAgent = isMlAgent;
        this.levelNumber = levelNumber;
    }
}
public class APIHandler : SingletonBehaviour<APIHandler>
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    [ContextMenu("Test Sending Data")]
    public void TestSendingData()
    {
        LevelResults test = new(
            final_score : 66,
            tinyRewards : 17,
            time : 356.7f,
            spikes_removed : 3,
            playerQuit : true,
            moves : 812,
            win : true,
            isMlAgent : false,
            levelNumber : 2
            );
        StartCoroutine(SendLevelResults(test));
    }
    public void SendData(LevelResults payload)
    {
        StartCoroutine(SendLevelResults(payload));
    }
    IEnumerator SendLevelResults(LevelResults payload)
    {
        string url = "https://djangodatabase-1.onrender.com/api/logs/";
        string jsonData = JsonUtility.ToJson(payload);
        using UnityWebRequest req = new(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        req.uploadHandler = new UploadHandlerRaw(bodyRaw);
        req.downloadHandler = new DownloadHandlerBuffer();
        req.SetRequestHeader("Content-Type", "application/json");

        yield return req.SendWebRequest();

        if (req.result == UnityWebRequest.Result.Success)
            Debug.Log("Response: " + req.downloadHandler.text);
        else
            Debug.LogError("Error: " + req.error);
    }
}
