using UnityEngine;

public class ExplosiveProjectileController : ProjectileController
{
    public ExplosionBehaviour explosionBehaviour;
    private void OnEnable()
    {
        onEndOfLifetime += OnEndOfLifetime;
    }

    private void OnDisable()
    {
        onEndOfLifetime -= OnEndOfLifetime;
    }
    private void OnEndOfLifetime()
    {
        explosionBehaviour.transform.SetParent(null);
        explosionBehaviour.transform.localScale = Vector3.one;
        explosionBehaviour.gameObject.SetActive(true);
        Destroy(explosionBehaviour.gameObject, 0.2f);
        ShakeCamera.Instance.ShakeForDuration(0.5f, 0.5f, transform.position);
    }
}
