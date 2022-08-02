
public class EnemyData 
{
    public readonly int enemyId;
    public readonly string enemyName;
    public readonly int maxHp;
    public readonly int attackPower;
    public readonly string description;

    public EnemyData(int id, string name, int hp, int attack, string desc)
    {
        enemyId = id;
        enemyName = name;
        maxHp = hp;
        attackPower = attack;
        description = desc;
    }
}
