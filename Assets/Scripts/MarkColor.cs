using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi;
using UnityEngine;

public class MarkColor : MonoBehaviour
{

    public GameObject colisor;

   

    void Start()
    {
        
    }


    public Color GetColor() {
        float d = (gameObject.transform.position - colisor.transform.position).magnitude;
        if(d>0.9) return Color.green;
        else if(d>0.7) return Color.yellow;
        else return Color.red;
    }

    void Update() {

        gameObject.GetComponent<Renderer>().material.SetColor("_Color", GetColor());

    }
}