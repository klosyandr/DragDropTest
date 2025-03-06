using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Класс реагирует на заданные события от InputSystem, 
/// необходимые для реализации DrapAndDrop механики
/// </summary>
public class InputController : MonoBehaviour
{
    [SerializeField] private CameraScroll _camera;
    [Header("Input Settings")]
    [SerializeField] private InputAction _press;
    [SerializeField] private InputAction _position;
    [SerializeField] private InputAction _delta;
    [SerializeField] private float _deltaCoef = 0.1f; //коэффициент чувствительности для скролла

    private Vector2 _currentPosition;
    private Vector2 _currentDelta;
    private DraggableComponent _targetObject;

    public Vector2 CurrentWorldPosition => Camera.main.ScreenToWorldPoint(_currentPosition);
    public Vector2 Delta => _currentDelta * _deltaCoef;

    private void Awake()
    {
        _press.Enable();
        _position.Enable();
        _delta.Enable();

        _position.performed += context => { _currentPosition = context.ReadValue<Vector2>(); };
        _delta.performed += context => { _currentDelta = context.ReadValue<Vector2>(); };

        _press.performed += OnPress;
        _press.canceled += context => { OnPressOut(); };
    }

    /// <summary>
    /// Обработка нажатия клавиши или тапа
    /// </summary>
    private void OnPress(InputAction.CallbackContext context)
    {
        OnPressOut();
        var ray = Camera.main.ScreenToWorldPoint(_currentPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray, Vector2.zero);

        if (TryFindDragCollider(hits, "draggable", out var hit))
        {
            _targetObject = hit.collider.gameObject.GetComponentInParent<DraggableComponent>();
            _targetObject.StartDrag(this);
        }
        else
        {
            _camera.IsScrolling = true;
        }
    }

    /// <summary>
    /// Обработка отнажатия клавиши или тапа
    /// </summary>
    private void OnPressOut()
    {
        if (_targetObject != null)
        {
            _targetObject.Drop();
            _targetObject = null;
        }
        _camera.IsScrolling = false;
    }

    /// <summary>
    /// Поиск первого коллайдера с целевым тегом в массиве коллайдеров, на которые попал луч
    /// </summary>
    /// <param name="tag">Целевой тег</param>
    /// <param name="hit">Найденный RaycastHit2D, если функция вернула true. Иначе - дефолтный</param>
    private bool TryFindDragCollider(RaycastHit2D[] hits, string tag, out RaycastHit2D hit)
    {
        hit = default;
        if (hits.Length == 0) return false;
        
        foreach (var temp in hits)
        {
            if (temp.collider.gameObject.CompareTag(tag))
            {
                hit = temp;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Пересчитывает значение коэффициент чувствительности для скролла
    /// </summary>
    public void ChangeDeltaCoef(float value)
    {
        _deltaCoef = value * (0.02f - 0.005f);
    }

    private void OnDestroy()
    {
        //отписываемся от всех событий
        _position.performed -= context => { _currentPosition = context.ReadValue<Vector2>(); };
        _delta.performed -= context => { _currentDelta = context.ReadValue<Vector2>(); };

        _press.performed -= OnPress;
        _press.canceled -= context => { OnPressOut(); };
    }
}
