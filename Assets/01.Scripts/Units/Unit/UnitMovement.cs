using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    private Rigidbody2D rb;
    private NavMeshAgent agent;

    [SerializeField] private Vector2 destination;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();

        // 2D이므로 회전, 축 고정
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Move()
    {
        agent.isStopped = false;
        agent.speed = unit.Data.MoveSpeed;
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
    }
    public Vector2 GetDestination()
    {
        return destination;
    }
    public void SetDestination(Vector2 destination)
    {
        this.destination = destination;

        agent.isStopped = false;
        agent.SetDestination(destination);
    }
}
