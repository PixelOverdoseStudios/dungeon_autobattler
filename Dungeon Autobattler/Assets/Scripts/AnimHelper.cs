using UnityEngine;

public class AnimHelper : MonoBehaviour
{
    private MonsterBrain monsterBrain;
    private HeroBrain heroBrain;

    private void Awake()
    {
        if(GetComponentInParent<MonsterBrain>() != null)
            monsterBrain = GetComponentInParent<MonsterBrain>();

        if (GetComponentInParent<HeroBrain>() != null)
            heroBrain = GetComponentInParent<HeroBrain>();
    }

    public void AttackIsDone()
    {
        if(monsterBrain != null) monsterBrain.AttackIsDone();
        if(heroBrain != null) heroBrain.AttackIsDone();
    }

    public void DeliverDamage()
    {
        if (monsterBrain != null) monsterBrain.DeliveryDamage();
        if (heroBrain != null) heroBrain.DeliveryDamage();
    }
}
