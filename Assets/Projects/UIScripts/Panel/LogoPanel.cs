using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using System;

public class LogoPanel : BasePanel
{
    private string Password = "I Love Loli";
    //private string data1 = "2021/5/13 00:00:00";

    protected override void Start()
    {
        base.Start();
        //CompanyDate(System.DateTime.Now.ToString(), data1);
    }

    public override void Open()
    {
        base.Open();
        if(Config.Instance)
        {
            if(Password.Contains(Config.Instance.configData.去水印码))
            {
                Hide();
            }
        }
    }

    public void CompanyDate(string dateStr1, string dateStr2)
    {
        //将日期字符串转换为日期对象
        DateTime t1 = Convert.ToDateTime(dateStr1);
        DateTime t2 = Convert.ToDateTime(dateStr2);
        //通过DateTIme.Compare()进行比较（）
        int num = DateTime.Compare(t1, t2);
        //t1> t2
        if (num > 0)
        {
            if(LogMsg.Instance)
            {
                LogMsg.Instance.Log("已过期!");
            }
            
        }
    }
}
