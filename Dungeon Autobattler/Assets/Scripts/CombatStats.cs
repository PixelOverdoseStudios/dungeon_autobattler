using UnityEngine;

public class CombatStats : MonoBehaviour
{
    [field:SerializeField] public int Health { get; private set; }
    [field:SerializeField] public float MoveSpeed { get; private set; }
    [field:SerializeField] public float AttackCooldown { get; private set; }
    [field:SerializeField] public int PhysicalDamage { get; private set; }
    [field:SerializeField] public int MagicDamage { get; private set; }
    [field:SerializeField] public int CritChance { get; private set; }
    [field:SerializeField] public float CritBonus { get; private set; }
    [field:SerializeField] public int Armor { get; private set; }
    [field:SerializeField][field:Range(0, 100)] public int MagicResist { get; private set; }
    [field:SerializeField][field: Range(0, 100)] public float Evasion { get; private set; }
    [SerializeField] private bool showDamageStatsInConsole;

    private int currentHealth;

    private void Start()
    {
        currentHealth = Health;
    }

    public void TakeDamage(int _physicalDamage, int _magicDamage, bool _doesCrit, float critBonus)
    {
        if(DidDodgeAttack())
        {
            Debug.Log("Dodged Attack");
            _physicalDamage = 0;
            _magicDamage = 0;
        }
        else
        {
            if (_physicalDamage > 0)
            {
                if (_doesCrit)
                {
                    _physicalDamage = Mathf.RoundToInt(_physicalDamage * critBonus);
                }

                _physicalDamage -= Armor;

                if (_physicalDamage <= 0) _physicalDamage = 1;
            }

            if (_magicDamage > 0)
            {
                float percent = MagicResist * 0.01f;
                int damageToTakeOff = Mathf.RoundToInt(_magicDamage * percent);
                _magicDamage -= damageToTakeOff;
            }
        }

        int totalDamage = _physicalDamage + _magicDamage;

        currentHealth -= totalDamage;

        if(showDamageStatsInConsole)
            Debug.Log(gameObject.name + " took - Physical Damage: " + _physicalDamage + " Magic Damage: " + _magicDamage
                + " Did Crit? " + _doesCrit);

        if (currentHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private bool DidDodgeAttack()
    {
        if (Evasion == 0) return false;

        float evasionRoll = Random.Range(1, 100);

        if (evasionRoll <= Evasion) return true;
        else return false;
    }
}
