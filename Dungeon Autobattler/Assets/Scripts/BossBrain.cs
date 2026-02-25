using UnityEngine;

public class BossBrain : MonoBehaviour
{
    private CombatStats combatStats;

    private void Awake()
    {
        combatStats = GetComponent<CombatStats>();
    }
}
