
using MTFrame.MTPool;
using UnityEngine;

/// <remarks>对象池管理类</remarks>
public class PoolManager:MonoBehaviour
{
    public static PoolManager Instance;

    public static SmallPicPool smallPicPool;
    [Header("小图预设")]
    public GameObject SmallPicPrefabs;

    public static BigPicPool bigPicPool;
    [Header("大图预设")]
    public GameObject BigPicPrefabs;

    private float HeightMax;
    private float WidthMax;
    private float offect = 10f;

    private void Awake()
    {
        Instance = this;
        Init();

        HeightMax = SmallPicPrefabs.GetComponent<RectTransform>().sizeDelta.y / 2 + BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.y / 2 + offect;
        WidthMax = SmallPicPrefabs.GetComponent<RectTransform>().sizeDelta.x / 2 + BigPicPrefabs.GetComponent<RectTransform>().sizeDelta.x / 2 + offect;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    private void Init()
    {
        smallPicPool = new SmallPicPool();
        smallPicPool.Init();

        bigPicPool = new BigPicPool();
        bigPicPool.Init();
    }

    /// <summary>
    /// 放回对象池
    /// </summary>
    public void AddPool(PoolType poolType,GameObject go)
    {
        switch (poolType)
        {
            case PoolType.SmallPic:
                smallPicPool.AddPool(go);
                break;
            case PoolType.BigPic:
                bigPicPool.AddPool(go);
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// 获取对象
    /// </summary>
    public GameObject GetPool(PoolType poolType)
    {
        GameObject t;
        switch (poolType)
        {
            case PoolType.SmallPic:
                t= smallPicPool.GetPool();
                if(t == null)
                {
                    t = Instantiate(SmallPicPrefabs);
                    smallPicPool.UsePool.Add(t);
                }
                break;
            case PoolType.BigPic:
                t = bigPicPool.GetPool();
                if (t == null)
                {
                    t = Instantiate(BigPicPrefabs);
                    bigPicPool.UsePool.Add(t);
                }
                break;
            default:
                t = null;
                break;
        }
        return t;
    }

    private void OnDestroy()
    {
        smallPicPool.Clear();
        bigPicPool.Clear();
    }

    /// <summary>
    /// 核心公式
    /// </summary>
    /// <param name="CirclePoint">大图坐标</param>
    /// <param name="r">感应半径</param>
    /// <param name="point">小图坐标</param>
    /// <returns></returns>
    //public static bool IsInCircle(Vector2 CirclePoint, float r, Vector2 point)
    //{
    //    return Mathf.Sqrt((point.x - CirclePoint.x) * (point.x - CirclePoint.x) + (point.y - CirclePoint.y) * (point.y - CirclePoint.y)) < r;
    //}

    /// <summary>
    /// 核心公式2
    /// </summary>
    /// <param name="CirclePoint"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsInRectangle(Vector2 CirclePoint, Vector2 point)
    {
        if(Mathf.Abs(CirclePoint.x - point.x) <= WidthMax && Mathf.Abs(CirclePoint.y - point.y) <= HeightMax)
        {
            return true;
        }

        return false;
    }
}