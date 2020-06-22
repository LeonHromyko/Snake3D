using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay
{
    public class SnakeController : BaseGameEntity
    {
        private delegate GameFieldCell MoveDirectionFunc(GameFieldCell cell);
        
        private static readonly MoveDirectionFunc[] MoveDirectionFuncs =
        {
            cell => new GameFieldCell{X = cell.X + 1, Y = cell.Y, Z = cell.Z},
            cell => new GameFieldCell{X = cell.X - 1, Y = cell.Y, Z = cell.Z},
            
            cell => new GameFieldCell{X = cell.X, Y = cell.Y + 1, Z = cell.Z},
            cell => new GameFieldCell{X = cell.X, Y = cell.Y - 1, Z = cell.Z},
            
            cell => new GameFieldCell{X = cell.X, Y = cell.Y, Z = cell.Z + 1},
            cell => new GameFieldCell{X = cell.X, Y = cell.Y, Z = cell.Z - 1},
        };

        private readonly List<Transform> _snakeParts = new List<Transform>();
        private bool _growth;
        private MoveDirectionFunc _decisionMove;
        private GameFieldCell _prevLastCell;
        
        public override void Spawn(IGameManager gameManager)
        {
            base.Spawn(gameManager);
            var snakeLength = _gameManager.Settings.SnakeStartLength;

            while (true)
            {
                var startCell = _gameManager.GetRandomEmptyCell();
                var fit = true;
                
                for (var dX = 1; dX < snakeLength; dX++)
                {
                    if (startCell.X + dX >= _gameManager.Settings.GameFieldSize ||
                        _gameManager.GetGameFieldCellEntities(startCell.X + dX, startCell.Y, startCell.Z).Count != 0)
                    {
                        fit = false;
                        break;
                    }
                }

                if (fit)
                {
                    for (var dX = 0; dX < snakeLength; dX++)
                    {
                        var cell = new GameFieldCell
                        {
                            X = startCell.X + dX,
                            Y = startCell.Y,
                            Z = startCell.Z
                        };
                        
                        _gameManager.AssignGameEntityToGameFieldCell(this, cell);
                        
                        var snakePart = Instantiate(_gameManager.Settings.SnakePartPrefab,
                            _gameManager.GameCellToWorldCoords(cell),
                            Quaternion.identity,
                            transform);
                        _snakeParts.Add(snakePart.transform);
                        _cells.Add(cell);
                    }
                    
                    break;
                }
            }
        }

        public override void MakeDecision()
        {
            _growth = false;
            var bestPathLength = int.MaxValue;
            var moves = new Dictionary<MoveDirectionFunc, int>();

            foreach (var moveDirection in MoveDirectionFuncs)
            {
                var step = 0;
                var cell = _cells[0];
                
                while (true)
                {
                    if (++step > bestPathLength) break;
                    
                    cell = moveDirection(cell);
                    if (!_gameManager.IsCellInsideGameField(cell)) break;

                    if (_gameManager.GetGameFieldCellEntities(cell).Count > 0)
                    {
                        foreach (var entity in _gameManager.GetGameFieldCellEntities(cell))
                        {
                            if (entity is FoodController)
                            {
                                if (step < bestPathLength)
                                    bestPathLength = step;
                                
                                moves[moveDirection] = step;
                                break;
                            }
                        }
                        
                        break;
                    }

                    moves[moveDirection] = int.MaxValue;
                }
            }

            var bestMoves = new List<MoveDirectionFunc>();
            foreach (var kvp in moves)
            {
                if (kvp.Value == bestPathLength)
                    bestMoves.Add(kvp.Key);
            }

            if (bestMoves.Count == 0)
            {
                Die();
                return;
            }

            _decisionMove = bestMoves[Random.Range(0, bestMoves.Count)];
        }

        public override void Move()
        {
            if (_die) return;

            var newStartCell = _decisionMove(_cells[0]);
            _cells.Insert(0, newStartCell);
            _gameManager.AssignGameEntityToGameFieldCell(this, newStartCell);

            _prevLastCell = _cells[_cells.Count - 1];
            _cells.RemoveAt(_cells.Count - 1);
            _gameManager.UnassignGameEntityToGameFieldCell(this, _prevLastCell);
        }

        private Coroutine _moveCoroutine;
        public override void ApplyGameStateChanges()
        {
            base.ApplyGameStateChanges();
            
            if (_die) return;

            if (_moveCoroutine != null)
                StopCoroutine(_moveCoroutine);
            
            _moveCoroutine = StartCoroutine(MoveCoroutine());
        }

        private IEnumerator MoveCoroutine()
        {
            var time = 0f;
            var n = _snakeParts.Count;
            var startPos = new Vector3[n];
            var endPos = new Vector3[n];

            for (var i = 0; i < n; i++)
            {
                startPos[i] = _snakeParts[i].position;
                endPos[i] = _gameManager.GameCellToWorldCoords(_cells[i]);
            }

            if (_growth)
            {
                _cells.Add(_prevLastCell);
                _gameManager.AssignGameEntityToGameFieldCell(this, _prevLastCell);
                var snakePart = Instantiate(_gameManager.Settings.SnakePartPrefab,
                    _gameManager.GameCellToWorldCoords(_prevLastCell),
                    Quaternion.identity,
                    transform);
                _snakeParts.Add(snakePart.transform);
            }
            
            while (time < _gameManager.Settings.TickTime)
            {
                var stage = time / _gameManager.Settings.TickTime;
                
                for (var i = 0; i < n; i++)
                {
                    _snakeParts[i].position = Vector3.Lerp(startPos[i], endPos[i], stage);
                }

                time += Time.deltaTime;
                yield return null;
            }

            _moveCoroutine = null;
        }

        public void Growth()
        {
            _growth = true;
        }
    }
}