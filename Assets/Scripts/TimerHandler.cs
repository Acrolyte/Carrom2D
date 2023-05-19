using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class TimerHandler : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timeText;
    float stoptime = 5;

    private void OnEnable()
    {
        stoptime = 5;
    }
    private void Update()
    {
        if(stoptime > 0)
        {
            stoptime -= Time.deltaTime;
        }
        timeText.SetText(stoptime.ToString("0"));
    }
}
