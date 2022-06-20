public interface IEnemy
{
    int EnemyId { get; }
    int MaxHp { get; }
    int CurHp { get; }

    void Move();
    void OnDamaged(int dmg);
    void OnDie();
}
