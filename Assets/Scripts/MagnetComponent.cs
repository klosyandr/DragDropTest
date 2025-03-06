using System;
using UnityEngine;

/// <summary>
/// Класс, реагирующий на прохождение коллайдером-триггером через поверхности рядом
/// </summary>
public class MagnetComponent : MonoBehaviour
{
    public bool IsActive = false;

    public event Action<Vector2> OnMagnet; //посылает подписчикам оповещение о встреченной поверхности с координатами её центра

    private void OnTriggerEnter2D(Collider2D other)
    {
       OnTrigger(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
       OnTrigger(other);
    }   

    private void OnTrigger(Collider2D other)
    {
        if(!IsActive) return;
        if(!other.gameObject.CompareTag("put")) return;
        if (!CheckSideTouch(other)) return;

        OnMagnet?.Invoke(other.bounds.center);
    }
    
    /// <summary>
    /// возвращает true, если объект падает рядом, а не сверху.
    /// Либо если объект немного ниже поверхности
    /// </summary>
    private bool CheckSideTouch(Collider2D other)
    {
        return transform.position.x < other.bounds.min.x 
            || transform.position.x > other.bounds.max.x
            || transform.position.y < other.bounds.max.y;
    }
}
