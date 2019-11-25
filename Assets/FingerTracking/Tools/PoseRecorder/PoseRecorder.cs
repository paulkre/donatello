using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public class PoseRecorder : MonoBehaviour
{
    public List<GameObject> objects;
    public string recordName = "rec";

    System.IO.BinaryWriter writer;
    System.IO.BinaryReader reader;
    System.IO.FileStream stream;

    private bool recording = false;
    private List<RecordFrame> recordFrames;


    private bool playing = false;
    private Record playRecord;
    private int playIndex = 0;
    private long playbackStartTime;

    private void Start()
    {

    }

    public void Update()
    {
        if(recording)
        {
            DoRecord();
        }
        else if(playing)
        {
            DoPlay();
        }

        KeyControls();
    }

    void KeyControls()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartRecord();
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            StopRecord();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            StartPlayback();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            StopPlayback();
        }
    }

    void PrepareRecording()
    {
        if (File.Exists($"records/{recordName}.rec"))
            File.Delete($"records/{recordName}.rec");

        recordFrames = new List<RecordFrame>(1024);
    }

    void StartRecord()
    {
        PrepareRecording();
        recording = true;

        print("start record");
    }

    void DoRecord()
    {
        RecordFrame currentFrame = new RecordFrame();
        List<RecordPose> poses = new List<RecordPose>(32);

        foreach (GameObject g in objects)
        {
            GetPoses(g, ref poses);
        }

        currentFrame.record = poses.ToArray();  

        recordFrames.Add(currentFrame);
    }

    void GetPoses(GameObject g, ref List<RecordPose> poses)
    {
        poses.Add(new RecordPose(g));

        for(int i = 0;i<g.transform.childCount;i++)
        {
            GetPoses(g.transform.GetChild(i).gameObject, ref poses);
        }
    }

    void StopRecord()
    {
        recording = false;

        Record rec = new Record(recordName, objects, recordFrames);
        string json = JsonUtility.ToJson(rec,true);

        if (!Directory.Exists("records"))
            Directory.CreateDirectory("records");

        File.WriteAllText($"records/{recordName}.rec", json);

        print("stop record");
    }

    void LoadRecord()
    {
        playRecord = JsonUtility.FromJson<Record>(File.ReadAllText($"records/{recordName}.rec"));
    }

    void StartPlayback()
    {
        if(playRecord != null)
        {
            if(playRecord.name.CompareTo(recordName) != 0)
            {
                LoadRecord();
            }
        }
        else
        {
            LoadRecord();
        }
        playbackStartTime = System.DateTime.Now.Ticks;
        
        string objectNames = "";
        foreach(string n in playRecord.gameObjectNames)
        {
            objectNames += $"{n} ";
        }
        System.DateTime date = new System.DateTime(playRecord.date);
        print($"loaded record: {playRecord.name} [{playRecord.size} frames / {playRecord.playtime} sec] from {date.ToString()}." +
            $"\nrecorded: {objectNames} | {playRecord.frames[0].record.Length} poses");

        playIndex = 0;
        playing = true;
    }

    void StopPlayback()
    {
        playing = false;
    }

    void DoPlay()
    {

        RecordPose[] poses = playRecord.frames[playIndex].record;

        int index = 0;
        foreach (GameObject g in objects)
        {
            SetPoses(g, ref poses, ref index);
        }

        //ensure matching playback time
        while(playIndex < playRecord.frames.Length && (System.DateTime.Now.Ticks-playbackStartTime) > playRecord.frames[playIndex].timestamp)
        {
            playIndex++;
        }

        //loop if end reached (exceeded)
        if (playIndex == playRecord.frames.Length)
        {
            playbackStartTime = System.DateTime.Now.Ticks;
            playIndex = 0;
        }
    }

    void SetPoses(GameObject g, ref RecordPose[] poses, ref int index)
    {
        g.transform.position = poses[index].GetPos();
        g.transform.rotation = poses[index].GetRot();

        index++;

        for (int i = 0; i < g.transform.childCount; i++)
        {
            SetPoses(g.transform.GetChild(i).gameObject, ref poses, ref index);
        }
    }
}

[System.Serializable]
public class Record
{
    public string name;
    public long date;
    public int size;
    public float playtime;

    public string[] gameObjectNames;
    public RecordFrame[] frames;


    public Record(string name, List<GameObject> gameObjects, List<RecordFrame> frames)
    {
        date = System.DateTime.Now.Ticks;
        this.name = name;

        gameObjectNames = new string[gameObjects.Count];
        for(int i = 0; i<gameObjectNames.Length; i++)
        {
            gameObjectNames[i] = gameObjects[i].name;
        }

        this.frames = frames.ToArray();

        //timestamp relative to record start time
        long recordStartTime = this.frames[0].timestamp;
        for (int i = 0; i < this.frames.Length; i++)
        {
            this.frames[i].timestamp -= recordStartTime;
        }

        size = frames.Count;

        int sec = Mathf.RoundToInt((this.frames[this.frames.Length - 1].timestamp) / 10E5f);
        playtime = sec / 10f;
    }
}

[System.Serializable]
public class RecordFrame
{
    public long timestamp = System.DateTime.Now.Ticks;
    public RecordPose[] record;
}


[System.Serializable]
public class RecordPose
{
    public float posX, posY, posZ, rotX, rotY, rotZ, rotW;

    public RecordPose(GameObject g)
    {
        Init(g.transform.position, g.transform.rotation);
    }

    public RecordPose(Transform t)
    {
        Init(t.position, t.rotation);
    }

    private void Init(Vector3 v, Quaternion q)
    {
        posX = v.x;
        posY = v.y;
        posZ = v.z;

        rotX = q.x;
        rotY = q.y;
        rotZ = q.z;
        rotW = q.w;
    }

    public Vector3 GetPos()
    {
        return new Vector3(posX, posY, posZ);
    }

    public Quaternion GetRot()
    {
        return new Quaternion(rotX, rotY, rotZ, rotW);
    }
}

