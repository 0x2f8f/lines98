using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    Button[,] buttons;
    Image[] images;
    Lines lines;
    public AudioSource audio;

    void Start()
    {
        lines = new Lines(ShowBox, PlayCut);
        InitButtons();
        InitImages();
        lines.Start();
    }

    public void ShowBox(int x, int y, int ball)
    {
        buttons[x, y].GetComponent<Image>().sprite = images[ball].sprite;
    }

    public void PlayCut()
    {
        audio.Play();
    }

    public void Click()
    {
        string buttonName = EventSystem.current.currentSelectedGameObject.name;
        int buttonNum = this.GetNumber(buttonName);
        int x = buttonNum % Lines.SIZE;
        int y = buttonNum / Lines.SIZE;
        //Debug.Log(buttonName + " - " + buttonNum + $", x={x}, y={y}");
        lines.Click(x, y);
    }

    private void InitButtons()
    {
        buttons = new Button[Lines.SIZE, Lines.SIZE];
        for(int i=0; i<Lines.SIZE*Lines.SIZE; i++) {
            buttons[i % Lines.SIZE, i / Lines.SIZE] = GameObject.Find($"Button ({i})").GetComponent<Button>();
        }
    }

    private void InitImages()
    {
        images = new Image[Lines.BALLS];
        for (int i = 0; i < Lines.BALLS; i++) {
            images[i] = GameObject.Find($"Image ({i})").GetComponent<Image>();
        }
    }

    private int GetNumber(string name)
    {
        Regex regex = new Regex("\\((\\d+)\\)");
        Match match = regex.Match(name);
        if (!match.Success) {
            throw new System.Exception("Unrecognized object name");
        }


        return Convert.ToInt32(match.Groups[1].Value);

    }
}
