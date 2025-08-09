using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;

public class BoxingRob : MonoBehaviour
{
    WebSocket websocket;
    public string serverIP;
    public string port = "666";
    public float RobX = 0;
    public float RobZ = 0;
    public float MaxSpeed = 0.08f;
    public float MaxRotate = 0.04f;
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
        data.x = RobX;
        data.z = RobZ;

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
        if (Input.GetKey("w"))
        {
            RobZ = -MaxSpeed;
            RobX = MaxSpeed;
        }
        else if (Input.GetKey("s"))
        {
            RobZ = MaxSpeed;
            RobX = -MaxSpeed;
        }
        else if (Input.GetKey("a"))
        {
            RobX = MaxRotate;
            RobZ = MaxRotate;
        }
        else if (Input.GetKey("d"))
        {
            RobX = -MaxRotate;
            RobZ = -MaxRotate;
        }
        else
        {
            RobX = 0;
            RobZ = 0;
        }
    }
}
