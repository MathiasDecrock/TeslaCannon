using System;
using UnityEngine;

public class ParticleAttractor : MonoBehaviour 
{
    [SerializeField] private ParticleSystem particles;
    [SerializeField] internal Transform particlesAttractor;
    [SerializeField] internal GameObject ParticleSystem;

    private void OnEnable()
    {
        const string name = "ParticleSystem";
        ParticleSystem = transform.Find(name).gameObject;
    }

    private void LateUpdate()
    {
        if(particlesAttractor!= null)
        {
            ParticleSystem.SetActive(true);
            ShootParticles();
        }

        if (particlesAttractor == null)
        {
            ParticleSystem.SetActive(false);
        }
    }

    private void AttractParticles()
    {
        ParticleSystem.Particle[] ps = new ParticleSystem.Particle[particles.particleCount];
        particles.GetParticles (ps);
        for (int i =0; i<ps.Length; i++)
            ps [i].velocity = Vector3.Lerp (ps [i].velocity, (particlesAttractor.position - ps [i].position).normalized, 0.1f);
        particles.SetParticles (ps, ps.Length);
         
    }
     
    private void ShootParticles()
    {
        var pos = (particlesAttractor.position - transform.position).normalized;
        var particlesVelocityOverLifetime = particles.velocityOverLifetime;
        particlesVelocityOverLifetime.x = pos.x;
        particlesVelocityOverLifetime.y = pos.y;
        particlesVelocityOverLifetime.z = pos.z;

    }
}