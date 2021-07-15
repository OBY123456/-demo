using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 播放序列帧，挂在Image组件上
/// </summary>
public class PlaySprite : MonoBehaviour
{
    private Image ImageSource;
    private int mCurFrame = 0;
    private float mDelta = 0;
    [Header("帧率")]
    public float FPS = 30;
    Sprite[] SpriteFrames;
    [Header("是否播放")]
    public bool IsPlaying = false;
    [Header("序列帧路径")]
    public string Path;
    [Header("循环帧节点")]
    public int LoopIndex;

    public int FrameCount
    {
        get
        {
            return SpriteFrames.Length;
        }
    }
    void Awake()
    {
        ImageSource = GetComponent<Image>();
        SpriteFrames = Resources.LoadAll<Sprite>(Path);
    }
    void OnEnable()
    {
        IsPlaying = true;
    }
    private void SetSprite(int idx)
    {
        ImageSource.sprite = SpriteFrames[idx];
    }

    void Update()
    {
        if (!IsPlaying || 0 == FrameCount)
        {
            return;
        }
        else
        {
            mDelta += Time.deltaTime;
            if (mDelta > 1 / FPS)
            {
                mDelta = 0;
                mCurFrame++;
                //循环播放部分
                if (mCurFrame >= FrameCount)
                {
                    mCurFrame = LoopIndex;
                }
                SetSprite(mCurFrame);
            }
        }
    }
    private void OnDisable()
    {
        //初始化序列帧初始图片
        SetSprite(0);
        mDelta = 0;
        mCurFrame = 0;
        IsPlaying = false;
    }
}
