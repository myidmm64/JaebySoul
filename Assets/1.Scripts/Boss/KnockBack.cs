using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    [SerializeField]
    private GameObject _player = null;

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Debug.Log("¿Í ! ³Ë¹é !!");
            _player.GetComponent<Player>().Knockback((_player.transform.position - transform.position).normalized);
        }
    }
}
