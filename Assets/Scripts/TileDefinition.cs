using UnityEngine;

[CreateAssetMenu(fileName = "TileDefinition", menuName = "Space and Time/Tile Definition")]
public class TileDefinition : ScriptableObject
{
    public string displayName;
    public bool isMovable = true;
    public Color colour = Color.yellow;
}
