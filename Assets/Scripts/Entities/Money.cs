using DG.Tweening;
using UnityEngine;

public class Money : MonoBehaviour
{
    [SerializeField] float _duration;

    MainCharacterController _player;

    Tweener tweenMove;


    private void Start()
    {
        _player = MainCharacterController.Instance;

        tweenMove = transform.DOMove(_player.transform.position, _duration);
        //tweenLookAt = transform.DOLookAt(_player.transform.position, _duration);
        //targetLastPos = _player.transform.position;
    }

    private void Update()
    {
        tweenMove.ChangeEndValue(_player.transform.position, true);
    }
}
