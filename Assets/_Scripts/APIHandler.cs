using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class LevelResults
{
    public int final_score;
    public int rewards;
    public float time;
    public int spikes_removed;
    public float pause_time;
    public int moves;
    public bool win;
    public bool isMlAgent;
    public int Q1;
    public int Q2;
    public int Q3;

    public LevelResults(int final_score, int rewards, float time, int spikes_removed, 
        float pause_time, int moves, bool win, bool isMlAgent, int Q1, int Q2, int Q3)
    {
        this.final_score = final_score;
        this.rewards = rewards;
        this.time = time;
        this.spikes_removed = spikes_removed;
        this.pause_time = pause_time;
        this.moves = moves;
        this.win = win;
        this.isMlAgent = isMlAgent;
        this.Q1 = Q1;
        this.Q2 = Q2;
        this.Q3 = Q3;
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
            rewards : 17,
            time : 356.7f,
            spikes_removed : 3,
            pause_time : 24.2f,
            moves : 812,
            win : true,
            isMlAgent : false,
            Q1 : 4,
            Q2 : 5,
            Q3 : 3
            );
        StartCoroutine(SendLevelResults(test));
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
