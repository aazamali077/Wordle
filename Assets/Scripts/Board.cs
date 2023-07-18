using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Board : MonoBehaviour
{

    private static readonly KeyCode[] SUPPORTED_KEY = new KeyCode[]
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F, KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L, KeyCode.M,
        KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R, KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X, KeyCode.Y, KeyCode.Z,
    };

    private Row[] rows;
    private int RowIndex, ColumnIndex;
    public static string word;

    [Header("States")]
    public Tile.State emptyState;
    public Tile.State OccupiedState;
    public Tile.State CorrectState;
    public Tile.State incorrectState;
    public Tile.State wrongspotState;

    public TextMeshProUGUI HintText;
    public Button Showhint, Hidehint, Tryagain, Newword;
    private void Awake()
    {
        rows = GetComponentsInChildren<Row>();  
    }

    private void Start()
    {
        Showhint.onClick.AddListener(() =>
        {
            HintText.gameObject.SetActive(true);
            Showhint.gameObject.SetActive(false);
            Hidehint.gameObject.SetActive(true);
        });
        Hidehint.onClick.AddListener(() =>
        {
            HintText.gameObject.SetActive(false);
            Showhint.gameObject.SetActive(true);
            Hidehint.gameObject.SetActive(false);
        });
        Tryagain.onClick.AddListener(() =>
        {
            TryAgain();
        });
        Newword.onClick.AddListener(() =>
        {
            NewGame();
        });

        NewGame();
    }

    public void NewGame()
    {
        ClearBoard();
        StartCoroutine(GetData());
        enabled = true;
    }

    public void TryAgain()
    {
        ClearBoard();
        enabled = true;
    }


    void Update()
    {
        Row currentRow = rows[RowIndex];

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            ColumnIndex = Mathf.Max(ColumnIndex - 1, 0);
            currentRow.tiles[ColumnIndex].SetLetter('\0');
            currentRow.tiles[ColumnIndex].SetStates(emptyState);


        }


        else if (ColumnIndex >= rows[RowIndex].tiles.Length)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                SubmitRow(currentRow);
            }
        }

        else
        {
            for (int i = 0; i < SUPPORTED_KEY.Length; i++)
            {
                if (Input.GetKeyDown(SUPPORTED_KEY[i]))
                {
                    currentRow.tiles[ColumnIndex].SetLetter((char)SUPPORTED_KEY[i]);
                    currentRow.tiles[ColumnIndex].SetStates(OccupiedState);
                    ColumnIndex++;
                    break;
                }
            }
        }


        
    }

    private void SubmitRow(Row row)
    {
        string remaining = word;

        for (int i=0;i<row.tiles.Length;i++)
        {
            Tile tile = row.tiles[i];
            if (tile.letter == word[i])
            {
                tile.SetStates(CorrectState);
                remaining = remaining.Remove(i, 1);
                remaining = remaining.Insert(i," ");
            }
            else if (!word.Contains(tile.letter))
            {
                tile.SetStates(incorrectState);
            }
        }

        for (int i=0;i<row.tiles.Length;i++)
        {
            Tile tile = row.tiles[i];

            if (tile.state!=CorrectState&&tile.state !=CorrectState)
            {
                if (remaining.Contains(tile.letter))
                {
                    int index = remaining.IndexOf(tile.letter);
                    tile.SetStates(wrongspotState);
                    remaining = remaining.Remove(index, 1);
                    remaining = remaining.Insert(index, " ");
                }
                else
                {
                    tile.SetStates(incorrectState);
                }
            }
        }

        /*for (int i=0;i<row.tiles.Length;i++)
        {
            Tile tile = row.tiles[i];
            if (tile.letter == word[i])
            {
                tile.SetStates(CorrectState);
            }
            else if (word.Contains(tile.letter))
            {
                tile.SetStates(wrongspotState);
            }
            else
            {
                tile.SetStates(incorrectState);
            }
        }*/

        if (HasWon(row))
        {
            enabled = false;
        }
        RowIndex++;
        ColumnIndex=0;

        if (RowIndex>=rows.Length)
        {
            enabled = false;
        }
    }

    private bool HasWon(Row row)
    {
        for (int i=0; i<row.tiles.Length;i++)
        {
            if (row.tiles[i].state != CorrectState)
            {
                return false;
            }
        }
        return true;
    }

    private void ClearBoard()
    {
        for (int row=0;row<rows.Length;row++)
        {
            for (int col = 0; col < rows[row].tiles.Length;col++)
            {
                rows[row].tiles[col].SetLetter('\0');
                rows[row].tiles[col].SetStates(emptyState);
            }
        }
        RowIndex = 0;
        ColumnIndex = 0;
    }



    private void OnEnable()
    {
        Tryagain.gameObject.SetActive(false);
        Newword.gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        Tryagain.gameObject.SetActive(true);
        Newword.gameObject.SetActive(true);
    }

    IEnumerator GetData()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://asia-south1-dj-ui-dev.cloudfunctions.net/wordlewordV2");
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("String sent successfully");
            string results = request.downloadHandler.text;
            GetWords SavedData = JsonUtility.FromJson<GetWords>(results);
            Debug.Log(SavedData.word);
            word = SavedData.word;
            HintText.text = word;
        }
        else
        {
            Debug.LogError("Error sending string: " + request.error);
        }
    }

}
[Serializable]
public class GetWords
{
    public string word;
}