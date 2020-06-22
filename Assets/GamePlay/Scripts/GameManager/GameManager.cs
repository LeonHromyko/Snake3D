using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay
{
    public struct GameFieldCell
    {
        public int X;
        public int Y;
        public int Z;

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }
    }
    
    public class GameManager : MonoBehaviour, IGameManager
    {
        [SerializeField]
        private Transform _gameFieldRoot;

        [SerializeField]
        private GameSettings _settings;
        public GameSettings Settings => _settings;

        private HashSet<IGameEntity>[,,] _gameField;
        public HashSet<IGameEntity> GetGameFieldCellEntities(GameFieldCell cell) => _gameField[cell.X, cell.Y, cell.Z];
        public HashSet<IGameEntity> GetGameFieldCellEntities(int x, int y, int z) => _gameField[x, y, z];
        
        private readonly List<IGameEntity> _gameEntities = new List<IGameEntity>();
        private readonly List<IGameEntity> _gameEntitiesToUnregister = new List<IGameEntity>();
        private float _tickTime;

        private void Start()
        {
            GenerateGameField();
        }

        private void Update()
        {
            if (_tickTime > Time.time) return;

            foreach (var gameEntity in _gameEntities)
            {
                gameEntity.MakeDecision();
            }
            
            foreach (var gameEntity in _gameEntities)
            {
                gameEntity.Move();
            }
            
            ApplyGameStateChanges();
            
            foreach (var gameEntity in _gameEntities)
            {
                gameEntity.ApplyGameStateChanges();
            }

            foreach (var gameEntity in _gameEntitiesToUnregister)
            {
                _gameEntities.Remove(gameEntity);
            }
            
            _gameEntitiesToUnregister.Clear();
            _tickTime = Time.time + Settings.TickTime;
        }

        private void ApplyGameStateChanges()
        {
            var fieldSize = Settings.GameFieldSize;
            
            for (var x = 0; x < fieldSize; x++)
            {
                for (var y = 0; y < fieldSize; y++)
                {
                    for (var z = 0; z < fieldSize; z++)
                    {
                        if (_gameField[x, y, z].Count < 2) continue;

                        var foodInCell = false;
                        var snakesCount = 0;
                        
                        foreach (var entity in _gameField[x, y, z])
                        {
                            switch (entity)
                            {
                                case FoodController _:
                                    foodInCell = true;
                                    break;
                                
                                case SnakeController _:
                                    snakesCount++;
                                    break;
                            }
                        }

                        if (snakesCount > 1)
                        {
                            foreach (var entity in _gameField[x, y, z])
                            {
                                entity.Die();
                            }
                        }
                        else if (foodInCell)
                        {
                            foreach (var entity in _gameField[x, y, z])
                            {
                                switch (entity)
                                {
                                    case FoodController food:
                                        food.Die();
                                        break;
                                    
                                    case SnakeController snake:
                                        snake.Growth();
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void GenerateGameField()
        {
            var fieldSize = Settings.GameFieldSize;
            _gameField = new HashSet<IGameEntity>[fieldSize, fieldSize, fieldSize];
            
            for (var x = 0; x < fieldSize; x++)
            {
                for (var y = 0; y < fieldSize; y++)
                {
                    for (var z = 0; z < fieldSize; z++)
                    {
                        _gameField[x,y,z] = new HashSet<IGameEntity>();
                        Instantiate(Settings.GameFieldCellPrefab,
                            GameCellToWorldCoords(x, y, z),
                            Quaternion.identity,
                            _gameFieldRoot);
                    }
                }
            }

            SpawnGameEntities(Settings.SnakePrefab, Settings.SnakesCount);
            SpawnGameEntities(Settings.FoodPrefab, Settings.FoodCount);
        }
        
        private void SpawnGameEntities(GameObject prefab, int count)
        {
            for (var i = 0; i < count; i++)
            {
                var go = Instantiate(prefab);
                var gameEntity = go.GetComponent<IGameEntity>();
                gameEntity.Spawn(this);
            }
        }

        public Vector3 GameCellToWorldCoords(GameFieldCell cell)
            => GameCellToWorldCoords(cell.X, cell.Y, cell.Z);

        public bool IsCellInsideGameField(GameFieldCell cell)
        {
            var fieldSize = Settings.GameFieldSize;
            
            return cell.X >= 0 && cell.X < fieldSize &&
                   cell.Y >= 0 && cell.Y < fieldSize &&
                   cell.Z >= 0 && cell.Z < fieldSize;
        }

        public Vector3 GameCellToWorldCoords(int x, int y, int z)
        {
            var fieldSize = Settings.GameFieldSize;
            var cellSize = Settings.GameFieldCellSize;
            var offset = Vector3.one * fieldSize * cellSize / 2f;
            return new Vector3(x * cellSize, y * cellSize, z * cellSize) - offset;
        }

        public void RegisterGameEntity(IGameEntity gameEntity)
        {
            _gameEntities.Add(gameEntity);
        }

        public void UnregisterGameEntity(IGameEntity gameEntity)
        {
            _gameEntitiesToUnregister.Add(gameEntity);
        }

        public void AssignGameEntityToGameFieldCell(IGameEntity gameEntity, GameFieldCell cell)
        {
            _gameField[cell.X, cell.Y, cell.Z].Add(gameEntity);
        }

        public void UnassignGameEntityToGameFieldCell(IGameEntity gameEntity, GameFieldCell cell)
        {
            _gameField[cell.X, cell.Y, cell.Z].Remove(gameEntity);
        }

        public GameFieldCell GetRandomEmptyCell()
        {
            var fieldSize = Settings.GameFieldSize;

            while (true)
            {
                var x = Random.Range(0, fieldSize);
                var y = Random.Range(0, fieldSize);
                var z = Random.Range(0, fieldSize);

                if (_gameField[x, y, z].Count == 0)
                {
                    return new GameFieldCell
                    {
                        X = x,
                        Y = y,
                        Z = z
                    };
                }
            }
        }
    }
}