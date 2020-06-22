using System.Collections.Generic;
using UnityEngine;

namespace GamePlay
{
    public interface IGameManager
    {
        GameSettings Settings { get; }

        HashSet<IGameEntity> GetGameFieldCellEntities(GameFieldCell cell);
        HashSet<IGameEntity> GetGameFieldCellEntities(int x, int y, int z);
        void RegisterGameEntity(IGameEntity gameEntity);
        void UnregisterGameEntity(IGameEntity gameEntity);
        void AssignGameEntityToGameFieldCell(IGameEntity gameEntity, GameFieldCell cell);
        void UnassignGameEntityToGameFieldCell(IGameEntity gameEntity, GameFieldCell cell);
        GameFieldCell GetRandomEmptyCell();
        Vector3 GameCellToWorldCoords(int x, int y, int z);
        Vector3 GameCellToWorldCoords(GameFieldCell cell);
        bool IsCellInsideGameField(GameFieldCell cell);
    }
}