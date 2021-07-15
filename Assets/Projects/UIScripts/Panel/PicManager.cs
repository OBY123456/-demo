using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicManager : MonoBehaviour
{
    public static PicManager Instance;

    string PicPath;

    //照片合集
    [HideInInspector]
    public List<Texture2D> PicGroup = new List<Texture2D>();

    //当前照片列表
    [HideInInspector]
    public List<string> PicList = new List<string>();

    private void Awake()
    {
        Instance = this;
        PicPath = Application.streamingAssetsPath + "/Picture";
        if(FileHandle.Instance.IsExisFolder2(PicPath))
        {
            List<string> vs = FileHandle.Instance.GetImagePath(PicPath);
            for (int i = 0; i < vs.Count; i++)
            {
                PicGroup.Add(FileHandle.Instance.LoadByIO(vs[i]));
            }
        }
    }

    /// <summary>
    /// 获取图片
    /// </summary>
    /// <returns></returns>
    public Texture2D GetPicture()
    {
        int num = UnityEngine.Random.Range(0, PicGroup.Count -1);
        //只有当照片总数大于当前移动的照片数量才需要去重
        if(PoolManager.bigPicPool.UsePool.Count <= PicGroup.Count)
        {
            if (PicList.Count > 0)
            {
                bool IsSame = JudeSameName(PicGroup[num].name);
                while (IsSame)
                {
                    num = UnityEngine.Random.Range(0, PicGroup.Count - 1);
                    IsSame = JudeSameName(PicGroup[num].name);
                }
            }
            PicList.Add(PicGroup[num].name);
        }

        return PicGroup[num];
    }

    /// <summary>
    /// 判断是否重复
    /// </summary>
    /// <param name="st"></param>
    /// <returns></returns>
    private bool JudeSameName(string st)
    {
        foreach (var item in PicList)
        {
            if(item == st)
            {
                return false;
            }
        }
        return true;
    }
}
