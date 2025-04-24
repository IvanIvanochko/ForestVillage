using System;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class MainCharacterController : MonoBehaviour
{
    public static MainCharacterController Instance;

    [Header("Essentials")]
    [SerializeField] FloatingJoystick _joystick;
    [SerializeField] EnemyManager _enemyManager;
    [Header("Character Settings")]
    [SerializeField] float _speed = 5f;
    [Header("Character Combat Settings")]
    [SerializeField] float _damageDistance;
    [Header("Character features")]
    public GameObject backpack;
    [SerializeField] private GameObject _axeGO;

    CharacterController _characterController;
    [HideInInspector] public Animator animator;

    List<Item> _backpackItemsGO = new List<Item>();

    Vector3 motion;

    bool _isStanding;
    
    static readonly int _isStandingHash = Animator.StringToHash("IsStanding"); 
    static readonly int _speedHash = Animator.StringToHash("Speed"); 
    static readonly int _isTreeCuttingHash = Animator.StringToHash("IsTreeCutting");
    static readonly int _currentAnimSpeed = Animator.StringToHash("CurrentAnimSpeed");

    public static readonly string AxeAnimString = "Standing Melee Combo Attack";

    public event EventHandler OnAxeAnimFinished;
    public void AxeAnimFinished() => OnAxeAnimFinished?.Invoke(this, EventArgs.Empty);

    bool _isEnemyOutOfReach;

    public float DamageDistance
    {
        get => _damageDistance;
    }

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        _characterController = GetComponent<CharacterController>();    
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _axeGO.SetActive(false);
    }

    void Update()
    {
        MoveCharacter();
        Combat();
    }
    void MoveCharacter()
    {
        if (_joystick.Direction != Vector2.zero)
        {
            _isStanding = false;

            motion = new Vector3(_joystick.Direction.x, 0, _joystick.Direction.y);

            ForestVillage.Math.RotateCharacter(_joystick.Direction.x, _joystick.Direction.y, transform);

            _characterController.Move(motion * Time.deltaTime * _speed);

            animator.SetBool(_isStandingHash, _isStanding);

            Vector2 absDir = new Vector2(Mathf.Abs(_joystick.Direction.x), Mathf.Abs(_joystick.Direction.y));
            animator.SetFloat(_speedHash, (absDir.x > absDir.y) ? absDir.x : absDir.y);
        }
        else
        {
            _isStanding = true;

            animator.SetBool(_isStandingHash, _isStanding);
        }
    }

    #region Combat
    void Combat()
    {
        if (_enemyManager.IsEnemiesNull()) return;

        if (_enemyManager.GetClosestEnemy() > 0
            && _enemyManager.GetClosestEnemy() < _damageDistance && _isEnemyOutOfReach)
        {
            AxeAnimation(3);      

            _isEnemyOutOfReach = false;
        }
        if (_enemyManager.GetClosestEnemy() > _damageDistance && !_isEnemyOutOfReach)
        {
            DefaultUpperBodyAnimation();

            _isEnemyOutOfReach = true;
        }
        if (_enemyManager.GetClosestEnemy() > 0
            && _enemyManager.GetClosestEnemy() < _damageDistance)
        {
            Vector3 enemy = _enemyManager.GetClosestEnemyController().transform.position;
            Vector3 rotate = (enemy - transform.position).normalized;

            ForestVillage.Math.RotateCharacter(rotate.x, rotate.z, transform);
        }
    }

    #endregion
    
    #region Item
    public void AddItemToBackpack(Item item)
    {
        GameObject newItem = item.Instantiate(backpack.transform);

        newItem.transform.localPosition = new Vector3(0, _backpackItemsGO.Count * item.heightGap, 0);

        _backpackItemsGO.Add(newItem.GetComponent<Item>());
    }

    public void TransferItemToBackpack(Item item)
    {
        item.transform.parent = backpack.transform;

        item.transform.localRotation = item.Rotation();

        item.transform.localPosition = new Vector3(0, _backpackItemsGO.Count * item.heightGap, 0);

        _backpackItemsGO.Add(item);
    }

    public Item PopBackpackItem()
    {
        if(_backpackItemsGO.Count > 0)
        {
            var item = _backpackItemsGO[_backpackItemsGO.Count - 1];
            _backpackItemsGO.Remove(item);

            return item;
        }
        else 
            return null;
    }

    public Item PopBackpackItem(Item requiredItem)
    // Pops the last required item
    {
        if (_backpackItemsGO.Count > 0)
        {
            // Loop through the backpack items in reverse to find the required item
            for (int i = _backpackItemsGO.Count - 1; i >= 0; i--)
            {
                var item = _backpackItemsGO[i];

                if (item.ID == requiredItem.ID)
                {
                    _backpackItemsGO.RemoveAt(i);
                    return item;
                }
            }
        }

        return null;
    }

    public List<Item> BackPackItems()
    {
        return _backpackItemsGO;
    }

    public List<Item> BackPackItems(Item requiredItem)
    // returns a list with required Items
    {
        List<Item> matchingItems = new List<Item>();

        foreach (var item in _backpackItemsGO)
        {
            if (item.ID == requiredItem.ID)
            {
                matchingItems.Add(item);
            }
        }

        return matchingItems;
    }

    public void ClearBackPackItems()
    {
        _backpackItemsGO.Clear();
    }

    public bool ContainsItemInBackpack(Item item)
    {
        foreach (var backpackItem in _backpackItemsGO)
        {
            if (backpackItem.ID == item.ID)
            {
                return true;
            }
        }
        return false;
    }
    #endregion

    #region Animations
    public void AxeAnimation(float animSpeed)
    {
        animator?.SetBool(_isTreeCuttingHash, true);

        animator?.SetFloat(_currentAnimSpeed, animSpeed);

        _axeGO.SetActive(true);
    }

    public void DefaultUpperBodyAnimation()
    {
        animator?.SetBool(_isTreeCuttingHash, false);

        animator?.SetFloat(_currentAnimSpeed, 0);

        _axeGO.SetActive(false);
    }
    #endregion
}