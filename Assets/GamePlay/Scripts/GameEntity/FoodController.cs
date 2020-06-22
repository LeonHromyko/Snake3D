namespace GamePlay
{
    public class FoodController : BaseGameEntity
    {
        public override void Spawn(IGameManager gameManager)
        {
            base.Spawn(gameManager);

            var cell = _gameManager.GetRandomEmptyCell();
            _cells.Add(cell);
            _gameManager.AssignGameEntityToGameFieldCell(this, cell);
            transform.position = _gameManager.GameCellToWorldCoords(cell);
        }

        public override void MakeDecision()
        {
        }

        public override void Move()
        {
        }
    }
}