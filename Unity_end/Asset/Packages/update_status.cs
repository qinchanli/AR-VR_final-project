using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;

public class update_status
{
    // Start is called before the first frame update
    public Dictionary<string, string> Read(string object_name, string object_type)
    {
        Dictionary<string, string> to_return = new Dictionary<string, string>();
        List<Dictionary<string,string>> read_in = new List<Dictionary<string, string>>();
        string path = @"C:\Users\MSI-NB\Final project\data\" + object_type; 
        if (File.Exists(path))
        {
            using (StreamReader sr = File.OpenText(path))
            {
                Dictionary<string, string> obj = new Dictionary<string, string>();
                string line;
                string key;
                string value;
                string temp_word;
                while ((line = sr.ReadLine()) != null)
                {
                    temp_word = "";
                    value = "";
                    key = "";
                    string[] words = line.Split(' ');
                    foreach (var word in words)
                    {
                        if (word == ":")
                        {
                            if(key != "")
                            {
                                obj.Add(key, value);
                            }
                            value = "";
                            key = temp_word;
                            temp_word = "";
                        }
                        value += temp_word;
                        temp_word = word;
                    }
                    obj.Add(key, value);
                    read_in.Add(obj);
                    if (obj["name"] == object_name)
                    {
                        to_return = obj;
                        break;
                    }
                }
            }
        }
        else
        {
            to_return.Add("err_msg", "not file for this type");
        }
        return to_return;
    }

    // Update is called once per frame
    public string Write(string object_name, string object_type, Dictionary<string, string> object_data)
    {
        string path = @"C:\Users\MSI-NB\Final project\data\" + object_type+".json";
        List<string> write_in = new List<string>();
        List<Dictionary<string, string>> read_in = new List<Dictionary<string, string>>();
        bool status = true;
        Debug.Log("lol");
        // Create a file to write to.
        if (File.Exists(path))
        {
            string contents = File.ReadAllText(path);
            Debug.Log(contents);
            read_in = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(contents);
            foreach (Dictionary<string, string> data in read_in)
            {
                if (data["name"] == object_data["name"])
                {
                    status = false;
                    foreach (string key in object_data.Keys)
                    {
                        if (!data.ContainsKey(key))
                        {
                            data.Add(key, object_data[key]);
                        }
                        else
                        {
                            data[key] = object_data[key];
                        }

                    }
                }
            }
        }
        if (status)
        {
            read_in.Add(object_data);
        }
        string output = Newtonsoft.Json.JsonConvert.SerializeObject(read_in);
        using (StreamWriter sw = File.CreateText(path))
        {
            sw.WriteLine(output);
        }
        return "success";
    }
}
