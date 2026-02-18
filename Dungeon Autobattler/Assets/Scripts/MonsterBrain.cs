using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    private CombatStats combatStats;
    private Rigidbody2D rb;
    private MonsterState monsterState = MonsterState.Idle;
    private Vector3 moveDirection;
    [SerializeField] private GameObject target;

    [Header("Misc")]
    [SerializeField] private float distanceToStopFromTarget = 1f;

    private void Awake()
    {
        if (TryGetComponent<CombatStats>(out CombatStats stats))
        {
            combatStats = stats;
        }
        else
        {
            Debug.Log(gameObject.name + ": needs a CombatStats script attached");
            gameObject.SetActive(false);
        }

        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        BasicMovementAndFacingDirectionLogic();

        switch (monsterState)
        { 
            case MonsterState.Idle:
                break;
            case MonsterState.Moving:
                break;
            case MonsterState.Attacking:
                break;
        }
    }

    private void BasicMovementAndFacingDirectionLogic()
    {
        moveDirection.Normalize();

        rb.linearVelocity = moveDirection * combatStats.MoveSpeed;

        if (moveDirection.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (moveDirection.x > 0)
            transform.localScale = Vector3.one;
    }

    private void IdleLogic()
    {

    }
}

public enum MonsterState
{
    Idle,
    Moving,
    Attacking
}
