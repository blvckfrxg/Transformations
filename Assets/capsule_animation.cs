using UnityEngine;

public class CapsuleScale : MonoBehaviour
{
    [Header("Настройки изменения размера")]
    [SerializeField] private float scaleSpeed = 1f;
    [SerializeField] private Vector3 minScale = Vector3.one;
    [SerializeField] private Vector3 maxScale = Vector3.one * 2f;

    [Header("Настройки цикличности")]
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private bool pingPong = true;
    [SerializeField] private bool uniformScaling = true;

    [Header("Дополнительные опции")]
    [SerializeField] private bool affectChildren = false;
    [SerializeField] private bool useLocalScale = true;

    private Vector3 originalScale;
    private float scaleProgress = 0f;
    private bool scalingUp = true;

    void Start()
    {
        originalScale = useLocalScale ? transform.localScale : GetWorldScale(transform);

        // Корректируем минимальный и максимальный размер относительно исходного
        minScale = Vector3.Scale(originalScale, minScale);
        maxScale = Vector3.Scale(originalScale, maxScale);
    }

    void Update()
    {
        // Обновляем прогресс
        if (pingPong)
        {
            if (scalingUp)
            {
                scaleProgress += scaleSpeed * Time.deltaTime;
                if (scaleProgress >= 1f)
                {
                    scaleProgress = 1f;
                    scalingUp = false;
                }
            }
            else
            {
                scaleProgress -= scaleSpeed * Time.deltaTime;
                if (scaleProgress <= 0f)
                {
                    scaleProgress = 0f;
                    scalingUp = true;
                }
            }
        }
        else
        {
            scaleProgress += scaleSpeed * Time.deltaTime;
            scaleProgress = Mathf.Repeat(scaleProgress, 1f);
        }

        // Применяем кривую
        float curvedProgress = scaleCurve.Evaluate(scaleProgress);

        // Вычисляем новый размер
        Vector3 newScale;
        if (uniformScaling)
        {
            float scaleFactor = Mathf.Lerp(minScale.x, maxScale.x, curvedProgress);
            newScale = originalScale * scaleFactor;
        }
        else
        {
            newScale = Vector3.Lerp(minScale, maxScale, curvedProgress);
        }

        // Применяем изменение размера
        if (useLocalScale)
        {
            transform.localScale = newScale;
        }
        else
        {
            // Для установки мирового масштаба
            SetWorldScale(newScale);
        }

        // Применяем к дочерним объектам (если нужно)
        if (affectChildren)
        {
            foreach (Transform child in transform)
            {
                child.localScale = Vector3.one;
            }
        }
    }

    // Метод для получения мирового масштаба
    private Vector3 GetWorldScale(Transform t)
    {
        Vector3 worldScale = t.localScale;
        Transform parent = t.parent;

        while (parent != null)
        {
            worldScale = Vector3.Scale(worldScale, parent.localScale);
            parent = parent.parent;
        }

        return worldScale;
    }

    // Метод для установки мирового масштаба
    private void SetWorldScale(Vector3 worldScale)
    {
        if (transform.parent != null)
        {
            Vector3 parentWorldScale = GetWorldScale(transform.parent);

            // Защита от деления на 0
            parentWorldScale.x = Mathf.Abs(parentWorldScale.x) > 0.001f ? parentWorldScale.x : 1f;
            parentWorldScale.y = Mathf.Abs(parentWorldScale.y) > 0.001f ? parentWorldScale.y : 1f;
            parentWorldScale.z = Mathf.Abs(parentWorldScale.z) > 0.001f ? parentWorldScale.z : 1f;

            transform.localScale = new Vector3(
                worldScale.x / parentWorldScale.x,
                worldScale.y / parentWorldScale.y,
                worldScale.z / parentWorldScale.z
            );
        }
        else
        {
            transform.localScale = worldScale;
        }
    }

    // Методы для изменения параметров через другие скрипты
    public void SetScaleSpeed(float speed) => scaleSpeed = Mathf.Max(0, speed);
    public void SetMinScale(Vector3 scale) => minScale = scale;
    public void SetMaxScale(Vector3 scale) => maxScale = scale;

    void OnDrawGizmosSelected()
    {
        // Визуализация минимального и максимального размеров
        Gizmos.color = Color.blue;
        Gizmos.DrawWireMesh(GetComponent<MeshFilter>()?.sharedMesh,
                           transform.position,
                           transform.rotation,
                           minScale);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireMesh(GetComponent<MeshFilter>()?.sharedMesh,
                           transform.position,
                           transform.rotation,
                           maxScale);
    }
}