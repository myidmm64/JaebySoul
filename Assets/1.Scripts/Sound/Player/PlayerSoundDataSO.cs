using UnityEngine;

[CreateAssetMenu(menuName = "SO/Sound/Player")]
public class PlayerSoundDataSO : ScriptableObject
{
    public AudioClip walkClip;
    public AudioClip dashClip;
    public AudioClip meleeAttackClip;
    public AudioClip rangeAttackClip;
    public AudioClip damageClip;

    public AudioClip posionDrinkClip;
    public AudioClip levelUpClip;
    public AudioClip coinClip;
}
