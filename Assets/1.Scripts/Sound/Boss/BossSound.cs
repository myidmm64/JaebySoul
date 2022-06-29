using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSound : AudioPlayer
{
    [SerializeField]
    private BossSoundDataSO _dataSO = null;

    [SerializeField]
    private AudioSource _tailSoundAudioSource = null;

    public void PlayStartSound()
    {
        PlayClip(_dataSO.startClip);
    }
    public void PlayFlySound()
    {
        StartCoroutine(FlySoundCoroutine());
    }
    private IEnumerator FlySoundCoroutine()
    {
        for(int i = 0; i<3; i++)
        {
            PlayClip(_dataSO.flyClip);
            yield return new WaitForSeconds(1f);
        }
    }

    public void PlayMeleeAttackSound()
    {
        PlayClip(_dataSO.meleeAttackClip);
    }
    public void PlayTailAttackSound()
    {
        _tailSoundAudioSource.Stop();
        _tailSoundAudioSource.clip = _dataSO.tailAttackClip;
        _tailSoundAudioSource.Play();
    }
}
