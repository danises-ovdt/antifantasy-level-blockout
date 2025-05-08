using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private MainCharacterController _mainCharacterController;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public Camera GetMainCamera()
    {
        return _mainCamera;
    }

    public void SetMainCamera(Camera mainCamera)
    {
        _mainCamera = mainCamera;
    }

    public MainCharacterController GetMainCharacterController()
    {
        return _mainCharacterController;
    }

    public void SetMainCharacterController(MainCharacterController mainCharacterController)
    {
        _mainCharacterController = mainCharacterController;
    }
}