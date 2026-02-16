using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    private const float Delay = 2;

    [SerializeField] private Enemy _enemy;
    [SerializeField] private int _poolCapacity = 5;
    [SerializeField] private int _poolMaxSize = 5;
    [SerializeField] private GameObject[] _startPoint;
    [SerializeField] private float _speed;

    private ObjectPool<Enemy> _pool;

    private bool _isOpen = true;

    private int _min = 0;
    private float _horizontalLeft = -1f;
    private float _horizontalRight = 1f;
    private float _depthLeft = -1f;
    private float _depthRight = 1f;
    private float _vertical = 0f;

    private WaitForSeconds _wait = new WaitForSeconds(Delay);

    private void Awake() 
    {
    _pool = new ObjectPool<Enemy>(
        createFunc: () => { Enemy enemy = Instantiate(_enemy);
        return enemy;},
        actionOnGet: (enemy) => PrepareObject(enemy),
        actionOnRelease: (enemy) => enemy.gameObject.SetActive(false),
        actionOnDestroy: (enemy) => Destroy(enemy.gameObject),
        collectionCheck: true,
        defaultCapacity: _poolCapacity,
        maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(OnsetObject());
    }

    private void PrepareObject(Enemy enemy)
    {
        enemy.ResetPosition();
        enemy.gameObject.SetActive(true);
    }

    private void GetSpawnPoint(Enemy enemy)
    {
        int index = Random.Range(_min, _startPoint.Length);
        enemy.transform.position = _startPoint[index].transform.position;
    }

    private void ReturnEnemyPool(Enemy enemy)
    {
        enemy.LifeTimeEnded -= ReturnEnemyPool;
        _pool.Release(enemy);
    }

    private void GetDirection(Enemy enemy)
    {
        Rigidbody rigidbody = enemy.GetComponent<Rigidbody>();
        Vector3 direction = new Vector3(Random.Range(_horizontalLeft, _horizontalRight), _vertical, Random.Range(_depthLeft, _depthRight)).normalized;
        rigidbody.velocity = direction * _speed;
    }

    private IEnumerator OnsetObject()
    {
        while (_isOpen)
        {
            if(_startPoint != null && _startPoint.Length != 0)
            {
                Enemy enemy = _pool.Get();
                GetSpawnPoint(enemy);
                GetDirection(enemy);
                enemy.LifeTimeEnded += ReturnEnemyPool;
            }

            yield return _wait;
        }
    }
}