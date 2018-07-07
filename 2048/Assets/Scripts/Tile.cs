using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    public int Number
    {
        get
        {
            return this.number;
        }
        set
        {
            this.number = value;
            if (value == 0)
            {
                this.SetEmpty();
            }
            else
            {
                this.ApplyStyleFromHolder(this.number);
                this.SetVisible();
            }
        }
    }

    public int rowIndex;
    public int colIndex;

    private int number;
    private Text tileText;
    private Image tileImage;
    private Animator anim;

    void Awake()
    {
        this.anim = GetComponent<Animator>();
        this.tileText = GetComponentInChildren<Text>();
        this.tileImage = transform.Find("NumberedCell").GetComponent<Image>();
    }
    
    public void PlayAppearAnimation()
    {
        Debug.Log("Play appear animation");
        this.anim.SetTrigger("Appear");
    }

    public void PlayMergeAnimation()
    {
        Debug.Log("Play merge animation");
        this.anim.SetTrigger("Merge");
    }


    private void ApplyStyleFromHolder(int number)
    {
        TileStyle tileStyle = TileStyleManager.GetTileStyle(number);
        this.tileText.text = tileStyle.number.ToString();
        this.tileText.color = tileStyle.textColor;
        this.tileImage.color = tileStyle.tileColor;
    }

    private void SetVisible()
    {
        this.tileText.enabled = true;
        this.tileImage.enabled = true;
    }

    private void SetEmpty()
    {
        this.tileText.enabled = false;
        this.tileImage.enabled = false;
    }
}
