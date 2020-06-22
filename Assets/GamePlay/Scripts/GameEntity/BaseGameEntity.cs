using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public abstract class BaseGameEntity : MonoBehaviour, IGameEntity
    {
        protected IGameManager _gameManager;
        protected readonly List<GameFieldCell> _cells = new List<GameFieldCell>();
        protected bool _die;

        public virtual void Spawn(IGameManager gameManager)
        {
            _gameManager = gameManager;
            _gameManager.RegisterGameEntity(this);
        }

        public virtual void Die()
        {
            _die = true;
        }
        
        public abstract void MakeDecision();
        public abstract void Move();

        public virtual void ApplyGameStateChanges()
        {
            if (_die)
            {
                Destroy(gameObject);
                _gameManager.UnregisterGameEntity(this);
            
                foreach (var cell in _cells)
                {
                    _gameManager.UnassignGameEntityToGameFieldCell(this, cell);
                }
            }
        }
    }
}