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
        _mainPauseMenu.SetActive(false);
        _SettingMenu.SetActive(true);
        _menuState = MenuState.Default;
    }
}
