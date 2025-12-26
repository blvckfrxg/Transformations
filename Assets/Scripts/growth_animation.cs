using UnityEngine;

public class Scaler : MonoBehaviour
{
    [SerializeField] private float _growthSpeed = 0.5f;

    private Vector3 _initialScale;

    private void Start()
    {
        _initialScale = transform.localScale;
    }

    private void Update()
    {
        float growth = _growthSpeed * Time.deltaTime;
        transform.localScale += new Vector3(growth, growth, growth);
    }

    public void ResetSize()
    {
        transform.localScale = _initialScale;
    }
}