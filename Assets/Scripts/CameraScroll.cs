using UnityEngine;

/// <summary>
/// Класс отвечает за скроллинг сцены 
/// </summary>
public class CameraScroll : MonoBehaviour
{
    [SerializeField] private InputController _input;
    [SerializeField] private SpriteRenderer _background;

    private float _leftBorder;
    private float _rightBorder;
    private bool _isScrolling;
    
    public bool IsScrolling { set { _isScrolling = value; }}

    private void Start()
    {
        CalculateCameraBounds();
    }

    private void Update()
    {
        if (_isScrolling)
        {
            var x = transform.position.x - _input.Delta.x;
            transform.position = new Vector3(Mathf.Clamp(x, _leftBorder, _rightBorder), transform.position.y, transform.position.z);
        }
    }

    /// <summary>
    /// Вычисление границ по оси X, за которые не должна выезжать камера.
    /// Вычисляется по границам целевого SpriteRenderer
    /// </summary>
    private void CalculateCameraBounds()
    {
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;

        _leftBorder = _background.bounds.min.x + camHalfWidth;
        _rightBorder = _background.bounds.max.x - camHalfWidth;
    }
}
