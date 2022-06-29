using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGSoundManager : AudioPlayer
{
    [SerializeField]
    private AudioClip _normallyBGSound = null; // 아침 사운드
    [SerializeField]
    private AudioClip _nightEffectSound = null;// 밤 사운드
    [SerializeField]
    private AudioClip _bossStartBGSound = null; // 보스 사운드

    public AudioSource _childSource = null;


    protected override void Awake()
    {
        base.Awake();
        _childSource = transform.Find("NightEffectSound").GetComponent<AudioSource>();
    }

    public void OnDayBGPlay()
    {
        PlayClip(_normallyBGSound);
    }
    public void OnNightBGPlay()
    {
        _childSource.Stop();
        _childSource.clip = _nightEffectSound;
        _childSource.Play();
    }
    public void OnBossBGPlay()
    {
        PlayClip(_bossStartBGSound);
    }

}
