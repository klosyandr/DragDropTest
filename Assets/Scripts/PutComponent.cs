using System;
using UnityEngine;

/// <summary>
/// Класс, реагирующий на столкновение коллайдера с поверхностью
/// </summary>
public class PutComponent : MonoBehaviour
{
    public event Action OnPut; //посылает подписчикам оповещение о столкновении с поверхностью
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("put") 
            || collision.gameObject.CompareTag("floor"))
        {
            OnPut?.Invoke();
        }
        
    }
}
