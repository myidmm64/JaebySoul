using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectSound : AudioPlayer
{
    [SerializeField]
    private PlayerSoundDataSO _dataSO;

    public void PlayMeleeAttackSound()
    {
        PlayClipWithVariablePitch(_dataSO.meleeAttackClip);
    }
    public void PlayDashSound()
    {
        _audioSorce.pitch = 1f;
        PlayClip(_dataSO.dashClip);
    }
    public void PlayDamageSound()
    {
        PlayClipWithVariablePitch(_dataSO.damageClip);
    }
    public void PlayLevelUpSound()
    {
        _audioSorce.pitch = 1f;
        PlayClip(_dataSO.levelUpClip);
    }
    public void PlayRangeAttackSound()
    {
        PlayClipWithVariablePitch(_dataSO.rangeAttackClip);
    }
    public void PlayWalkSound()
    {
        _audioSorce.pitch = 1f;
        PlayClip(_dataSO.walkClip);
    }
    public void PlayPosionDrinkSound()
    {
        PlayClipWithVariablePitch(_dataSO.posionDrinkClip);
    }
    public void PlayCoinSound()
    {
        PlayClipWithVariablePitch(_dataSO.coinClip);
    }

}
