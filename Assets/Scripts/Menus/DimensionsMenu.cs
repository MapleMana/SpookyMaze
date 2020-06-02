using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionsMenu : Menu<DimensionsMenu>
{
    public void OnDimensionPressed()
    {
        LevelSelectMenu.Open();
    }
}
