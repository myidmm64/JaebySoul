using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Sound/Boss")]
public class BossSoundDataSO : ScriptableObject
{
    public AudioClip startClip;
    public AudioClip flyClip;
    public AudioClip meleeAttackClip;
    public AudioClip tailAttackClip;

}
