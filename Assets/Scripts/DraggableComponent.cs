using UnityEngine;

/// <summary>
/// Класс контролирует поведение Draggable объектов
/// </summary>
public class DraggableComponent : MonoBehaviour
{
    [SerializeField] private float _sizeCoef = 1.2f; //коэффициент увеличения захваченного объекта
    [SerializeField] private MagnetComponent _magnete;  //компонент объекта, реагирующий на прохождение через поверхности рядом
    [SerializeField] private PutComponent _put; //компонент объекта, реагирующий на физическое столкновение с поверхностью
    [SerializeField] private Collider2D _touchCollider; //коллайдер для захвата объекта

    private Vector2 _initialSize; //начальный размер объекта
    private bool _isDragging = false;
    private float _startGravity;
    private Vector3 _velocity = Vector3.zero;

    private Rigidbody2D _rigidbody;
    private InputController _input; //нужен для получения текущих координат

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        _initialSize = transform.localScale;
        _startGravity = _rigidbody.gravityScale;

        _magnete.OnMagnet += Magnete; //получать уведомления о возможности примагнитится к поверхности рядом
        _put.OnPut += Put; //получать уведомления, когда нужно поставить объект
    }

    private void Update()
    {
        if (_isDragging)
        {
            //два варианта следования за курсором - более плавный и следующий четко за текущей позицией
            //transform.position = _input.CurrentWorldPosition;
            transform.position = Vector3.SmoothDamp(transform.position, _input.CurrentWorldPosition, ref _velocity, 0.2f);
        }
    }

    /// <summary>
    /// Запустить следование за курсором
    /// </summary>
    public void StartDrag(InputController input)
    {
        _input ??= input; //если объект подняли впервые, запоминаем InputController

        transform.localScale = _initialSize * _sizeCoef; //увеличиваем размер объекта
        _rigidbody.isKinematic = true; //выводим объект из-под контроля физики
        _magnete.IsActive = false;  //пока объект перемещается - отключаем магниты

        _isDragging = true;
    }

    /// <summary>
    /// Отпускаем объект
    /// </summary>
    public void Drop()
    {
        _isDragging = false;
        transform.localScale = _initialSize;   //возвращаем исходный размер

        _rigidbody.gravityScale = _startGravity; //возвращаем объекту исходное значение гравитации
        _rigidbody.isKinematic = false; //возвращаем объект под контроль физики
        _touchCollider.enabled = false; //пока объект падает, мы не можем его поднять
        _magnete.IsActive = true;   //пока объект падает, он может примагнититься
    }

    /// <summary>
    /// Ставим объект на поверхность
    /// </summary>
    private void Put()
    {
        _rigidbody.velocity = Vector2.zero; //убираем скорость
        _rigidbody.gravityScale = 0; //отключаем гравитацию
        _magnete.IsActive = false;   //когда объект стоит, он не может примагнититься
        _touchCollider.enabled = true;  //когда объект стоит, мы можем его поднять
    }
    
    /// <summary>
    /// Примагнитить объект к поверхности
    /// </summary>
    private void Magnete(Vector2 center)
    {
        //сработает, только если объект отпустили близко к целовой поверхности
        //нужно для сглаживания случайных промахов игроком мимо поверхности
        if (Mathf.Abs(_rigidbody.velocity.y) < 15f)
        {
            //вычисляем целевые координаты, исходя из текущего положения объекта и центра целевой поверхности
            //объект не должен лететь всегда в центр, но и совсем на край его ставить не стоит.
            //формулы требуют доработки, либо пересмотра расставновки коллайдеров поверхностей
            var targetPositionX = center.x + 0.6f * (transform.position.x - center.x);
            var targetPositionY = center.y + 0.3f * (transform.position.y - center.y);
            transform.position = new Vector2(targetPositionX, targetPositionY);

            Put(); //когда объект примагнитился - ставим его
        }
    }

    private void OnDestroy()
    {
        //отписываемся от всех событий
        _magnete.OnMagnet -= Magnete;
        _put.OnPut -= Put; 
    }
}
