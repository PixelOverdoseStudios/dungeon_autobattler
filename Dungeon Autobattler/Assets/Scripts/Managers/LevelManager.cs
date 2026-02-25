using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [field:SerializeField] public BossBrain DungeonBoss { get; private set; }
    [field:SerializeField] public List<HeroBrain> HeroesInDungeon { get; private set; }
    [field:SerializeField] public float HeroLineOfScrimmage { get; private set; }
    [field:SerializeField] public float EndOfDungeon { get; private set; }

    private void Awake()
    {
        if(instance != null && instance != this)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void AddHeroToDungeon(HeroBrain _hero)
    {
        if(HeroesInDungeon.Count > 0)
        {
            for(int i = 0; i < HeroesInDungeon.Count; i++)
                if (HeroesInDungeon[i] == _hero) return;
        }

        HeroesInDungeon.Add(_hero);
    }
    public void RemoveHeroFromDungeon(HeroBrain _hero)
    {
        for(int i = 0; i < HeroesInDungeon.Count; i++)
            if(HeroesInDungeon[i] == _hero) HeroesInDungeon.RemoveAt(i);
    }

    private void OnDrawGizmos()
    {
        // Draws a line of when heroes enter far enough in the dungeon for combat
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Vector3(HeroLineOfScrimmage, 15, 0), Vector3.down * 30);

        // Draws a line where the heroes will stay prioritizing the boss and can go after the gold
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(new Vector3(EndOfDungeon, 15, 0), Vector3.down * 30);
    }


    // Temp Testing
    [Header("Temp Spawn Testing")]
    [SerializeField] private bool doSpawnTesting;
    [SerializeField] private GameObject skeletonPrefab;
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private float monsterSpawnTimer = 1.5f;
    private float monsterCounter = 1f;
    [SerializeField] private float heroSpawnTimer = 6f;
    private float heroCounter = 5.5f;

    private void Start()
    {
        monsterCounter = monsterSpawnTimer - 0.5f;
        heroCounter = heroSpawnTimer - 0.5f;
    }

    private void Update()
    {
        if(doSpawnTesting)
        {
            monsterCounter += Time.deltaTime;

            if (monsterCounter >= monsterSpawnTimer)
            {
                float randomY = Random.Range(-5f, 1.5f);
                Instantiate(skeletonPrefab, new Vector3(7, randomY, 0), Quaternion.identity);
                monsterCounter = 0;
            }

            heroCounter += Time.deltaTime;

            if (heroCounter >= heroSpawnTimer)
            {
                float randomY = Random.Range(-5f, 1.5f);
                Instantiate(heroPrefab, new Vector3(-16, randomY, 0), Quaternion.identity);
                heroCounter = 0;
            }
        }
        
    }
}
