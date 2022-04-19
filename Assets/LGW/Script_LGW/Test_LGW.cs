using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_LGW : MonoBehaviour
{
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Destroy(gameObject);
        }
    }
}
