using UnityEngine;

public class HeroBattlePos : MonoBehaviour
{
    // TODO: This class will check to see if a monster is nearby the point by checking distance, if so will go occupied and update the hero brain script.

    public bool isOccupied = false;
    public MonsterBrain monster = null;
    private HeroBrain heroBrain;

    private void Awake()
    {
        heroBrain = GetComponentInParent<HeroBrain>();
    }

    public void AddMonsterToPosition(MonsterBrain _monster) => monster = _monster;
    public void RemoveMonsterFromPosition() => monster = null;
}
