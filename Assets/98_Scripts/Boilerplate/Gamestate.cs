public class Gamestate : Singleton<Gamestate>
{
    private Gamestats currentState;

    public static Gamestats CurrentState { get => Instance.currentState; }

    void Start()
    {

    }

    public static void TryToChangeState(Gamestats newState)
    {
        bool isAllowed = false;

        if (newState != CurrentState)
        {
            isAllowed = true;
        }

        switch (newState)
        {
            case Gamestats.Init:
                {
                    if (isAllowed)
                    {

                    }
                    break;
                }
            case Gamestats.Game:
                {
                    if (isAllowed)
                    {

                    }
                    break;
                }
            case Gamestats.Menu:
                {
                    if(isAllowed)
                    {

                    }
                    break;
                }
            case Gamestats.Quit:
                {
                    if(isAllowed)
                    {

                    }
                    break;
                }
        }

        if(isAllowed)
        {
            Instance.currentState = newState;
        }
    }
}



public enum Gamestats
{
    Init,
    Menu,
    Game,
    Quit
}