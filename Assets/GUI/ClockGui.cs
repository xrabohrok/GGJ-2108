using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class ClockGui : MonoBehaviour
{
    private Text ClockText;
	// Use this for initialization
	void Start ()
	{
	    ClockText = GetComponentInChildren<Text>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    String formatTime = string.Format("{0:hh : mm tt}", DateTime.Now);


	    ClockText.text = formatTime;
	}
}
