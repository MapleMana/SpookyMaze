using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameMenu : Menu<EndGameMenu>
{
    public void OnMainMenuButtonPressed()
    {
        MainMenu.Open();
    }
}
