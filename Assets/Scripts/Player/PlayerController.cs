using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Vector2 movementInput;

    private Rigidbody _rb;
    private Animator _animator;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }

    public void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();

        _animator.SetFloat("MoveX", movementInput.x);
        _animator.SetFloat("MoveY", movementInput.y);
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        Vector3 movement = new Vector3(movementInput.x, 0, movementInput.y) * _moveSpeed * Time.deltaTime;

        _rb.MovePosition(_rb.position + movement);
        _animator.SetBool("isMoving", false);

        if (movement != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
            _rb.rotation = Quaternion.RotateTowards(_rb.rotation, toRotation, 720 * Time.deltaTime);

            _animator.SetBool("isMoving", true);
        }
    }
}
