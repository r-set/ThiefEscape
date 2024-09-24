using UnityEngine;

public class Door : MonoBehaviour
{
    private Vector3 _openDoorPositionOffset = new Vector3(0, 2f, 0);
    private bool _isOpenDoor = false;
    [SerializeField] private GameObject _keyPrefab;

    private void OnTriggerEnter(Collider other)
    {
        if (!_isOpenDoor && other.CompareTag("Player"))
        {
            if (_keyPrefab.GetComponent<Key>().isCollectedKey)
            {
                OpenDoor();
                Destroy(_keyPrefab);
            }
        }
    }

    private void OpenDoor()
    {
        transform.position += _openDoorPositionOffset;
        _isOpenDoor = true;
    }
}
