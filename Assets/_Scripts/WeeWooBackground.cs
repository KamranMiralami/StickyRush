using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.LowLevelPhysics;
using UnityEngine.UIElements;


public class WeeWooBackground : SingletonBehaviour<WeeWooBackground>
{
    [SerializeField] GameObject PickUpTrigger;
    int ColorIncrementer = 0;

    List<Color> AlphaColors = new List<Color>()
    {
        new Color(1, 0, 0, 0f),
        new Color(1,0,0, 0.05f),
        new Color(1,0,0, 0.10f),
        new Color(1,0,0, 0.15f)
        
    };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public IEnumerator ColorChanger()
    {
        while (true)
        {
            GetComponent<SpriteRenderer>().color = AlphaColors[ColorIncrementer];
            yield return new WaitForSeconds(0.2f);
            ColorIncrementer++;
            Debug.Log(this.GetComponent<SpriteRenderer>().color);
            if (ColorIncrementer == 4) { ColorIncrementer = 0; }
        }
    }
    
    

    // Update is called once per frame
    void Update()
    {

    }
}
