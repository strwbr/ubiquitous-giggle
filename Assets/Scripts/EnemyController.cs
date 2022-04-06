using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /*
     * Враги патрулируют местность, т.е. ходят между двумя точками
     * Патрулируют между двумя точками, ждут некоторое время в каждой из точек
     * и возвращаются в противоположную точку.
     * 
     */

    [SerializeField] private float walkDistance = 10f; // длина патруля
    [SerializeField] private float patrolSpeed = 1f; // ск-ть при патруле
    [SerializeField] private float chasingSpeed = 5f; // ск-ть при преследовании
    [SerializeField] private float timeToWait = 5f; // время "стоянки"
    [SerializeField] private float timeToChase = 3f; // время преследования

    private Rigidbody2D _rb;
    private Vector2 _leftBoundaryPosition; // координаты левой точки патруля
    private Vector2 _rightBoundaryPosition; // ... правой точки ...
    private Transform _playerTranform;
    private Vector2 _nextPoint;

    private bool _isWait = false;
    private bool _isChasingPlayer;
    private bool _collidedWithPlayer; // ударился ли игрок о коллайдер врага  
    private bool _isFacingRight = true;

    private float _walkSpeed; // текущяя ск-ть (либо = patrolSpeed, либо = chasingSpeed)
    private float _waitTime;
    private float _chaseTime;

    private void Start()
    {
        _playerTranform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();

        _leftBoundaryPosition = transform.position; // левая т. = текущее расположение
        // т.к. right... и walk... - разные типы, то прибавляем к left... новый вектор (6,0)
        _rightBoundaryPosition = _leftBoundaryPosition + Vector2.right * walkDistance; // Vector2.right = new Vector2(1,0)

        _waitTime = timeToWait;
        _chaseTime = timeToChase;

        _walkSpeed = patrolSpeed;// изначально враг в режиме патруля => он имеет ск-ть при патруле
    }

    private void Update()
    {
        if (_isWait && !_isChasingPlayer)
        {
            // т.к. Update() вызывается каждый фрейм
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
        _nextPoint = Vector2.right * _walkSpeed * Time.fixedDeltaTime; // ск-ть перемещения

        if(_isChasingPlayer && _collidedWithPlayer)
        {
            return;
        }

        if (_isChasingPlayer)
        {
            ChasePlayer();
        }

        // если враг НЕ ждет и НЕ преследует игрока, то он должен патрулировать
        if (!_isWait && !_isChasingPlayer)
        {
            Patrol();
        }
    }

    /*Режим патруля*/
    private void Patrol()
    {
        // первоначально враг идет вправо с положительной ск-тью
        // если смотрит влево, то меняем направление, т.е. придаем отрицательную ск-ть
        if (!_isFacingRight)
        {
            _nextPoint *= -1;
        }

        // перемещение врага
        _rb.MovePosition((Vector2)transform.position + _nextPoint);
    }

    /*Режим преследования*/
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

    /*Активация режима преcледования*/
    private void StartChasingPlayer()
    {
        _isChasingPlayer = true;
        _chaseTime = timeToChase;
        _walkSpeed = chasingSpeed;
    }

    /*Режим преследования - таймер*/
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

    /*Режим ожидания - таймер*/
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

    /*Функция проверки, должен ли враг ждать*/
    private bool ShouldWait()
    {
        bool isOutOfRightBoundary = _isFacingRight && transform.position.x >= _rightBoundaryPosition.x;
        bool isOutOfLeftBoundary = !_isFacingRight && transform.position.x <= _leftBoundaryPosition.x;
        return isOutOfLeftBoundary || isOutOfRightBoundary;
    }

    /*Расстояние между игроком и врагом*/
    // >0 - игрок справа от врага
    private float DistanceToPlayer()
    {
        return _playerTranform.position.x - transform.position.x;
    }

    /*Соприкосновение с коллайдером другого объекта*/
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _collidedWithPlayer = true;
        }
    }

    /*Выход из коллайдера другого объекта*/
    private void OnCollisionExit2D(Collision2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            _collidedWithPlayer = false;
        }
    }

    /*Поворот объекта в зависимости от направления движения*/
    void Flip()
    {
        _isFacingRight = !_isFacingRight;
        Vector3 playerScale = transform.localScale;
        playerScale.x *= -1;
        transform.localScale = playerScale;
    }

    /*Красная линия - показывает путь патруля*/
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(_leftBoundaryPosition, _rightBoundaryPosition);
    }
}