using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box_1 : MonoBehaviour
{
    private string objectname;
    private Dictionary<string, string> object_data;
    private string position = "";
    private string color = "red";
    private update_status update2file = new update_status();
    private string object_type = "box";
    // Start is called before the first frame update
    void Start()
    {
        object_data = new Dictionary<string, string>();
        objectname = "box_1";
        Vector3 pos = transform.position;
        position = "(" + pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString() + ")";
        object_data.Add("name", objectname);
        object_data.Add("position", position);
        object_data.Add("color", color);
        update2file.Write(objectname, object_type, object_data);
        Debug.Log(pos);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        position = "(" + pos[0].ToString() + "," + pos[1].ToString() + "," + pos[2].ToString() + ")";
        object_data["position:"]= position;
    }
}
