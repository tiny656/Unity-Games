using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Text ScoreNumber;
    public Text HighScoreNumber;

    public int Score
    {
        get { return this.score; }
        set
        {
            this.score = value;
            this.ScoreNumber.text = this.score.ToString();
            if (this.score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", this.score);
                this.HighScoreNumber.text = this.score.ToString();
            }
        }
    } 

    private static ScoreManager instance;
    private int score;

    private ScoreManager()
    {

    }

    void Awake()
    {
        instance = this;
        if (!PlayerPrefs.HasKey("HighScore"))
            PlayerPrefs.SetInt("HighScore", 0);
        this.ScoreNumber.text = "0";
        this.score = 0;
        this.HighScoreNumber.text = PlayerPrefs.GetInt("HighScore").ToString();
    }

    public static ScoreManager GetInstance()
    {
        return instance;
    }
}
