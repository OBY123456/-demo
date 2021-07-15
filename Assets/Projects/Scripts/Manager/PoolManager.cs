
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


    private void Awake()
    {
        Instance = this;
        Init();
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
}