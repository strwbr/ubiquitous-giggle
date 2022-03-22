using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    /*
     * ����� ����������� ���������, �.�. ����� ����� ����� �������
     * ����������� ����� ����� �������, ���� ��������� ����� � ������ �� �����
     * � ������������ � ��������������� �����.
     * 
     * ��� ������������� ������ (����� ��� ����� � ���� ��������� �����) ���� ���������� ��� ��� ����� � �� ����
     */

    [SerializeField] private float walkDistance = 10f; // ����� �������
    [SerializeField] private float patrolSpeed = 1f; // ��-�� ��� �������
    [SerializeField] private float chasingSpeed = 5f; // ��-�� ��� �������������
    [SerializeField] private float timeToWait = 5f; // ����� "�������"
    [SerializeField] private float timeToChase = 3f; // ����� �������������

    private Rigidbody2D _rb;
    private Vector2 _leftBoundaryPosition; // ���������� ����� ����� �������
    private Vector2 _rightBoundaryPosition; // ... ������ ����� ...
    private Transform _playerTranform;
    private Vector2 _nextPoint;

    private bool _isWait = false;
    private bool _isChasingPlayer;
    private bool _collidedWithPlayer; // �������� �� ����� � ��������� �����  
    private bool _isFacingRight = true;

    private float _walkSpeed; // ������� ��-�� (���� = patrolSpeed, ���� = chasingSpeed)
    private float _waitTime;
    private float _chaseTime;

    private void Start()
    {
        _playerTranform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        _rb = GetComponent<Rigidbody2D>();

        _leftBoundaryPosition = transform.position; // ����� �. = ������� ������������
        // �.�. right... � walk... - ������ ����, �� ���������� � left... ����� ������ (6,0)
        _rightBoundaryPosition = _leftBoundaryPosition + Vector2.right * walkDistance; // Vector2.right = new Vector2(1,0)

        _waitTime = timeToWait;
        _chaseTime = timeToChase;

        _walkSpeed = patrolSpeed;// ���������� ���� � ������ ������� => �� ����� ��-�� ��� �������
    }

    private void Update()
    {
        if (_isWait && !_isChasingPlayer)
        {
            // �.�. Update() ���������� ������ �����
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
        _nextPoint = Vector2.right * _walkSpeed * Time.fixedDeltaTime; // ��-�� �����������

        if(_isChasingPlayer && _collidedWithPlayer)
        {
            return;
        }

        if (_isChasingPlayer)
        {
            ChasePlayer();
        }

        // ���� ���� �� ���� � �� ���������� ������, �� �� ������ �������������
        if (!_isWait && !_isChasingPlayer)
        {
            Patrol();
        }
    }

    /*����� �������*/
    private void Patrol()
    {
        // ������������� ���� ���� ������ � ������������� ��-���
        // ���� ������� �����, �� ������ �����������, �.�. ������� ������������� ��-��
        if (!_isFacingRight)
        {
            _nextPoint *= -1;
        }

        // ����������� �����
        _rb.MovePosition((Vector2)transform.position + _nextPoint);
    }

    /*����� �������������*/
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

    /*��������� ������ ���c���������*/
    private void StartChasingPlayer()
    {
        _isChasingPlayer = true;
        _chaseTime = timeToChase;
        _walkSpeed = chasingSpeed;
    }

    /*����� ������������� - ������*/
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

    /*����� �������� - ������*/
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

    /*������� ��������, ������ �� ���� �����*/
    private bool ShouldWait()
    {
        bool isOutOfRightBoundary = _isFacingRight && transform.position.x >= _rightBoundaryPosition.x;
        bool isOutOfLeftBoundary = !_isFacingRight && transform.position.x <= _leftBoundaryPosition.x;
        return isOutOfLeftBoundary || isOutOfRightBoundary;
    }

    /*���������� ����� ������� � ������*/
    // >0 - ����� ������ �� �����
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
