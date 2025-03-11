using UnityEngine;
using UnityEngine.AI;

public class PathFollowNM : MonoBehaviour
{
    [SerializeField] NavMeshAgent na;
    [SerializeField] GameObject target;
    public bool canMove = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        na = GetComponent<NavMeshAgent>();
        na.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            na.SetDestination(target.transform.position);
        }
    }
}
