using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Linq;
using UnityEditor;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using UnityEngine.UI;
using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
public class Speech_Rec : MonoBehaviour
{
	private DictationRecognizer dictationRecognizer;
	private bool is_listen;
	private bool status_change;
	private string theName = "";
	public Text textElement;
	public Text input;
	private bool running;
	private Dictionary<string, string> data_from_bot = new Dictionary<string, string>();
	string target_position;
	Vector3 target_pos;
	public pointer pointers;
	// Use this for initialization
	void Start()
	{
		is_listen = false;
		status_change = false;
		dictationRecognizer = new DictationRecognizer();

		dictationRecognizer.DictationResult += onDictationResult;
		dictationRecognizer.DictationHypothesis += onDictationHypothesis;
		dictationRecognizer.DictationComplete += onDictationComplete;
		dictationRecognizer.DictationError += onDictationError;

		//dictationRecognizer.Start();
	}
	void Update()
    {
		dictationRecognizer.DictationComplete += onDictationComplete;
		if (Input.GetKeyDown(KeyCode.V))
        {
			if (status_change) status_change = false;
			else status_change = true;

			if (is_listen) is_listen = false;
			else is_listen = true;
		}
		if (is_listen && status_change)
        {
			dictationRecognizer.Start();
			status_change = false;
		}
        else if (!is_listen && status_change)
        {
			dictationRecognizer.Stop();
			status_change = false;
		}
    }
	void onDictationResult(string text, ConfidenceLevel confidence)
	{
		// write your logic here
		theName = text;
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

	void onDictationHypothesis(string text)
	{
		// write your logic here
		//Debug.LogFormat("Dictation hypothesis: {0}", text);
	}

	void onDictationComplete(DictationCompletionCause cause)
	{
		// write your logic here
		if (cause != DictationCompletionCause.Complete)
			Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", cause);
	}

	void onDictationError(string error, int hresult)
	{
		// write your logic here
		Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
	}
}
