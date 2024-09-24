using UnityEngine;

public class Key : MonoBehaviour
{
    private Vector3 _keyOffset = new Vector3(0, 1.7f, 0);
    private Transform _player;
    [HideInInspector] public bool isCollectedKey = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _player = other.transform;
            isCollectedKey = true;

            GetComponent<Collider>().enabled = false;
        }
    }

    private void Update()
    {
        KeyPosition();
    }

    private void KeyPosition()
    {
        if (isCollectedKey && _player != null)
        {
            transform.position = _player.position + _keyOffset;
        }
    }
}
