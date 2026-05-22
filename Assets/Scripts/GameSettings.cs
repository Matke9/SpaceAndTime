using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Space and Time/Game Settings")]
public class GameSettings : ScriptableObject
{
    private static GameSettings _current;

    public static GameSettings Current
    {
        get
        {
            if (_current == null)
                _current = Resources.Load<GameSettings>("Data/GameSettings");
            return _current;
        }
    }

    [Header("Timer / Health")]
    public float startingTime = 75f;
    public float maxTime = 75f;

    [Header("Player")]
    public float attackTimeCost = 5f;

    [Header("Enemy")]
    public float enemyAttackTimeDrain = 2f;

    [Header("Music")]
    public float musicMediumThreshold = 50f;
    public float musicFastThreshold = 25f;
}
