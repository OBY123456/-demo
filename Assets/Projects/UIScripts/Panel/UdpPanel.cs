using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MTFrame;
using UnityEngine.UI;
using MTFrame.MTEvent;
using System;

public class UdpPanel : BasePanel
{
    public Text text;

    private float BackTime;
    private float Back_Time;
    private bool IsBack;

    protected override void Start()
    {
        base.Start();
        if(Config.Instance)
        BackTime = Config.Instance.configData.Backtime;
    }

    public override void InitFind()
    {
        base.InitFind();
        text = FindTool.FindChildComponent<Text>(transform, "Text");
    }

    public override void Open()
    {
        base.Open();

        text.text = "打开UdpPanel面板";
        EventManager.AddListener(MTFrame.MTEvent.GenericEventEnumType.Message, MTFrame.EventType.DataToPanel.ToString(), callback);
    }

    private void callback(EventParamete parameteData)
    {
        //如果需要判断事件名就用parameteData.EvendName
        //如果需要判断数据内容就用string data = parameteData.GetParameter<string>()[0];
        //要接收什么类型的数据就定义什么类型的数据，这里只会获取你选择的数据类型的数据
        string data = parameteData.GetParameter<string>()[0];
        text.text = data;
    }

    public override void Hide()
    {
        base.Hide();

        IsBack = true;
        Back_Time = BackTime;

        EventManager.RemoveListener(GenericEventEnumType.Message, MTFrame.EventType.DataToPanel.ToString(), callback);

        text.text = "AAAAAAAA";
    }

    private void Update()
    {
        if (Back_Time > 0 && IsBack)
        {
            Back_Time -= Time.deltaTime;

            if (Back_Time <= 0)
            {
                IsBack = false;
                UIState.SwitchPanel(PanelName.WaitPanel);
            }
            if (Input.GetMouseButton(0))
            {
                Back_Time = BackTime;
            }
        }
    }

}
