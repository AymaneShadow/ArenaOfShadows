using UnityEngine;

namespace Controls
{
    public class AbilityProjectile1 : AbilityProjectile
    {
        void FixedUpdate()
        {
            if (_alive)
            {
                _animationProgress += Time.deltaTime;
                transform.position =
                    MathParabola.Parabola(transform.position, _targetPosition, 0.5f, _animationProgress);
                if (Vector3.Distance(transform.position, _targetPosition) < 0.5f)
                {
                    TerminateParticle();
                }
            }
        }

        public void TerminateParticle()
        {
            abilityPrefab.SetActive(true);
            Instantiate(abilityPrefab, _targetPosition, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}