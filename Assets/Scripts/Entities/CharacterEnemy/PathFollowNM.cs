using UnityEngine;
using UnityEngine.AI;

public class PathFollowNM : MonoBehaviour, Character.ICharacterMove
{
    public Character character;
    [SerializeField] NavMeshAgent na;
    [SerializeField] Transform target;
    public bool canMove = false;
    void Start()
    {
        na = GetComponent<NavMeshAgent>();
        na.updateRotation = false;
        na.isStopped = true;
        na.enabled = false;
        character.characterInfo.isActive = true;
    }
    void Update()
    {
        if (canMove && target != null)
        {
            na.SetDestination(target.position);
        }
    }
    public Rigidbody GetRigidbody()
    {
        return null;
    }
    public void SetPositionTarget(Transform position)
    {
        target = position;
    }
    public void SetCanMoveState(bool state)
    {
        na.enabled = state;
        canMove = state;
        na.isStopped = !state;
    }
    public void Move(){}
    public void SetTarget(Transform targetPos)
    {
        target = targetPos;        
    }
}
