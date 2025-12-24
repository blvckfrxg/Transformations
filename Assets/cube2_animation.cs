using UnityEngine;

public class SimpleOrbitingCube : MonoBehaviour
{
    [Header("Движение по кругу")]
    [SerializeField] private float orbitRadius = 3f;
    [SerializeField] private float orbitSpeed = 30f; // градусов в секунду

    [Header("Вращение вокруг себя")]
    [SerializeField] private float selfRotationSpeed = 60f; // градусов в секунду
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    [Header("Изменение размера")]
    [SerializeField] private float scaleSpeed = 0.5f;
    [SerializeField] private float minScale = 0.7f;
    [SerializeField] private float maxScale = 1.5f;

    private Vector3 orbitCenter;
    private Vector3 originalScale;
    private float currentAngle = 0f;
    private float scaleTimer = 0f;

    void Start()
    {
        // Центр орбиты - текущая позиция
        orbitCenter = transform.position;
        originalScale = transform.localScale;
    }

    void Update()
    {
        // 1. Обновляем угол для движения по кругу
        currentAngle += orbitSpeed * Time.deltaTime;
        if (currentAngle >= 360f) currentAngle -= 360f;

        // 2. Вычисляем новую позицию по кругу
        float angleInRadians = currentAngle * Mathf.Deg2Rad;
        float x = Mathf.Sin(angleInRadians) * orbitRadius;
        float z = Mathf.Cos(angleInRadians) * orbitRadius;

        transform.position = orbitCenter + new Vector3(x, 0, z);

        // 3. Поворачиваем куб, чтобы он смотрел по направлению движения
        // Вычисляем направление движения (касательная к окружности)
        Vector3 tangent = new Vector3(
            Mathf.Cos(angleInRadians),
            0,
            -Mathf.Sin(angleInRadians)
        ).normalized;

        // Поворачиваем куб в направлении движения
        if (tangent.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(tangent, Vector3.up);
        }

        // 4. Вращение вокруг своей оси (дополнительно к повороту по направлению)
        transform.Rotate(rotationAxis * selfRotationSpeed * Time.deltaTime, Space.Self);

        // 5. Изменение размера (пульсация)
        scaleTimer += scaleSpeed * Time.deltaTime;
        float scaleFactor = (Mathf.Sin(scaleTimer) + 1f) * 0.5f; // от 0 до 1
        float currentScale = Mathf.Lerp(minScale, maxScale, scaleFactor);
        transform.localScale = originalScale * currentScale;
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация орбиты
        Gizmos.color = Color.green;
        Vector3 center = Application.isPlaying ? orbitCenter : transform.position;

        // Рисуем окружность
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(0, 0, orbitRadius);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = center + new Vector3(
                Mathf.Sin(angle) * orbitRadius,
                0,
                Mathf.Cos(angle) * orbitRadius
            );

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // Центр орбиты
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(center, 0.2f);

        // Направление движения (касательная)
        Gizmos.color = Color.blue;
        Vector3 direction = transform.forward;
        Gizmos.DrawRay(transform.position, direction * 1.5f);
    }
}