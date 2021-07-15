using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
    private float r;
    private float AnimaTime = 0.5f;
    //记录手指id，唯一值
    private float finger = -1;
    private Touch touch;
    //滑动速度，超过这个速度算甩飞
    private float SlideSpeed = 2.0f;
    //飞行速度
    private float FlySpeed = 5.0f;
    private bool IsFly;
    //飞行方向
    private Vector2 Direction;
    //大图长宽，用来计算边缘
    private float BigPicHeight, BigPicWidth;
    private float WidthMin, WidthMax, HeightMin, HeightMax;

    private void Awake()
    {
        button = transform.GetChild(0).GetComponent<Button>();
        rawImage = transform.GetComponent<RawImage>();
    }

    private void Start()
    {
        button.onClick.AddListener(() => {
            Close();
        });

        r = UIManager.GetPanel<WaitPanel>(WindowTypeEnum.ForegroundScreen).R;

        if(PoolManager.Instance)
        {
            BigPicWidth = PoolManager.Instance.BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.x;
            BigPicHeight = PoolManager.Instance.BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.y;
            WidthMin = -Screen.width / 2 - BigPicWidth;
            WidthMax = Screen.width / 2 + BigPicWidth;
            HeightMin = -Screen.height / 2 - BigPicHeight;
            HeightMax = Screen.height / 2 + BigPicHeight;
        }
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
        }

        if(finger != -1)
        {
            for (int i = 0; i < Input.touches.Length; i++)
            {
                if (Input.touches[i].fingerId == finger)
                {
                    touch = Input.touches[i];
                }
            }
            LogMsg.Instance.Log(touch.deltaPosition.x.ToString());
            if(touch.deltaPosition.x > SlideSpeed && !IsTouch)
            {
                Direction = touch.deltaPosition.normalized;
                IsFly = true;
                finger = -1;
                
            }
        }

        if(IsFly && Direction!=Vector2.zero)
        {
            Vector3 NewPos = transform.localPosition + new Vector3( Direction.x, Direction.y,0) * FlySpeed;
            transform.DOLocalMove(NewPos, 0.1f);
            if(transform.localPosition.x > WidthMax || transform.localPosition.x < WidthMin || transform.localPosition.y > HeightMax || transform.localPosition.y < HeightMin)
            {
                Close();
            }
        }

        if(IsCheck)
        {
            for (int i = 0; i < MoveImageList.Count; i++)
            {
                if(MoveImageList[i].GetComponent<SmallPicItem>().BigPic == null)
                {
                    if (PicManager.IsInCircle(transform.localPosition, r, MoveImageList[i].GetComponent<SmallPicItem>().CurrentPos))
                    {
                        MoveImageList[i].GetComponent<SmallPicItem>().BigPic = gameObject;
                        smallPicItems.Add(MoveImageList[i]);
                    }
                }
            }
        }
    }

    public void OnMouseDrag()
    {
        IsTouch = true;
        CountDown = 0;
    }

    public void BeginDrag()
    {
        Touch[] touches = Input.touches;
        
        for (int i = 0; i < touches.Length; i++)
        {
            LogMsg.Instance.Log("touch.phase:" + touches[i].phase);
            if (touches[i].phase == TouchPhase.Began)
            {
                finger = touches[i].fingerId;
                LogMsg.Instance.Log("fingerID:" + touches[i].fingerId);
            }
        }
        LogMsg.Instance.Log("finger:" + finger);
    }

    public void EndDrag()
    {
        IsTouch = false;
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
        Direction = Vector2.zero;
        foreach (var item in smallPicItems)
        {
            item.GetComponent<SmallPicItem>().BigPic = null;
        }
        PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.BigPic, gameObject);
        rawImage.texture = null;
    }
}
