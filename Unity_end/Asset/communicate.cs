using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
public class communicate : MonoBehaviour
{
    private string theName = "";
    public Text textElement;
    public Text input;
    private readonly Thread receiveThread;
    private bool running;
    private Dictionary<string, string> data_from_bot = new Dictionary<string, string>();
    string target_position;
    Vector3 target_pos;
    public pointer pointers;

    public void StoreText(string s)
    {
        theName = s;
        Debug.Log(theName);
        AsyncIO.ForceDotNet.Force();
        string message_from_bot = "";
        using (var socket = new RequestSocket())
        {
            socket.Connect("tcp://localhost:5555");
            socket.SendFrame(theName);
            message_from_bot = socket.ReceiveFrameString();
        }
        data_from_bot = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(message_from_bot);
        Debug.Log(data_from_bot["response"]);
        textElement.text = data_from_bot["response"];
        Debug.Log(data_from_bot["response_cata"]);
        if (data_from_bot["response_cata"] == "position")
        {
            target_position = data_from_bot["response"];
            target_position = target_position.Substring(1, target_position.Length - 2);
            string[] posArray = target_position.Split(',');
            target_pos = new Vector3(float.Parse(posArray[0]), float.Parse(posArray[1]), float.Parse(posArray[2]));
            pointers = GameObject.Find("pointer").GetComponent<pointer>();
            pointers.target_position = target_pos;
        }
    }
    /*
    public Receiver()
    {
        receiveThread = new Thread((object callback) =>
        {
            using (var socket = new RequestSocket())
            {
                socket.Connect("tcp://localhost:5555");

                while (running)
                {
                    socket.SendFrameEmpty();
                    string message = socket.ReceiveFrameString();
                    Data data = JsonUtility.FromJson<Data>(message);
                    ((Action<Data>)callback)(data);
                }
            }
        });
    }
    */
}
