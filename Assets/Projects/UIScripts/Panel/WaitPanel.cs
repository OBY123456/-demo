using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using System;

public class WaitPanel : BasePanel
{
    //返回待机页参数
    private float BackTime;
    private float Back_Time;
    private bool IsBack;

    /*照片墙参数*/
    //行
    private int Row = 7;
    
    //列
    //private int Column = 5;

    [Header("起点")]
    public Vector2 Origin;

    [Header("终点")]
    public float End;

    [Header("行间隔")]
    public float Interval_row;

    [Header("列间隔")]
    public float interval_column;

    [Header("移动速度")]
    public float MoveSpeed;

    [Header("图片宽")]
    public float PicWidth;

    [Header("图片高")]
    public float PicHeight;

    //生成点位置
    public Vector2[] CreatPoint;

    //生成图片开关
    private bool IsStart;

    //计时
    private float Times;

    private Transform SmallPicGroup;

    protected override void Start()
    {
        base.Start();
        if (Config.Instance)
        {
            BackTime = Config.Instance.configData.定时调用GC;
            MoveSpeed = Config.Instance.configData.小图移动速度;
            Interval_row = Config.Instance.configData.小图行间隔;
            interval_column = Config.Instance.configData.小图列间隔;
            CountDownStart();
        }

        if (PoolManager.Instance)
        {
            PicWidth = PoolManager.Instance.SmallPicPrefabs.GetComponent<RectTransform>().sizeDelta.x;
            PicHeight = PoolManager.Instance.SmallPicPrefabs.GetComponent<RectTransform>().sizeDelta.y;

            //假如左下角原点为起点
            Origin = new Vector2(-Screen.width/2 - PicWidth / 2, -Screen.height/2 + PicHeight / 2);
            Vector2 temp = Origin;
            //假如右下角原地为终点
            End = Screen.width / 2 + PicWidth / 2;

            interval_column += PicHeight;

            CreatPoint = new Vector2[Row];
            for (int i = 0; i < Row; i++)
            {
                CreatPoint[i] = temp;
                temp += Vector2.up * interval_column;
            }

            CreatPic();
            IsStart = true;
        }   
    }

    public override void InitFind()
    {
        base.InitFind();
        SmallPicGroup = FindTool.FindChildNode(transform, "SmallPicGroup");
    }

    /// <summary>
    /// 开启倒计时
    /// </summary>
    public void CountDownStart()
    {
        IsBack = true;
        Back_Time = BackTime;
    }

    private void FixedUpdate()
    {
        if(IsStart)
        {
            Times += Time.deltaTime;
            if(Times > Interval_row)
            {
                CreatPic();
                Times = 0;
            }
        }
    }

    private void CreatPic()
    {
        for (int i = 0; i < Row; i++)
        {
            GameObject Obj = PoolManager.Instance.GetPool(MTFrame.MTPool.PoolType.SmallPic);
            Obj.transform.SetParent(SmallPicGroup);
            Obj.transform.GetComponent<SmallPicItem>().ReSet(CreatPoint[i], MoveSpeed, End,PicManager.Instance.GetPicture());
        }
    }

    private void Update()
    {
        if (Back_Time > 0 && IsBack)
        {
            Back_Time -= Time.deltaTime;
            if (Back_Time <= 0)
            {
                GC.Collect();
            }

            //点击刷新倒计时
            if (Input.touchCount > 0)
            {
                Back_Time = BackTime;
            }
        }
    }
}
