using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedX = 1f; // ��-�� �� ��� �
    [SerializeField] private float jumpForce = 300f; // ���� ������

    private Rigidbody2D _rb;

    private float _horizontal = 0f; // ������� ������� (� ����� ������� ������ ��������� �����)

    private bool _isGround = false;
    private bool _isJump = false;
    private bool _isFacingRight = true;

    const float speedMultiplier = 50f; // ��������� ��������

    // Start is called before the first frame update

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _horizontal = Input.GetAxis("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && _isGround)
        {
            _isJump = true;
        }
    }

    private void FixedUpdate()
    {
        _rb.velocity = new Vector2(_horizontal * speedX * Time.fixedDeltaTime * speedMultiplier, _rb.velocity.y);

        if (_isJump)
        {
            _rb.AddForce(new Vector2(0f, jumpForce)); // �������� ���� ��� ������
            _isGround = false;
            _isJump = false; // ����� � ������� �� �������
        }

        // ���� ����� ���� ������, �� ������� �����, �� ������������ ��� ������ (�������� scale)
        if (_horizontal > 0f && !_isFacingRight)
        {
            Flip();
        } // ���� ���� �����, �� ������� ������, �� ���� ������������
        else if (_horizontal < 0f && _isFacingRight)
        {
            Flip();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }

    /*������� ������ � ����������� �� ����������� ��������*/
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }
}
