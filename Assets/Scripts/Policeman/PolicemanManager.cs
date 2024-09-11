using System.Collections;
using UnityEngine;

public class PolicemanManager : MonoBehaviour
{
    [Header("Walking")]
    [SerializeField] private float _waitTime = 2f;
    [SerializeField] private float _speedPoliceman = 1.5f;

    [Header("Patrolling")]
    [SerializeField] private Vector3[] _waypointsCoordinates;

    private Animator _animator;

    private int _currentWaypointIndex = 0;
    private bool _isWaiting = false;
    private bool _isMoving = false;
    private bool _isTurns = false;
    private bool _isFalling = false;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (_waypointsCoordinates.Length == 0)
        {
            enabled = false;
            return;
        }

        StartCoroutine(MoveToWaypoints());
    }

    private IEnumerator MoveToWaypoints()
    {
        while (true)
        {
            if (!_isWaiting)
            {
                yield return StartCoroutine(MoveTowardsWaypoint());

                _isTurns = true;

                _animator.SetBool("isMoving", false);

                FlipMove();

                yield return new WaitForSeconds(_waitTime);

                _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypointsCoordinates.Length;
            }

            yield return null;
        }
    }

    private IEnumerator MoveTowardsWaypoint()
    {
        Vector3 targetPosition = _waypointsCoordinates[_currentWaypointIndex];
        while (transform.position != targetPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _speedPoliceman * Time.deltaTime);

            _isMoving = true;
            _isTurns = false;

            _animator.SetBool("isMoving", _isMoving);
            _animator.SetBool("isTurns", _isTurns);

            yield return null;
        }
    }

    private void FlipMove()
    {
        //_animator.SetBool("isTurns", _isTurns);
        transform.Rotate(0f, 180f, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Buton"))
        {
            _speedPoliceman = 0f;

            _isFalling = true;
            _animator.SetBool("isFalling", _isFalling);

            Invoke("AccidentalFall", 2f);
        }
    }

    private void AccidentalFall()
    {
        _animator.enabled = false;
    }
}