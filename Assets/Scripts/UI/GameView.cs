public class GameView : ViewBase
{
    public MenuView menuView;
    public OptionWindow optionWindow;

    public void ClickMainMenu()
    {
        menuView.Open();
        Close();
    }
    
    public void ClickOption()
    {
        optionWindow.Open();
    }
}