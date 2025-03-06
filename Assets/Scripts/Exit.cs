using UnityEngine;

/// <summary>
/// Класс просто отвечает за закрытие приложения
/// </summary>
public class Exit : MonoBehaviour
{
    public void OnExit()
    {
        Application.Quit();
    }
}
