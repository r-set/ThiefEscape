using UnityEngine;

public class CameraRotateManager : MonoBehaviour
{
    [SerializeField] private GameObject _cameraToRotate;
    [SerializeField] private float _rotationAngle = 75f;
    [SerializeField] private float _rotationSpeed = 50f;

    private float _currentAngle = 0f;
    private bool _rotatingRight = true;

    private void Update()
    {
        RotateObject();
    }

    private void RotateObject()
    {
        float direction = _rotatingRight ? 1f : -1f;

        _currentAngle += direction * _rotationSpeed * Time.deltaTime;

        if (_currentAngle >= _rotationAngle)
        {
            _currentAngle = _rotationAngle;
            _rotatingRight = false;
        }
        else if (_currentAngle <= -_rotationAngle)
        {
            _currentAngle = -_rotationAngle;
            _rotatingRight = true;
        }

        _cameraToRotate.transform.rotation = Quaternion.Euler(-90f, _currentAngle, 0f);
    }
}
