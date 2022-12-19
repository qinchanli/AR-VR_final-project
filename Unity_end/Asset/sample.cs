using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sample : MonoBehaviour
{
    private Vector3 position;
    public pointer pointers;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pointers = GameObject.Find("pointer").GetComponent<pointer>();
        position = transform.position;
        //pointers.target_position = position;
    }
}
