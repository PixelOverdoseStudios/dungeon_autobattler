using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class HeroBrain : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 moveDirection;
    private HeroState heroState = HeroState.Moving;
    private CombatStats combatStats;
    [field:SerializeField] public int NumberOfEnemies { get; private set; }
    [field:SerializeField] public List<MonsterBrain> MonstersNearby { get; private set; }
    [field:SerializeField] public List<HeroBattlePos> BattlePositions { get; private set; }
    [SerializeField] private GameObject target;

    private float attackTimer;
    private bool enteredDungeon = false;
    private bool endOfDungeon = false;

    private void Awake()
    {
        combatStats = GetComponent<CombatStats>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        moveDirection.Normalize();

        rb.linearVelocity = moveDirection * combatStats.MoveSpeed;

        DungeonBooleanConditions();

        if(attackTimer > 0) attackTimer -= Time.deltaTime;

        switch (heroState)
        {
            case HeroState.Idle:
                IdleLogic();
                break;
            case HeroState.Moving:
                MovingLogic();
                break;
            case HeroState.Attack:
                AttackLogic();
                break;
        }
    }

    private void IdleLogic()
    {
        moveDirection = Vector3.zero;

        if (target == null) heroState = HeroState.Moving;

        if (attackTimer <= 0) heroState = HeroState.Attack;

        animator.Play("idle");
    }

    private void MovingLogic()
    {
        if (MonstersNearby.Count > 0)
        {
            if (target == null)
            {
                int randomIndex = Random.Range(0, MonstersNearby.Count);
                target = MonstersNearby[randomIndex].gameObject;
            }
            heroState = HeroState.Idle;
        }
        else
        {
            if (!endOfDungeon)
                moveDirection = new Vector3(1, 0, 0);
            else if (endOfDungeon)
            {
                moveDirection = LevelManager.instance.DungeonBoss.gameObject.transform.position - transform.position;
            }
        }

        animator.Play("moving");
    }

    private void AttackLogic()
    {
        moveDirection = Vector3.zero;
        attackTimer = combatStats.AttackCooldown;
        animator.Play("attack");
    }

    private bool DoesAttackCrit()
    {
        if (combatStats.CritChance == 0) return false;

        int critRoll = Random.Range(1, 100);

        if (critRoll <= combatStats.CritChance) return true;
        else return false;
    }

    private GameObject FindNearestEnemy()
    {
        GameObject closestMonster = null;

        if(target == null)
        {
            foreach(HeroBattlePos battlePos in BattlePositions)
            {
                if (battlePos.monster != null && closestMonster == null)
                    closestMonster = battlePos.monster.gameObject;
                else if (battlePos.monster != null &&
                    Vector3.Distance(battlePos.monster.transform.position, transform.position) < Vector3.Distance(closestMonster.transform.position, transform.position))
                    closestMonster = battlePos.monster.gameObject;
            }
        }

        return closestMonster;
    }

    private void DungeonBooleanConditions()
    {
        if(!enteredDungeon)
            if(transform.position.x >= LevelManager.instance.HeroLineOfScrimmage)
            {
                LevelManager.instance.AddHeroToDungeon(this);
                enteredDungeon = true;
            }

        if(!endOfDungeon)
            if(transform.position.x >= LevelManager.instance.EndOfDungeon)
            {
                endOfDungeon = true;
            }
    }

    public void AddEnemyToList() => NumberOfEnemies++;
    public void RemoveEnemyToList() => NumberOfEnemies--;
    public void AddMonsterNearby(MonsterBrain _monster) => MonstersNearby.Add(_monster);
    public void RemoveMonsterNearby(MonsterBrain _monster)
    {
        for (int i = 0; i < MonstersNearby.Count; i++)
            if (MonstersNearby[i] == _monster) MonstersNearby.RemoveAt(i);
    }

    public void AttackIsDone() => heroState = HeroState.Idle;
    public void DeliveryDamage()
    {
        if(target != null)
            target.GetComponent<CombatStats>().TakeDamage(combatStats.PhysicalDamage, combatStats.MagicDamage, DoesAttackCrit(), combatStats.CritBonus);
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        LevelManager.instance.RemoveHeroFromDungeon(this);
        foreach(HeroBattlePos battlePos in BattlePositions)
        {
            if (battlePos.monster != null) battlePos.monster.BattlePosition = null;
        }
    }
}

public enum HeroState
{ 
    Idle,
    Moving,
    MovingAcrossDungeon,
    Attack
}

