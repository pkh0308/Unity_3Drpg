public interface IEnemy
{
    int EnemyId { get; }
    int MaxHp { get; }
    int CurHp { get; }
    int AttackPower { get; }

    void Move();
    void OnDamaged(int dmg);
    void OnDie();
}
