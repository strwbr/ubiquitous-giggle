using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float speedX = 1f; // ск-ть по оси Х
    [SerializeField] private float jumpForce = 300f; // сила прыжка

    private Rigidbody2D _rb;

    private float _horizontal = 0f; // нажатая клавиша (в какую сторону должен двигаться игрок)

    private bool _isGround = false;
    private bool _isJump = false;
    private bool _isFacingRight = true;

    const float speedMultiplier = 50f; // множитель скорости

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
            _rb.AddForce(new Vector2(0f, jumpForce)); // придание силы для прыжка
            _isGround = false;
            _isJump = false; // игрок в воздухе НЕ прыгает
        }

        // если игрок идет вправо, но смотрит влево, то поворачиваем его спрайт (изменяем scale)
        if (_horizontal > 0f && !_isFacingRight)
        {
            Flip();
        } // если идет влево, но смотрит вправо, то тоже поворачиваем
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

    /*поворот игрока в зависимости от направления движения*/
    private void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }
}