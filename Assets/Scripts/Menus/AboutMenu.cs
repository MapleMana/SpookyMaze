﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutMenu : Menu<AboutMenu>
{
    public void OnMainMenuButtonPressed()
    {
        MainMenu.Open();
    }
}
