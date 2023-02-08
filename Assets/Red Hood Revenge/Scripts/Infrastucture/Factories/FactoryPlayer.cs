using UnityEngine;

public class FactoryPlayer : IService
{
    GameObject _playerPrefab;

    public FactoryPlayer(GameObject playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public GameObject BuildPlayer(Vector3 at)
        => Object.Instantiate(_playerPrefab, at, Quaternion.identity);
}
