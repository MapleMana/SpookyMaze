using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameMenu : Menu<EndGameMenu>
{
    public TextMeshProUGUI NextPlay;

    public void OnMainMenuButtonPressed()
    {
        MainMenu.Open();
    }

    public void SetNextActionText(bool mazeCompleted)
    {
        NextPlay.text = mazeCompleted ? "Go to the Next Level" : "Play Again";
    }
}
