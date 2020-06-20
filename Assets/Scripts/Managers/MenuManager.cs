using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    public AboutMenu AboutMenuPrefab;
    public DimensionsMenu DimensionsMenuPrefab;
    public EndGameMenu EndGameMenuPrefab;
    public LevelSelectMenu LevelSelectMenuPrefab;
    public MainMenu MainMenuPrefab;
    public OnReplayMenu OnReplayMenuPrefab;
    public ScoreMenu ScoreMenuPrefab;
    public SettingsMenu SettingsMenuPrefab;

    private Stack<Menu> menuStack = new Stack<Menu>();

    public override void Awake()
    {
        base.Awake();

        MainMenu.Open();
    }


    public void CreateInstance<T>() where T : Menu
    {
        var prefab = GetPrefab<T>();

        Instantiate(prefab, transform);
    }

    public void OpenMenu(Menu instance)
    {
        if (menuStack.Count > 0)
        {
            if (instance.DisableMenusUnderneath)
            {
                foreach (Menu menu in menuStack)
                {
                    menu.gameObject.SetActive(false);

                    if (menu.DisableMenusUnderneath)
                        break;
                }
            }

            var topCanvas = instance.GetComponent<Canvas>();
            var previousCanvas = menuStack.Peek().GetComponent<Canvas>();
            topCanvas.sortingOrder = previousCanvas.sortingOrder + 1;
        }

        menuStack.Push(instance);
    }

    private T GetPrefab<T>() where T : Menu
    {
        var fields = this.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (var field in fields)
        {
            var prefab = field.GetValue(this) as T;
            if (prefab != null)
            {
                return prefab;
            }
        }

        throw new MissingReferenceException("Prefab not found for type " + typeof(T));
    }

    public void CloseMenu(Menu menu)
    {
        if (menuStack.Count == 0)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because menu stack is empty", menu.GetType());
            return;
        }

        if (menuStack.Peek() != menu)
        {
            Debug.LogErrorFormat(menu, "{0} cannot be closed because it is not on top of stack", menu.GetType());
            return;
        }

        CloseTopMenu();
    }

    public void CloseTopMenu()
    {
        var instance = menuStack.Pop();

        if (instance.DestroyWhenClosed)
            Destroy(instance.gameObject);
        else
            instance.gameObject.SetActive(false);

        foreach (Menu menu in menuStack)
        {
            menu.gameObject.SetActive(true);

            if (menu.DisableMenusUnderneath)
                break;
        }
    }

    public void ClearMenuStack()
    {
        foreach (Menu menu in menuStack)
        {
            Destroy(menu.gameObject);
        }
        menuStack.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && menuStack.Count > 0)
        {
            menuStack.Peek().OnBackPressed();
        }
    }
}