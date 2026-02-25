using System.Collections.Generic;
using UnityEngine;

public class MonsterBrain : MonoBehaviour
{
    private CombatStats combatStats;
    private Rigidbody2D rb;
    private Animator animator;
    private MonsterState monsterState = MonsterState.Idle;
    private Vector3 moveDirection;
    private float attackTimer;

    [Header("Misc")]
    //[SerializeField] private float distanceToStopFromTarget = 1f;
    [SerializeField] private GameObject target;
    [field:SerializeField] public HeroBattlePos BattlePosition { get; set; }


    //TODOS:
    // - if target becomes inactive and not destroyed monster still tries to fight
    // - Need to figure out how to update hero list of monsters nearby to fight, but make it so it only checks and calls it once instead of multiple times it attacking

    private void Awake()
    {
        SetUpAllAttachedComponents();
    }

    private void Update()
    {
        BasicMovementAndFacingDirectionLogic();

        if(attackTimer > 0f) attackTimer -= Time.deltaTime;

        if(target == null) BattlePosition = null;

        switch (monsterState)
        { 
            case MonsterState.Idle:
                IdleLogic();
                break;
            case MonsterState.Moving:
                MovingLogic();
                break;
            case MonsterState.Attacking:
                AttackLogic();
                break;
        }
    }

    private void BasicMovementAndFacingDirectionLogic()
    {
        moveDirection.Normalize();

        rb.linearVelocity = moveDirection * combatStats.MoveSpeed;

        if(monsterState == MonsterState.Moving)
        {
            if (moveDirection.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (moveDirection.x > 0)
                transform.localScale = Vector3.one;
        }
        else if(target != null)
        {
            if(target.transform.position.x < transform.position.x)
                transform.localScale = new Vector3(-1, 1, 1);
            else if(target.transform.position.x > transform.position.x)
                transform.localScale = Vector3.one;
        }
    }

    private void IdleLogic()
    {
        if(target == null)
        {
            if(FindNearestHero() != null)
            {
                target = FindNearestHero();
                target.GetComponent<HeroBrain>().AddEnemyToList();
            }
        }

        if (target != null && BattlePosition == null)
        {
            BattlePosition = FindUnoccupiedBattlePos();
            if(BattlePosition != null) BattlePosition.AddMonsterToPosition(this);
        }

        if (target != null && BattlePosition != null && Vector3.Distance(transform.position, BattlePosition.transform.position) > 0.1f)
        {
            monsterState = MonsterState.Moving;
        }
        else if(target != null && BattlePosition != null && Vector3.Distance(transform.position, BattlePosition.transform.position) <= 0.1f)
        {
            AddMonsterToHeroesNearbyEnemyList();
            if (attackTimer <= 0) monsterState = MonsterState.Attacking;
        }

        moveDirection = Vector3.zero;
        animator.Play("idle");
    }

    private void MovingLogic()
    {
        if(target == null) monsterState = MonsterState.Idle;

        if(target != null && Vector3.Distance(transform.position, BattlePosition.transform.position) > 0.1f)
        {
            moveDirection = BattlePosition.transform.position - transform.position;
            animator.Play("moving");
        }
        else
        {
            monsterState = MonsterState.Idle;
        }     
    }

    private void AttackLogic()
    {
        attackTimer = combatStats.AttackCooldown;
        moveDirection = Vector3.zero;
        animator.Play("attack");
    }

    private bool DoesAttackCrit()
    {
        if (combatStats.CritChance == 0) return false;

        int critRoll = Random.Range(1, 100);

        if(critRoll <= combatStats.CritChance) return true;
        else return false;
    }

    private GameObject FindNearestHero()
    {
        if (LevelManager.instance.HeroesInDungeon.Count > 0)
        {
            HeroBrain nearestHero = null;

            foreach (HeroBrain hero in LevelManager.instance.HeroesInDungeon)
            {
                if(hero.NumberOfEnemies < 3)
                {
                    if (nearestHero == null) nearestHero = hero;
                    else
                    {
                        if (Vector3.Distance(hero.transform.position, transform.position) < Vector3.Distance(nearestHero.transform.position, transform.position))
                            nearestHero = hero;
                    }
                }
            }

            if (nearestHero != null) return nearestHero.gameObject;
            else return null;
        }
        else return null;
    }

    private HeroBattlePos FindUnoccupiedBattlePos()
    {
        List<HeroBattlePos> heroPos = target.GetComponent<HeroBrain>().BattlePositions;
        HeroBattlePos closestPos = null;

        foreach(HeroBattlePos battlePos in heroPos)
        {
            if(battlePos.monster == null && closestPos == null)
            {
                closestPos = battlePos;
            }
            else if(battlePos.monster == null && closestPos != null)
            {
                if(Vector3.Distance(battlePos.transform.position, transform.position) < Vector3.Distance(closestPos.transform.position, transform.position))
                {
                    closestPos = battlePos;
                }
            }
        }

        return closestPos;
    }

    private void AddMonsterToHeroesNearbyEnemyList()
    {
        if(target == null) return;

        HeroBrain hero = target.gameObject.GetComponent<HeroBrain>();

        if (hero.MonstersNearby.Count <= 0) hero.AddMonsterNearby(this);
        else
        {
            for(int i = 0; i < hero.MonstersNearby.Count; i++)
            {
                if (hero.MonstersNearby[i] == this) return;
            }

            hero.AddMonsterNearby(this);
        }
    }

    public void AttackIsDone() => monsterState = MonsterState.Idle;
    public void DeliveryDamage()
    {
        if(target != null)
            target.GetComponent<CombatStats>().TakeDamage(combatStats.PhysicalDamage, combatStats.MagicDamage, DoesAttackCrit(), combatStats.CritBonus);
    }
    private void SetUpAllAttachedComponents()
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
        animator = GetComponentInChildren<Animator>();
    }

    private void OnDestroy()
    {
        if(target != null)
        {
            target.gameObject.GetComponent<HeroBrain>().RemoveMonsterNearby(this);
            target.gameObject.GetComponent<HeroBrain>().RemoveEnemyToList();
        }     
        if (BattlePosition != null) BattlePosition.RemoveMonsterFromPosition();
    }
}

public enum MonsterState
{
    Idle,
    Moving,
    Attacking
}
