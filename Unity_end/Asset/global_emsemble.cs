using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class global_emsemble : MonoBehaviour
{
    public Vector3 main_character_pos;
    // Start is called before the first frame update
    void Start()
    {
        main_character_pos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        main_character_pos = transform.position;
    }
}
