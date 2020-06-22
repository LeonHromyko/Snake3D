using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings")]
public class GameSettings : ScriptableObject
{
    [SerializeField]
    private float _tickTime = 0.5f;
    public float TickTime => _tickTime;
    
    [SerializeField]
    private GameObject _gameFieldCellPrefab;
    public GameObject GameFieldCellPrefab => _gameFieldCellPrefab;
        
    [SerializeField]
    private int _gameFieldSize = 15;
    public int GameFieldSize => _gameFieldSize;

    [SerializeField]
    private float _gameFieldCellSize = 1f;
    public float GameFieldCellSize => _gameFieldCellSize;

    [SerializeField]
    private int _snakesCount = 2;
    public int SnakesCount => _snakesCount;

    [SerializeField]
    private int _snakeStartLength = 3;
    public int SnakeStartLength => _snakeStartLength;

    [SerializeField]
    private GameObject _snakePrefab;
    public GameObject SnakePrefab => _snakePrefab;
    
    [SerializeField]
    private GameObject _snakePartPrefab;
    public GameObject SnakePartPrefab => _snakePartPrefab;

    [SerializeField]
    private int _foodCount = 10;
    public int FoodCount => _foodCount;

    [SerializeField]
    private GameObject _foodPrefab;
    public GameObject FoodPrefab => _foodPrefab;
}