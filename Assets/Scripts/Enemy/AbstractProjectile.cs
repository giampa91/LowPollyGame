using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public abstract class AbstractProjectile<SELF> : _AbstractProjectileInternal  where SELF : AbstractProjectile<SELF> {
    protected Vector3 direction;
    public float speed = 1f;
    private bool fixedDir = false;
    private static SELF shoot(GameObject projectilePrefab, Vector3 startPos, object direction, object speed) {
        SELF projectile = (SELF)shoot<SELF>(projectilePrefab, startPos);
        if (direction!=null) {
            projectile.direction = (Vector3)direction;
            projectile.fixedDir = true;
        }
        if (speed!=null) projectile.speed = (float)speed;
        return projectile;
    }
    public static SELF shoot(GameObject projectilePrefab, Vector3 startPos, Vector3 direction, float speed) {
        return shoot(projectilePrefab, startPos, (object)direction, (object)speed);
    }
    public static SELF shoot(GameObject projectilePrefab, Vector3 startPos, Vector3 direction) {
        return shoot(projectilePrefab, startPos, (object)direction, null);
    }
    public static SELF shoot(GameObject projectilePrefab, Vector3 startPos, float speed) {
        return shoot(projectilePrefab, startPos, null, (object)speed);
    }
    public static SELF shoot(GameObject projectilePrefab, Vector3 startPos) {
        return shoot(projectilePrefab, startPos, null, null);
    }

    public bool isDirectionFixed() {
        return fixedDir;
    }
}
public abstract class _AbstractProjectileInternal : MonoBehaviour
{
    protected Transform PlayerTransform;
    protected GameManager GameManager;

    public float EffectDurationSeconds = 3f;
    public float WorldColliderRadius = 0.1f;
    public float PlayerColliderRadius = 0.8f;
    public float ProjectileDecaySeconds = 8f;
    

    private ParticleSystem[] particleAnims;
    private SphereCollider trigger_big;
    private SphereCollider trigger_small;
    private GameObject childCollider;
    private bool effectConsumed = false;
    private float startTime;

    private static _AbstractProjectileInternal shoot(GameObject projectilePrefab, Type projectileType, Vector3 startPos) {
        GameObject projectile = GameObject.Instantiate(projectilePrefab);
        projectile.transform.position = startPos;
        return (_AbstractProjectileInternal)projectile.AddComponent(projectileType);
    }

    protected static _AbstractProjectileInternal shoot<projectileType>(GameObject projectilePrefab, Vector3 startPos) {
        return shoot(projectilePrefab, typeof(projectileType), startPos);
    }

    private GameObject getChildCollider() {
        if (childCollider==null) {
            childCollider = new GameObject("_null");
            childCollider.transform.parent = transform;
		    childCollider.transform.localPosition = Vector3.zero;
        }
        return childCollider;
    }

    protected void Awake()
    {
        Init();
    }
    
    // Must be called by Awake() in subclasses if overridden
    protected void Init()
    {
        startTime = Time.time;
        particleAnims = transform.gameObject.GetComponentsInChildren<ParticleSystem>();
        GameManager = FindObjectOfType<GameManager>();
        PlayerTransform = GameManager.thePlayer.transform;
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        trigger_big = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
        trigger_big.isTrigger = true;
        trigger_big.radius = PlayerColliderRadius;
        
        getChildCollider();

        //gameObject.AddComponent(typeof(GameObject));
        Fireball_SmallCollider childColliderScript = getChildCollider().gameObject.AddComponent<Fireball_SmallCollider>();
        childColliderScript.setParent(this);
    }

    private class Fireball_SmallCollider : MonoBehaviour  {
        _AbstractProjectileInternal parent;
        void Init_c()
        {
            Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;
            rigidbody.isKinematic = true;
            parent.trigger_small = gameObject.AddComponent(typeof(SphereCollider)) as SphereCollider;
            parent.trigger_small.isTrigger = true;
            PropertyInfo attachedRigidBody = typeof(SphereCollider).GetProperty("attachedRigidbody");
            //attachedRigidBody.SetValue(parent.trigger_small, parent.rigidbody, null);
            parent.trigger_small.radius = parent.WorldColliderRadius;
        }
        public void setParent(_AbstractProjectileInternal parent) {
            this.parent = parent;
            this.Init_c();
        }
        void OnTriggerEnter(Collider col)
        {
            if (parent==null || col.isTrigger) return;
            if (col.gameObject != parent.gameObject && col.gameObject != parent.PlayerTransform.gameObject && col.GetComponent<Collider>() != null)
                parent.Destroy();
        }
        void Update(){
            if (parent==null) return;
            if (Time.time-parent.startTime>parent.ProjectileDecaySeconds) parent.Destroy();
        }
    }

    private void Destroy()  {
        effectConsumed = true;
        base.enabled = false;
        foreach (ParticleSystem particleAnim in particleAnims) particleAnim.Stop();
        UnityEngine.Object.Destroy(gameObject, 5);
    }

    public bool isDestroyed() {
        return effectConsumed;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject != getChildCollider() && col.gameObject == PlayerTransform.gameObject && !effectConsumed)
        {
            effectConsumed = true;
            if ((!GameManager.thePlayer.getInvincibilityBool()))
            {
                GameManager.editEnergyHealth(-25);
            }
                

            Fire_Player[] onFireEffects = PlayerTransform.gameObject.GetComponentsInChildren<Fire_Player>();
            bool isEffectActiveOnPlayer = false;
            foreach (Fire_Player fireEffect in onFireEffects) if (!fireEffect.effect_stopped) isEffectActiveOnPlayer = true;

            if (!isEffectActiveOnPlayer) {
                GameObject fireStatusObject = new GameObject("_null");
                fireStatusObject.transform.localScale = this.transform.localScale;

                GameObject currentParent = fireStatusObject;
                foreach (ParticleSystem particleAnim in particleAnims) {
                    GameObject newParent = UnityEngine.Object.Instantiate(particleAnim.gameObject);
                    newParent.transform.parent = currentParent.transform;
                    newParent.transform.localPosition = Vector3.zero;
                    currentParent = newParent;
                }
                
                fireStatusObject.transform.parent = col.transform;
                fireStatusObject.transform.localPosition = Vector3.zero;
                Fire_Player statusEffect = fireStatusObject.gameObject.AddComponent<Fire_Player>();
                statusEffect.Init_e(EffectDurationSeconds, fireStatusObject);
            }

            Destroy();
        }
    }

    private class Fire_Player : MonoBehaviour  {
        float effectStartTime;
        private ParticleSystem[] particleAnims;
        GameObject parentObject;

        private float effectDuration;

        private bool effect_initialized = false;

        public bool effect_stopped = false;

        void Start() {
            effectStartTime = Time.fixedTime;
            particleAnims = transform.gameObject.GetComponentsInChildren<ParticleSystem>();
        }

        public void Init_e(float effectDuration, GameObject parentObject) {
            this.effectDuration = effectDuration;
            this.parentObject = parentObject;
            effect_initialized = true;
        }

        private void Destroy()  {
            foreach (ParticleSystem particleAnim in particleAnims) particleAnim.Stop();
            UnityEngine.Object.Destroy(parentObject, 5);
            effect_stopped = true;
        }

        void Update() {
            if (effect_initialized && Time.fixedTime-effectStartTime >= effectDuration)
                this.Destroy();
        }
    }
}
