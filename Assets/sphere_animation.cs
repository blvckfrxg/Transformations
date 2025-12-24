using UnityEngine;

public class SphereMovement : MonoBehaviour
{
    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float moveDistance = 5f;
    [SerializeField] private Vector3 moveDirection = Vector3.forward;

    [Header("Визуализация")]
    [SerializeField] private bool showGizmos = true;
    [SerializeField] private Color gizmoColor = Color.green;

    private Vector3 startPosition;
    private bool movingForward = true;
    private float currentProgress = 0f;

    void Start()
    {
        startPosition = transform.position;
        moveDirection = moveDirection.normalized;
    }

    void Update()
    {
        // Рассчитываем движение вперед-назад
        if (movingForward)
        {
            currentProgress += moveSpeed * Time.deltaTime / moveDistance;
            if (currentProgress >= 1f)
            {
                currentProgress = 1f;
                movingForward = false;
            }
        }
        else
        {
            currentProgress -= moveSpeed * Time.deltaTime / moveDistance;
            if (currentProgress <= 0f)
            {
                currentProgress = 0f;
                movingForward = true;
            }
        }

        // Плавное движение с использованием синуса для более естественного ускорения/замедления
        float smoothProgress = Mathf.SmoothStep(0f, 1f, currentProgress);

        // Обновляем позицию
        transform.position = startPosition + moveDirection * (smoothProgress * moveDistance);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        Gizmos.color = gizmoColor;
        Vector3 start = Application.isPlaying ? startPosition : transform.position;
        Vector3 direction = Application.isPlaying ? moveDirection : transform.forward;

        // Линия пути
        Gizmos.DrawLine(start, start + direction * moveDistance);

        // Стрелки направления
        DrawArrow(start, start + direction * moveDistance, gizmoColor);

        // Точки начала и конца
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(start, 0.2f);
        Gizmos.DrawWireSphere(start + direction * moveDistance, 0.2f);
    }

    void DrawArrow(Vector3 from, Vector3 to, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawLine(from, to);

        Vector3 direction = (to - from).normalized;
        float arrowSize = 0.5f;

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;

        Gizmos.DrawLine(to, to + right * arrowSize);
        Gizmos.DrawLine(to, to + left * arrowSize);
    }

    // Метод для сброса в начальное положение
    public void ResetPosition()
    {
        transform.position = startPosition;
        currentProgress = 0f;
        movingForward = true;
    }
}