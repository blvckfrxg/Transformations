using UnityEngine;

public class CubeRotation : MonoBehaviour
{
    [Header("Настройки вращения")]
    [SerializeField] private float rotationSpeed = 90f; // Градусов в секунду
    [SerializeField] private Vector3 rotationAxis = Vector3.up;
    [SerializeField] private Space rotationSpace = Space.World;

    [Header("Настройки цикличности")]
    [SerializeField] private bool smoothRotation = true;
    [SerializeField] private AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Дополнительные опции")]
    [SerializeField] private bool oscillateDirection = false;
    [SerializeField] private float oscillationFrequency = 0.5f;

    private float currentRotation = 0f;
    private float directionMultiplier = 1f;
    private float oscillationTimer = 0f;

    void Update()
    {
        // Обработка колебания направления (если включено)
        if (oscillateDirection)
        {
            oscillationTimer += Time.deltaTime * oscillationFrequency;
            directionMultiplier = Mathf.Sin(oscillationTimer * Mathf.PI * 2) * 0.5f + 0.5f;
        }

        // Рассчитываем вращение
        float rotationThisFrame = rotationSpeed * Time.deltaTime * directionMultiplier;
        currentRotation += rotationThisFrame;

        // Применяем вращение
        if (smoothRotation)
        {
            float curveValue = rotationCurve.Evaluate(Time.time % 1f);
            rotationThisFrame *= curveValue;
        }

        transform.Rotate(rotationAxis.normalized, rotationThisFrame, rotationSpace);

        // Сброс значения при переполнении
        if (currentRotation >= 360f) currentRotation -= 360f;
        if (currentRotation <= -360f) currentRotation += 360f;
    }

    // Методы для изменения параметров через другие скрипты
    public void SetRotationSpeed(float speed) => rotationSpeed = speed;
    public void SetRotationAxis(Vector3 axis) => rotationAxis = axis.normalized;

    void OnValidate()
    {
        // Автоматическая нормализация оси вращения
        rotationAxis = rotationAxis.normalized;
    }

    void OnDrawGizmosSelected()
    {
        // Визуализация оси вращения
        Gizmos.color = Color.red;
        Vector3 center = transform.position;
        Vector3 axis = rotationSpace == Space.World ? rotationAxis : transform.TransformDirection(rotationAxis);
        Gizmos.DrawLine(center - axis * 2, center + axis * 2);
        Gizmos.DrawWireSphere(center + axis * 2, 0.2f);
    }
}