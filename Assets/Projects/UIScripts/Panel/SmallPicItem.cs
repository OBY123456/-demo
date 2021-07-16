using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SmallPicItem : MonoBehaviour
{
    [Header("当前位置")]
    public Vector2 CurrentPos;

    [Header("目标位置")]
    public Vector2 TarGetPos;

    [Header("移动速度")]
    public float MoveSpeed;

    [Header("终点位置")]
    public float EndPoint;

    [Header("移动控制开关")]
    public bool IsStart;

    /// <summary>
    /// 图片高度，用来计算边缘
    /// </summary>
    private float Heiht;

    private Button button;
    private RawImage rawImage;

    private Transform BigPicGroup;

    public GameObject BigPic;

    private List<GameObject> BigPicList = new List<GameObject>();

    private float HeightMin, HeightMax;

    /// <summary>
    /// 向上/向下飞走的速度
    /// </summary>
    private float LeaveSpeed = 30f;

    private void Awake()
    {
        button = transform.GetComponent<Button>();
        BigPicGroup = GameObject.Find("BigPicGroup").transform;
        rawImage = transform.GetComponent<RawImage>();
        BigPic = null;
    }

    private void Start()
    {
        button.onClick.AddListener(() => {
            if (PoolManager.Instance)
            {
                GameObject obj = PoolManager.Instance.GetPool(MTFrame.MTPool.PoolType.BigPic);
                obj.transform.SetParent(BigPicGroup);
                obj.transform.localPosition = transform.localPosition;   
                obj.GetComponent<BigPicItem>().SetPic(rawImage.texture);
            }
        });

        if(PoolManager.Instance)
        {
            Heiht = PoolManager.Instance.SmallPicPrefabs.GetComponent<RectTransform>().sizeDelta.y;
            HeightMin = -Screen.height / 2 - Heiht /2;
            HeightMax = Screen.height / 2 + Heiht /2;
        }
    }

    bool IsHave = false;

    //核心算法
    private void FixedUpdate()
    {
        if(IsStart)
        {
            TarGetPos += Vector2.right * MoveSpeed;

            if(BigPic != null)
            {
                IsHave = JudeIsHaveBigPic();
                if(!IsHave)
                {
                    BigPic.GetComponent<BigPicItem>().smallPicItems.Remove(gameObject);
                    BigPic = null;
                }
            }
            else
            {
                IsHave = false;
            }
            
            if (IsHave)
            {
                //transform.SetAsLastSibling();

                if(transform.localPosition.y >= 0)
                {
                    CurrentPos += Vector2.up * LeaveSpeed;
                    if(CurrentPos.y > HeightMax)
                    {
                        CurrentPos = new Vector2(CurrentPos.x, HeightMax);
                    }
                }
                else
                {
                    CurrentPos += Vector2.down * LeaveSpeed;
                    if(CurrentPos.y < HeightMin)
                    {
                        CurrentPos = new Vector2(CurrentPos.x, HeightMin);
                    }
                }
                if(transform.localPosition.y == HeightMax || transform.localPosition.y == HeightMin)
                {
                    CurrentPos = new Vector2(TarGetPos.x,CurrentPos.y);
                }

                if (CurrentPos.x > EndPoint)
                {
                    Exit();
                }

                transform.DOLocalMove(CurrentPos, 0.1f);
                
            }
            else
            {
                CurrentPos += Vector2.right * MoveSpeed;
                
                if (CurrentPos.x > EndPoint)
                {
                    Exit();
                }
                CurrentPos = Vector3.Lerp(CurrentPos, TarGetPos, 0.1f);
                transform.localPosition = CurrentPos;
            }
        }
    }

    private void Exit()
    {
        if (PoolManager.Instance)
        {
            PoolManager.Instance.AddPool(MTFrame.MTPool.PoolType.SmallPic, this.gameObject);
        }
        if(BigPic!=null)
        {
            BigPic.GetComponent<BigPicItem>().smallPicItems.Remove(gameObject);
        }
        BigPic = null;
        IsHave = false;
        IsStart = false;
    }

    /// <summary>
    /// 设置属性
    /// </summary>
    /// <param name="_CurrentPos">起点坐标</param>
    /// <param name="_MoveSpeed">移动速度</param>
    /// <param name="_EndPoint">终点位置</param>
    public void ReSet(Vector3 _CurrentPos,float _MoveSpeed,float _EndPoint,Texture2D texture2D)
    {
        transform.localScale = Vector3.one;
        transform.localEulerAngles = Vector3.zero;
        CurrentPos = _CurrentPos;
        TarGetPos = _CurrentPos;
        transform.localPosition = _CurrentPos;
        MoveSpeed = _MoveSpeed;
        EndPoint = _EndPoint;
        rawImage.texture = texture2D;
        BigPicList = PoolManager.bigPicPool.UsePool;
        IsStart = true;
    }

    /// <summary>
    /// 判断回去的位置是否有大图
    /// </summary>
    /// <returns></returns>
    private bool JudeIsHaveBigPic()
    {
        foreach (var item in BigPicList)
        {
            if(PoolManager.Instance.IsInRectangle(item.transform.localPosition,TarGetPos))
            {
                return true;
            }
        }

        return false;
    }
}
