﻿using BTG.Utilities;
using UnityEngine;


namespace BTG.Actions.PrimaryAction
{
    [RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
    public class TeslaBallView : MonoBehaviour
    {
        [SerializeField]
        Rigidbody m_Rigidbody;

        [SerializeField]
        ParticleSystem m_ParticleSytem;

        [SerializeField]
        SphereCollider m_Collider;
        public SphereCollider Collider => m_Collider;

        private TeslaBallPool m_Pool;

        private int m_Damage;

        private void OnEnable()
        {
            m_Collider.enabled = true;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(m_Damage);
            }

            Reset();
        }

        private void OnDisable()
        {
            m_Collider.enabled = false;
        }

        /// <summary>
        /// Initialize the tesla ball.
        /// It will be shown and the particle system will start playing
        /// </summary>
        public void Init()
        {
            Show();
        }

        /// <summary>
        /// Add impulse force to the tesla ball to move it forward
        /// </summary>
        public void AddImpulseForce(float force)
        {
            m_Rigidbody.AddForce(transform.forward * force, ForceMode.Impulse);
        }

        /// <summary>
        /// Set the pool to return the tesla ball to
        /// </summary>
        public void SetPool(TeslaBallPool pool)
        {
            m_Pool = pool;
        }

        /// <summary>
        /// Set the damage the tesla ball will do
        /// </summary>
        public void SetDamage(int damage)
        {
            m_Damage = damage;
        }

        private void Reset()
        {
            m_Rigidbody.velocity = Vector3.zero;
            m_Rigidbody.angularVelocity = Vector3.zero;
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            
            Hide();

            m_Pool.ReturnTeslaBall(this);
        }

        private void Show()
        {
            gameObject.SetActive(true);
            m_ParticleSytem.Play();
        }

        private void Hide()
        {
            m_ParticleSytem.Stop();
            gameObject.SetActive(false);
        }
    }
}
