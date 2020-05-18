using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
public class Message : MonoBehaviour
{
    public GameObject Ball;
    public Text Distance;
    public Text Speed;
    public Text RunTime;
    public Text LocalTime;
    private float speed=0;
    private DateTime start;
    private Vector3 lastPos,towardPos;
    private float result;
    private float distance = 0;
    void Start()
    {
        lastPos = Ball.transform.position;
        start = DateTime.Now;
        LocalTime.text = DateTime.Now.ToShortTimeString().ToString();
        
        Speed.text = "当前速度：" + Convert.ToString(speed) + " km/h";
    }

    void FixedUpdate()
    {
        towardPos = Ball.transform.position - lastPos; //两帧间向量差
        speed = towardPos.magnitude / Time.deltaTime; // 距离/时间 =带正负的速度
        lastPos = Ball.transform.position; //设置lastPosition
        LocalTime.text = DateTime.Now.ToShortTimeString().ToString();
        DateTime now = DateTime.Now;
        TimeSpan t = now.Subtract(start);
        RunTime.text = "骑行时间：" + t.ToString();
        distance += towardPos.magnitude;
        Distance.text = "骑行距离：" + (int)distance + "m";
        Speed.text = "当前速度：" + Convert.ToString((int)(speed * 3.6)) + " km/h";
    }
}
