using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulEater : MonoBehaviour
{
    public void DestroyObj()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            other.GetComponent<PlayerDamaged>().HP -= 2;
        }
    }
}
