using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMonster : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> monsters = new List<GameObject>(); // ������ ���� ����
    [SerializeField]
    private List<Transform> spawnPoints = new List<Transform>(); // ���� ��ġ

    [SerializeField]
    private GameObject _spawnBeforeEffect = null;

    [SerializeField]
    private AudioClip _spawnMonsterClip = null;
    private AudioSource _audioSource = null;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private bool isSpwan = false; // �������ΰ�

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isSpwan == false)
            {
                isSpwan = true;
                StartCoroutine(Spawn(other.gameObject));
            }
        }
    }

    private IEnumerator Spawn(GameObject target)
    {
        _audioSource.clip = _spawnMonsterClip;
        _audioSource.Play();

        GameObject effect = Instantiate(_spawnBeforeEffect, transform.position, Quaternion.Euler(new Vector3(-90f, 0f ,0f)));
        Destroy(effect, 5f);
        transform.parent.GetComponent<MeshRenderer>().material.color = Color.red;


        yield return new WaitForSeconds(2f);

        for(int i =0; i<spawnPoints.Count; i++) // ���� ��ȯ
        {
            int random = Random.Range(0, monsters.Count);
            GameObject monster = Instantiate(monsters[random], spawnPoints[i].position, Quaternion.identity);
            monster.SendMessage("OnCkTarget", target); // �÷��̾�� Ÿ�� ����
            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(1f);
        Destroy(transform.parent.gameObject);
    }
}
