using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    [System.Serializable]
    public class State
    {
        public Color fillcolor;
        public Color outlinecolor;
    }


    private TextMeshProUGUI text;
    private Image fill;
    private Outline outline;
    public State state { get; private set; }
    public char letter { get;private set; }

    private void Awake()
    {
        fill = GetComponent<Image>();
        outline = GetComponent<Outline>();
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetLetter(char letter)
    {
        this.letter = letter;
        text.text = letter.ToString();
    }

    public void SetStates(State state)
    {
        this.state = state;
        fill.color = state.fillcolor;
        outline.effectColor = state.outlinecolor;
    }
}
