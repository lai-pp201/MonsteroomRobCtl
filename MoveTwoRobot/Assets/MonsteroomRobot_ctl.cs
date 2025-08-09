using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class MonsteroomRobot_ctl : MonoBehaviour
{
    [ReadOnly, SerializeField] public float RobR = 0f;
    [ReadOnly, SerializeField] public float RobL = 0f;
    [ReadOnly, SerializeField] public type nowType;
    WebSocket websocket;
    public string serverIP;
    public string port = "666";
    #region AutoMoveParam
    public float MaxSpeed = 0.8f;
    public float Front_Rspeed = 0f;
    public float Front_Lspeed = 0f;
    public float Front_Time = 0f;
    public float Front_BreakRspeed = 0f;
    public float Front_BreakLspeed = 0f;
    public float Front_BreakTime = 0f;
    public float Back_Rspeed = 0f;
    public float Back_Lspeed = 0f;
    public float Back_Time = 0f;
    public float Back_BreakRspeed = 0f;
    public float Back_BreakLspeed = 0f;
    public float Back_BreakTime = 0f;
    public float ConsK = 1;
    #endregion
    public type AfterStop;
    type preType = type.Stop;
    bool stop = true;
    float changeTime = 0f;
    float alarmTime = 0f;
    public bool Handctl = false;
    // Start is called before the first frame update
    async void Start()
    {
        #region webSocket
        websocket = new WebSocket("ws://" + serverIP + ":" + port);

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };
        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            // var message = System.Text.Encoding.UTF8.GetString(bytes);
            // Debug.Log("OnMessage! " + message);
        };

        // Keep sending messages at every 0.3s
        InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        // waiting for messages
        await websocket.Connect();
        #endregion
    }
    #region webSocket
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }
    async void SendWebSocketMessage()
    {
        // create a C# object to be serialized to JSON
        ValueToPi.valueToRpi data = new ValueToPi.valueToRpi();
        data.x = RobR;
        data.z = RobL;

        // serialize the C# object to JSON
        string json = JsonConvert.SerializeObject(data);

        //string message = (slider_leftRight.value).ToString("0.##") + "," + (slider_upDown.value).ToString("0.##");

        //Debug.Log("value to rpi: " + json);
        //happy_json = JsonUtility.ToJson(message);
        //Debug.Log("string: " + happy_json);
        //Debug.Log(websocket.State);
        if (websocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await websocket.SendText(json);
            //Debug.Log("sending");
            // Sending plain text
            //await websocket.SendText("plain text message");
        }
    }
    #endregion
    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
        if (Input.GetKeyDown("space"))
        {
            nowType = nowType == type.Stop ? AfterStop : type.Stop;
        }
        CheckType();
        SetSpeed();
        CheckTime();
        //AdSpeed();
    }
    void CheckTime()
    {
        if(Time.time - changeTime > alarmTime && alarmTime != 0)
        {
            if((int)nowType != 4)
            {
                nowType += 1;
            }
            else
            {
                nowType = type.Front;
            }
            
        }
    }
    void CheckType()
    {
        if(nowType != preType)
        {
            changeTime = Time.time;
            preType = nowType;
        }
    }
    void SetSpeed()
    {
        switch (((int)nowType))
        {
            case 0:
                RobR = 0f;
                RobL = 0f;
                alarmTime = 0f;
                break;
            case 1:
                RobR = Front_Rspeed * ConsK;
                RobL = Front_Lspeed * ConsK;
                alarmTime = Front_Time;
                break;
            case 2:
                RobR = Front_BreakRspeed * ConsK;
                RobL = Front_BreakLspeed * ConsK;
                alarmTime = Front_BreakTime;
                break;
            case 3:
                RobR = Back_Rspeed * ConsK;
                RobL = Back_Lspeed * ConsK;
                alarmTime = Back_Time;
                break;
            case 4:
                RobR = Back_BreakRspeed * ConsK;
                RobL = Back_BreakLspeed * ConsK;
                alarmTime = Back_BreakTime;
                break;
        }
        RobR = Mathf.Abs(RobR) > MaxSpeed ? MaxSpeed * (RobR/ Mathf.Abs(RobR)) : RobR;
        RobL = Mathf.Abs(RobL) > MaxSpeed ? MaxSpeed * (RobL / Mathf.Abs(RobL)) : RobL;
        if (Handctl)
        {
            HandControll();
        }
    }
    public enum type
    {
        Stop,
        Front,
        FrontBreak,
        Back,
        BackBreak,
        Control
    }
    void AdSpeed()
    {
        if (Input.GetKey("u"))
        {
            Back_Lspeed += 0.01f;
        }
        else if (Input.GetKey("j"))
        {
            Back_Lspeed -= 0.01f;
        }
        else if (Input.GetKey("i"))
        {
            Back_Rspeed += 0.01f;
        }
        else if (Input.GetKey("k"))
        {
            Back_Rspeed -= 0.01f;

        }
        if (Input.GetKey("t"))
        {
            Front_Lspeed += 0.01f;
        }
        else if (Input.GetKey("g"))
        {
            Front_Lspeed -= 0.01f;
        }
        else if (Input.GetKey("y"))
        {
            Front_Rspeed += 0.01f;
        }
        else if (Input.GetKey("h"))
        {
            Front_Rspeed -= 0.01f;
        }
        if (Input.GetKeyDown("n"))
        {
            Front_Time += 0.1f;
        }
        else if (Input.GetKeyDown("b"))
        {
            Front_Time -= 0.1f;
        }
        else if (Input.GetKeyDown("m"))
        {
            Back_Time -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            Back_Time += 0.1f;
            Debug.Log("Comma");
        }
    }
    void HandControll()
    {
        if (Input.GetKey("w"))
        {
            RobR = MaxSpeed;
            RobL = -MaxSpeed;
        }
        else if (Input.GetKey("s"))
        {
            RobR = -MaxSpeed;
            RobL = MaxSpeed;
        }
        else if (Input.GetKey("a"))
        {
            RobR = -MaxSpeed;
            RobL = -MaxSpeed;
        }
        else if (Input.GetKey("d"))
        {
            RobR = MaxSpeed;
            RobL = MaxSpeed;
        }
        else if (Input.GetKey("q"))
        {
            RobR = 0.0f;
            RobL = 0.0f;
        }
        else
        {
            RobR = 0.01f;
            RobL = -0.01f;
        }
    }
}
