using System;
using System.Collections;
using UnityEngine;

public class TeslaCommander : MonoBehaviour
{
    [SerializeField] internal ParticleAttractor _particleAttractor;
    [SerializeField] internal PrivateArea _area;
    private Humanoid hum;
    private HitData hitdata;
    private GameObject LightningStrikeVFX;
    private Transform strikelocation;
    private Collider _collider;

    private void Awake()
    {
       LightningStrikeVFX = ZNetScene.instance.GetPrefab("lightningAOE");
       //_particleAttractor.particlesAttractor = hitcollider.transform;
    }
    

    private void Castlightning()
    {
        if (!_area.IsEnabled()) return;
            Collider[] hitcolliders = Physics.OverlapSphere(transform.position, _area.m_radius);
            foreach (var hitcollider in hitcolliders)
            {
                if (hitcollider.gameObject.GetComponent<MonsterAI>() != null)
                {
                    if (hitcollider.gameObject.GetComponent<Humanoid>().m_tamed == true) return;
                    try
                    { 
                        var tame = hitcollider.GetComponent<Tameable>();
                        if(tame != null)
                            if (tame.GetTameness() > 0) return;
                    }
                    catch (Exception e)
                    {
                        Jotunn.Logger.LogInfo(e);
                    }
                   
                    var tmp = hitcollider.gameObject;
                    _collider = hitcollider;
                    hum = tmp.GetComponent<Humanoid>();
                    try
                    {
                        hitdata = new HitData
                        {
                            m_attacker = Player.m_localPlayer.GetZDOID(),
                            m_blockable = false,
                            m_damage = new HitData.DamageTypes
                            {
                                m_blunt = 0f,
                                m_chop = 0f,
                                m_lightning = 10f,
                                m_damage = 0f,
                                m_fire = 0f,
                                m_frost = 0f,
                                m_pickaxe = 0f,
                                m_pierce = 0f,
                                m_poison = 0f,
                                m_slash = 0f,
                                m_spirit = 0f
                            },
                            m_dir = Vector3.zero,
                            m_dodgeable = false,
                            m_ranged = true,
                            m_skill = Skills.SkillType.All,
                            m_backstabBonus = 0f,
                            m_hitCollider = _collider,
                            m_pushForce = 3.5f,
                            m_staggerMultiplier = 0.01f,
                            m_toolTier = 10,
                            m_statusEffect = "",
                            m_point = Vector3.zero
                        };
                    }
                    catch (Exception e)
                    {
                        Jotunn.Logger.LogError(e);
                    }
                   
                    strikelocation = hum.transform;
                    _particleAttractor.particlesAttractor = hum.transform;
                    StartCoroutine(LightningStrike());
                }
            }
    }
    
    internal IEnumerator LightningStrike()
    {
            Instantiate(LightningStrikeVFX, strikelocation, false);
            hum.ApplyDamage(hitdata, true, triggerEffects: false, HitData.DamageModifier.Weak);
            yield return new WaitForSeconds(2.5f);
    }
    
    private void OnEnable()
    {
        if (_area == null)
        {
            _area = GetComponentInParent<PrivateArea>();
        }
        _area.m_name = "Tesla Cannon";
        InvokeRepeating(nameof(Castlightning), 0f, 1);
    }
    
}
