using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectSound : AudioPlayer
{
    [SerializeField]
    private PlayerSoundDataSO _dataSO;

    public void PlayMeleeAttackSound()
    {
        PlayClip(_dataSO.meleeAttackClip);
    }
    public void PlayDashSound()
    {
        PlayClip(_dataSO.dashClip);
    }
    public void PlayDamageSound()
    {
        PlayClip(_dataSO.damageClip);
    }
    public void PlayLevelUpSound()
    {
        PlayClip(_dataSO.levelUpClip);
    }
    public void PlayRangeAttackSound()
    {
        PlayClip(_dataSO.rangeAttackClip);
    }
    public void PlayWalkSound()
    {
        PlayClip(_dataSO.walkClip);
    }
    public void PlayPosionDrinkSound()
    {
        PlayClip(_dataSO.posionDrinkClip);
    }
    public void PlayCoinSound()
    {
        PlayClip(_dataSO.coinClip);
    }

}
