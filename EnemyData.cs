
public class EnemyData 
{
    public readonly int enemyId;
    public readonly string enemyName;
    public readonly int maxHp;
    public readonly string description;

    public EnemyData(int id, string name, int hp, string desc)
    {
        enemyId = id;
        enemyName = name;
        maxHp = hp;
        description = desc;
    }
}
