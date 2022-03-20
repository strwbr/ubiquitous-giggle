using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /*
     * ¬раги патрулируют местность, т.е. ход€т между двум€ точками
     * ѕатрулируют между двум€ точками, ждут некоторое врем€ в каждой из точек
     * и возвращаютс€ в противоположную точку.
     * 
     * ѕри преследовании игрока (когда тот попал в зону видимости врага) враг игнорирует две эти точки и не ждет
     */

    [SerializeField] private float walkDistance = 10f; // длина патрул€
    [SerializeField] private float patrolSpeed = 1f; // ск-ть при патруле
    [SerializeField] private float chasingSpeed = 5f; // ск-ть при преследовании
    [SerializeField] private float timeToWait = 5f; // врем€ "сто€нки"
    [SerializeField] private float timeToChase = 3f; // врем€ преследовани€

    private Rigidbody2D _rb;
    private Vector2 _leftBoundaryPosition; // координаты левой точки патрул€
    private Vector2 _rightBoundaryPosition; // ... правой точки ...
    private Transform _playerTranform;
    private Vector2 _nextPoint;

    private bool _isWait = false;
    private bool _isChasingPlayer;
    private bool _collidedWithPlayer; // ударилс€ ли игрок о коллайдер врага  
    private bool _isFacingRight = true;

    private float _walkSpeed; // текущ€€ ск-ть (либо = patrolSpeed, либо = chasingSpeed)
    private float _waitTime;
    private float _chaseTime;

    private void Start()
    {
        _playerTranform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();

        _leftBoundaryPosition = transform.position; // лева€ т. = текущее расположение
        // т.к. right... и walk... - разные типы, то прибавл€ем к left... новый вектор (6,0)
        _rightBoundaryPosition = _leftBoundaryPosition + Vector2.right * walkDistance; // Vector2.right = new Vector2(1,0)

        _waitTime = timeToWait;
        _chaseTime = timeToChase;

        _walkSpeed = patrolSpeed;// изначально враг в режиме патрул€ => он имеет ск-ть при патруле
    }

    private void Update()
    {
        if (_isWait && !_isChasingPlayer)
        {
            // т.к. Update() вызываетс€ каждый фрейм
            StartWaitTimer();
        }

        if (_isChasingPlayer)
        {
            StartChasingTimer();
        }

        if (ShouldWait())
        {
            _isWait = true;
        }
    }

    private void FixedUpdate()
    {
        _nextPoint = Vector2.right * _walkSpeed * Time.fixedDeltaTime; // ск-ть перемещени€

        if(_isChasingPlayer && _collidedWithPlayer)
        {
            return;
        }

        if (_isChasingPlayer)
        {
            ChasePlayer();
        }

        // если враг Ќ≈ ждет и Ќ≈ преследует игрока, то он должен патрулировать
        if (!_isWait && !_isChasingPlayer)
        {
            Patrol();
        }
    }

    /*–ежим патрул€*/
    private void Patrol()
    {
        // первоначально враг идет вправо с положительной ск-тью
        // если смотрит влево, то мен€ем направление, т.е. придаем отрицательную ск-ть
        if (!_isFacingRight)
        {
            _nextPoint *= -1;
        }

        // перемещение врага
        _rb.MovePosition((Vector2)transform.position + _nextPoint);
    }

    /*–ежим преследовани€*/
    private void ChasePlayer()
    {
        float distance = DistanceToPlayer();

        if (distance < 0f)
        {
            _nextPoint *= -1;
        }

        if (distance > 0.2f && !_isFacingRight)
        {
            Flip();
        }  else if (distance < 0.2f && _isFacingRight)
        {
            Flip();
        }

        _rb.MovePosition((Vector2)transform.position + _nextPoint);
    }

    /*јктиваци€ режима преcледовани€*/
    private void StartChasingPlayer()
    {
        _isChasingPlayer = true;
        _chaseTime = timeToChase;
        _walkSpeed = chasingSpeed;
    }

    /*–ежим преследовани€ - таймер*/
    private void StartChasingTimer()
    {
        _chaseTime -= Time.deltaTime;
        if (_chaseTime < 0f)
        {
            _chaseTime = timeToChase;
            _isChasingPlayer = false;
            Flip();
        }
    }

    /*–ежим ожидани€ - таймер*/
    private void StartWaitTimer()
    {
        _waitTime -= Time.deltaTime;
        if (_waitTime < 0f)
        {
            _waitTime = timeToWait;
            _isWait = false;
            Flip();
        }
    }

    /*‘ункци€ проверки, должен ли враг ждать*/
    private bool ShouldWait()
    {
        bool isOutOfRightBoundary = _isFacingRight && transform.position.x >= _rightBoundaryPosition.x;
        bool isOutOfLeftBoundary = !_isFacingRight && transform.position.x <= _leftBoundaryPosition.x;
        return isOutOfLeftBoundary || isOutOfRightBoundary;
    }

    /*–ассто€ние между игроком и врагом*/
    // >0 - игрок справа от врага
    private float DistanceToPlayer()
    {
        return _playerTranform.position.x - transform.position.x;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _collidedWithPlayer = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _collidedWithPlayer = false;
        }
    }

    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_leftBoundaryPosition, _rightBoundaryPosition);
    }
}
