using UnityEngine;

public class CapsuleGrowth : MonoBehaviour
{
    [SerializeField] private float growthSpeed = 0.5f;
    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float growth = growthSpeed * Time.deltaTime;
        transform.localScale += new Vector3(growth, growth, growth);
    }

    public void ResetSize()
    {
        transform.localScale = initialScale;
    }
}