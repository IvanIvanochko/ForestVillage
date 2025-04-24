using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IGetHealthSystem
// https://discussions.unity.com/t/idle-animation-moves-my-character/664779
// https://www.youtube.com/watch?v=wf6vtCgLk6w&feature=youtu.be&t=46m11s
{
    Animator _animator;
    NavMeshAgent _agent;

    [SerializeField] GameObject _model;
    [Header("Settings")]
    [SerializeField] bool _drawGizmos = true;
    [SerializeField] int _healthMax;
    [SerializeField] float _speed;
    [SerializeField] float _followDistance;

    static readonly int _speedHash = Animator.StringToHash("Speed");

    MainCharacterController _player;
    HealthSystem _healthSystem;

    private float _playerDistance;

    #region Draw Gizmos
    private void OnDrawGizmos()
    {
        if(_drawGizmos == false) return;

        DrawGizmoDisk(_followDistance);
    }
    private const float GIZMO_DISK_THICKNESS = 0.01f;

    public void DrawGizmoDisk(float radius)
    {
        Transform t = this.transform;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0.2f, 0.2f, 0.2f, 0.5f); //this is gray, could be anything
        var pos = new Vector3(t.position.x, t.position.y + 0.5f, t.position.z);
        Gizmos.matrix = Matrix4x4.TRS(pos, t.rotation, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Gizmos.DrawSphere(Vector3.zero, radius);
        Gizmos.matrix = oldMatrix;
    }
    #endregion
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _speed;

        _healthSystem = new HealthSystem(_healthMax);
    }

    private void _player_OnAxeAnimFinished(object sender, System.EventArgs e)
    {
        _healthSystem.Damage(10);
    }

    private void Start()
    {
        _player = MainCharacterController.Instance;
        _player.OnAxeAnimFinished += _player_OnAxeAnimFinished;
    }

    private void Update()
    {
        _animator.SetFloat(_speedHash, _agent.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        _playerDistance = Vector3.Distance(_player.transform.position, transform.position);

        if (_playerDistance > _followDistance)
        {
            _agent.SetDestination(_player.transform.position);
        }
        else
        {
            if(_agent.velocity.magnitude > 0)
                _agent.SetDestination(transform.position);
        }

        _model.transform.rotation = Quaternion.LookRotation(_player.transform.position - transform.position);
    }

    public HealthSystem GetHealthSystem()
    {
        return _healthSystem;
    }

    public float GetPlayerDistance()
    {
        return _playerDistance;
    }

    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(0, 0, 200, 200), "Damageee"))
    //    {
    //        _healthSystem.Damage(10);
    //    }
    //}
}
