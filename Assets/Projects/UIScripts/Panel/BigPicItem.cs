using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BigPicItem : MonoBehaviour
{
    private Button button;
    private RawImage rawImage;

    [HideInInspector]
    public bool IsTouch;
    private float HideTime = 200f;
    [HideInInspector]
    public float CountDown = 0;

    private List<GameObject> MoveImageList = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> smallPicItems = new List<GameObject>();
    public bool IsCheck;

    private float AnimaTime = 0.5f;
    //记录手指id，唯一值
    private float finger = -1;
    private Touch touch;
    //滑动速度，超过这个速度算甩飞
    private float SlideSpeed = 10.0f;
    //飞行速度
    private float FlySpeed = 35.0f;
    private bool IsFly;
    //飞行方向
    private Vector2 Direction;
    //大图长宽，用来计算边缘
    private float BigPicHeight, BigPicWidth;
    private float WidthMin, WidthMax, HeightMin, HeightMax;
    //记录手指位置，用来计算滑动距离
    private Vector2 Pos1, Pos2;

    private void Awake()
    {
        button = transform.GetChild(0).GetComponent<Button>();
        rawImage = transform.GetComponent<RawImage>();
        //GetComponent<PressGesture>().Pressed += pressHandler;
        //GetComponent<ReleaseGesture>().Released += releasdHandler;
    }

    private void Start()
    {
        button.onClick.AddListener(() => {
            Close();
        });

        if(PoolManager.Instance)
        {
            BigPicWidth = PoolManager.Instance.BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.x;
            BigPicHeight = PoolManager.Instance.BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.y;
            WidthMin = -Screen.width / 2 - BigPicWidth;
            WidthMax = Screen.width / 2 + BigPicWidth;
            HeightMin = -Screen.height / 2 - BigPicHeight;
            HeightMax = Screen.height / 2 + BigPicHeight;
        }

        if(Config.Instance)
        {
            AnimaTime = Config.Instance.configData.大图出现动画时间;
            HideTime = Config.Instance.configData.大图自动消失时间;
            SlideSpeed = Config.Instance.configData.手指滑动灵敏度;
            FlySpeed = Config.Instance.configData.大图飞行速度;
        }
    }

    public void releasdHandler()
    {
        IsTouch = false;
        finger = -1;
        Pos1 = Vector2.zero;
    }

    public void pressHandler()
    {
        Touch[] touches = Input.touches;

        for (int i = 0; i < touches.Length; i++)
        {
            if (touches[i].phase == TouchPhase.Began)
            {
                finger = touches[i].fingerId;
            }
        }
        IsFly = false;
        Direction = Vector2.zero;
        IsTouch = true;
    }

    //核心算法
    private void FixedUpdate()
    {
        if(!IsTouch)
        {
            CountDown += Time.fixedDeltaTime;
            if(CountDown > HideTime)
            {
                Close();
            }

            if(Direction != Vector2.zero)
            {
                IsFly = true;
            }
        }
        else
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == finger)
                {
                    touch = Input.touches[i];
                }
            }
            Pos1 = touch.position;
            transform.position = touch.position;
            
            if (Pos1!=Pos2 && Pos1!=Vector2.zero && Pos2 != Vector2.zero)
            {
                Vector2 vector2 = Pos1 - Pos2;
                if (Mathf.Abs(vector2.x) > SlideSpeed)
                {
                    Direction = vector2.normalized;
                }
                else
                {
                    Direction = Vector2.zero;
                }
            }

        }

        if(IsFly)
        {
            Vector3 NewPos = transform.localPosition + new Vector3( Direction.x, Direction.y,0) * FlySpeed;
            transform.DOLocalMove(NewPos, 0.1f);
        }

        if(IsCheck)
        {
            for (int i = 0; i < MoveImageList.Count; i++)
            {
                if(MoveImageList[i].GetComponent<SmallPicItem>().BigPic == null)
                {
                    if (PoolManager.Instance.IsInRectangle(transform.localPosition,MoveImageList[i].GetComponent<SmallPicItem>().CurrentPos))
                    {
                        MoveImageList[i].GetComponent<SmallPicItem>().BigPic = gameObject;
                        smallPicItems.Add(MoveImageList[i]);
                    }
                }
            }
        }

        Pos2 = Pos1;

        if (transform.localPosition.x > WidthMax || transform.localPosition.x < WidthMin || transform.localPosition.y > HeightMax || transform.localPosition.y < HeightMin)
        {
            Close();
        }
    }

    public void OnMouseDrag()
    {
        IsTouch = true;
        CountDown = 0;
    }

    public void SetPic(Texture texture)
    {
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.zero;
        MoveImageList = PoolManager.smallPicPool.UsePool;
        rawImage.texture = texture;
        IsTouch = false;
        IsCheck = true;
        transform.DOScale(1, AnimaTime);
    }

    private void Close()
    {
        transform.DOKill();
        CountDown = 0;
        IsTouch = true;
        IsFly = false;
        IsCheck = false;
        finger = -1;
        Direction = Pos1 = Pos2 = Vector2.zero;
        foreach (var item in smallPicItems)
        {
            item.GetComponent<SmallPicItem>().BigPic = null;
        }
        PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.BigPic, gameObject);
        rawImage.texture = null;
    }
}
