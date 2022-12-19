using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pointer : MonoBehaviour
{
    public Transform character_pos;
    private Vector3 character_position;
    private Vector3 offset = new Vector3(-1, -0.3f, 2);
    public Vector3 target_position;
    private Vector3 direction;
    private Quaternion _facing;
    // Start is called before the first frame update
    void Start()
    {
        direction = new Vector3(0, 1, 0);
        character_position = character_pos.position;
        transform.position  = character_position + offset;
        _facing = transform.rotation;
        Debug.Log(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        character_position = character_pos.position;
        direction = target_position - transform.position;
        var rotation = Quaternion.LookRotation(direction.normalized);
        rotation *= _facing;
        transform.rotation = rotation;
        transform.position = character_position + offset;
    }
}
