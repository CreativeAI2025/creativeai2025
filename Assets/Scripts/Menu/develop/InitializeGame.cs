using UnityEngine;

public class InitializeGame : MonoBehaviour
{
    [SerializeField] private GameInitializer _initializer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _initializer.InitializeGame();
    }
}
