namespace Arena.Game.Core;

public class GameStateManager
{
    public GameState CurrentState { get; private set; } = GameState.Menu;

    public void StartGame() => CurrentState = GameState.Playing;
    public void GameOver() => CurrentState = GameState.GameOver;
    public void ReturnToMenu() => CurrentState = GameState.Menu;

    public bool IsPlaying => CurrentState == GameState.Playing;
    public bool IsMenu => CurrentState == GameState.Menu;
    public bool IsGameOver => CurrentState == GameState.GameOver;
}