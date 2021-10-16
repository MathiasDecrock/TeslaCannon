using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TeslaCommander : MonoBehaviour
{
    [SerializeField] internal ParticleAttractor _particleAttractor;
    [SerializeField] internal PrivateArea _area;
    private ZNetView m_nview;
    private GameObject LightningStrikeVFX;
    private Coroutine strikeRoutine;

    private void Awake()
    {
        m_nview = GetComponent<ZNetView>();
        LightningStrikeVFX = ZNetScene.instance.GetPrefab("lightningAOE");
    }

    private HashSet<Humanoid> GetTargets()
    {
        HashSet<Humanoid> targets = new HashSet<Humanoid>();
        Collider[] hitcolliders = Physics.OverlapSphere(transform.position, _area.m_radius);
        foreach (var hitcollider in hitcolliders)
        {
            if (hitcollider.gameObject.GetComponentInParent<MonsterAI>() != null)
            {
                Humanoid target = hitcollider.GetComponentInParent<Humanoid>();
                if (!target || target.m_tamed
                    || target.TryGetComponent(out Tameable tame) && tame.GetTameness() > 0)
                {
                    continue;
                }
                targets.Add(target);
            }
        }
        return targets;
    }

    private void Castlightning()
    {
        if(!m_nview.IsOwner())
        {
            //We're not the owner, check again next time, maybe the owner left
            return;
        }

        if (!_area.IsEnabled())
        {
            if (strikeRoutine != null)
            {
                //Fizzle animation? :D
                StopCoroutine(strikeRoutine);
                strikeRoutine = null;
            }
            return;
        }

        if (strikeRoutine != null)
        {
            //Nothing to do, already warming up
            return;
        }

        HashSet<Humanoid> targets = GetTargets();
        if (!targets.Any())
        {
            return;
        }

        strikeRoutine = StartCoroutine(LightningStrike(targets));
    }

    private HitData LightingStrike(float damage, Collider collider)
    {
        return new HitData
        {
            m_attacker = Player.m_localPlayer.GetZDOID(),
            m_blockable = false,
            m_damage = new HitData.DamageTypes
            {
                m_blunt = 0f,
                m_chop = 0f,
                m_lightning = damage,
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
            m_hitCollider = collider,
            m_pushForce = 3.5f,
            m_staggerMultiplier = 0.05f,
            m_toolTier = 10,
            m_statusEffect = "",
            m_point = Vector3.zero
        };
    }

    internal IEnumerator LightningStrike(HashSet<Humanoid> targets)
    {
        //warmup animation? :D
        yield return new WaitForSeconds(2.5f);

        float damage = 10f / targets.Count;
        foreach (Humanoid target in targets)
        {
            if (!target)
            {
                //target already dead
                continue;
            }
            Instantiate(LightningStrikeVFX, target.transform, false);
            HitData hitData = LightingStrike(damage, GetTopCollider(target));
            target.ApplyDamage(hitData, true, triggerEffects: false, HitData.DamageModifier.Weak);
        }

        strikeRoutine = null;
    }

    private Collider GetTopCollider(Humanoid target)
    {
        return target.GetComponentsInChildren<Collider>()
            .OrderByDescending(collider => collider.bounds.center.y)
            .First();
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