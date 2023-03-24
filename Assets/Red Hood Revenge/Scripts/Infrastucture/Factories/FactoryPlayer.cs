using UnityEngine;

public class FactoryPlayer : IService
{
    readonly Player _playerPrefab;

    public FactoryPlayer(Player playerPrefab)
    {
        _playerPrefab = playerPrefab;
    }

    public Player BuildPlayer(Vector3 at) => Object.Instantiate(_playerPrefab, at, Quaternion.identity);

    public Player BuildPlayer(Transform at) => Object.Instantiate(_playerPrefab, at.position, at.rotation);
}
