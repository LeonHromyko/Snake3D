namespace GamePlay
{
    public interface IGameEntity
    {
        void Spawn(IGameManager gameManager);
        void MakeDecision();
        void Move();
        void Die();
        void ApplyGameStateChanges();
    }
}