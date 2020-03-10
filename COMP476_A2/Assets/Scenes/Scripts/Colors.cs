using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Colors : MonoBehaviour
{
    public static Color lightBlue;
    public static Color green;
    // Start is called before the first frame update
    void Start()
    {
        lightBlue = new Color(1/255.0f, 230/ 255.0f, 255/ 255.0f);
        green = new Color(0.0f, 1.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
