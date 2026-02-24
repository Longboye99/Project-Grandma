using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _mainPauseMenu;
    [SerializeField] GameObject _SettingMenu;

    MenuState _menuState;
    public enum MenuState
    {
        Default,
        Setting
    }

    private void OnEnable()
    {
        GameEventsManager.instance.inputEvents.onPause += ExitMenu;
    }

    private void OnDisable()
    {
        GameEventsManager.instance.inputEvents.onPause -= ExitMenu;
    }

    private void Start()
    {
        _menuState = MenuState.Default;
    }

    public void OpenSettingMenu()
    {
        _mainPauseMenu.SetActive(false);
        _SettingMenu.SetActive(true);
        _menuState = MenuState.Setting;
    }

    public void CloseSettingMenu()
    {
        _mainPauseMenu.SetActive(true);
        _SettingMenu.SetActive(false);
        _menuState = MenuState.Default;
    }

    private void ExitMenu()
    {
        if(_menuState == MenuState.Setting)
        {
            CloseSettingMenu();
        }
        else if (_menuState == MenuState.Default)
        {
            GameManager.instance.uiManager.UnPause();
        }
    }
}
