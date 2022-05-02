using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class speed : MonoBehaviour
{
    TextMeshProUGUI speedText;
    // Start is called before the first frame update
    void Start()
    { 
        speedText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        int speed = System.Convert.ToInt32(PlayerMove.Instance.rb.velocity.magnitude);
        speedText.text = speed.ToString();
    }
}
