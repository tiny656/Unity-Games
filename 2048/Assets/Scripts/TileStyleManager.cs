using UnityEngine;

public class TileStyleManager : MonoBehaviour {

    private static TileStyleManager instance;

    [SerializeField]
    private TileStyle[] tileStyles;

    void Awake()
    {
        instance = this;
    }

    public static TileStyle GetTileStyle(int number)
    {
        TileStyle result = null;
        foreach (var tileStyle in instance.tileStyles)
        {
            if (number == tileStyle.number)
            {
                result = tileStyle;
            }
        }
        return result;
    }
}
