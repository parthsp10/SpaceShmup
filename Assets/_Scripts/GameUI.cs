using UnityEngine;

public class GameUI : MonoBehaviour
{
    // called by the Start button
    public void OnStartButton()
    {
        if (Main.S != null)
        {
            Main.S.StartGameFromUI();
        }
    }

    // called by the Play Again button
    public void OnPlayAgainButton()
    {
        if (Main.S != null)
        {
            Main.S.RestartGame();
        }
    }
}
