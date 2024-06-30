using BTG.Events;
using BTG.Utilities.EventBus;


namespace BTG.Actions.PrimaryAction
{
    /*
    /// <summary>
    /// Firing happens by charging the projectile and releasing it.
    /// </summary>
    public class ChargedFiring : IPrimaryAction, IUpdatable, IDestroyable
    {
        private const string FIRING_AUDIO_SOURCE_NAME = "FiringAudioSource";

        public event Action<TagSO> OnActionAssigned;
        public event Action OnActionStarted;
        public event Action<float> OnActionChargeUpdated;
        public event Action OnActionExecuted;

        /*[Inject]
            private AudioPool m_AudioPool;*/
    /*
        private IPrimaryActor m_Actor;
        private ProjectilePool m_ProjectilePool;
        private ChargedFiringDataSO m_Data;
        private AudioSource m_FiringAudioSource;
        private CancellationTokenSource m_Cts;

        private bool m_IsEnabled;
        private bool m_IsCharging;
        private float m_ChargeAmount;

        public ChargedFiring(ChargedFiringDataSO data, ProjectilePool projectilePool)
        {
            m_Data = data;
            m_ProjectilePool = projectilePool;
            // CreateFiringAudio();
        }

        public void Enable()
        {
            UnityMonoBehaviourCallbacks.Instance.RegisterToUpdate(this);
            UnityMonoBehaviourCallbacks.Instance.RegisterToDestroy(this);
            m_IsEnabled = true;
            // InitializeFiringAudio();
            OnActionAssigned?.Invoke(m_Data.Tag);
        }

        public void Update()
        {
            if (!m_IsEnabled)
                return;

            UpdateChargeAmount();
            ShootOnFullyCharged();
        }

        public void Disable()
        {
            ResetCharging();
            // DeInitializeFiringAudio();
            m_IsEnabled = false;
            m_Cts?.Cancel();

            UnityMonoBehaviourCallbacks.Instance.UnregisterFromUpdate(this);
            UnityMonoBehaviourCallbacks.Instance.UnregisterFromDestroy(this);
        }

        public void Destroy()
        {
            m_ProjectilePool.ClearPool();
            UnityMonoBehaviourCallbacks.Instance.UnregisterFromUpdate(this);
            UnityMonoBehaviourCallbacks.Instance.UnregisterFromDestroy(this);
        }

        public void SetActor(IPrimaryActor actor) => m_Actor = actor;

        public void StartAction()
        {
            if (!m_IsEnabled)
                return;

            m_IsCharging = true;
            // PlayChargingClip();
            OnActionStarted?.Invoke();
        }

        public void StopAction()
        {
            if (!m_IsEnabled)
                return;

            m_IsCharging = false;

            if (m_ChargeAmount <= 0f)
                return;

            SpawnProjectileAndShoot();
            OnActionExecuted?.Invoke();
            InvokeCameraShake();
            // PlayShotFiredClip();
            InvokeShootAudioEvent();
            ResetCharging();
        }

        private void InvokeCameraShake()
        {
            if (m_Actor.IsPlayer)
                m_Actor.RaisePlayerCamShakeEvent(new CameraShakeEventData { ShakeAmount = m_ChargeAmount, ShakeDuration = 0.5f });
        }

        private void InvokeShootAudioEvent()
        {
            EventBus<AudioEventData>.Invoke(new AudioEventData
            {
                AudioTag = m_Data.Tag,
                Position = m_Actor.FirePoint.position
            });
        }

        public void AutoStartStopAction(int stopTime)
        {
            StartAction();

            m_Cts = new CancellationTokenSource();

            _ = HelperMethods.InvokeAfterAsync(stopTime , () =>
            {
                StopAction();
            }, m_Cts.Token);
        }

        private void UpdateChargeAmount()
        {
            if (!m_IsCharging)
                return;

            m_ChargeAmount += Time.deltaTime / m_Data.ChargeTime;
            m_ChargeAmount = Mathf.Clamp01(m_ChargeAmount);
            // UpdateChargingClipPitch(m_ChargeAmount);
            OnActionChargeUpdated?.Invoke(m_ChargeAmount);
        }

        private void ShootOnFullyCharged()
        {
            if (m_ChargeAmount >= 1f)
            {
                StopAction();
            }
        }

        private void SpawnProjectileAndShoot()
        {
            SpawnProjectile(out ProjectileController projectile);
            projectile.AddImpulseForce(CalculateProjectileInitialSpeed());
        }

        private void ResetCharging()
        {
            m_ChargeAmount = 0f;
            // StopChargingClip();
        }

        private void SpawnProjectile(out ProjectileController projectile)
        {
            projectile = CreateProjectile();
            projectile.SetPositionAndRotation(m_Actor.FirePoint.position, m_Actor.FirePoint.rotation);
            projectile.SetOwnerOfView(m_Actor.Transform);
            projectile.SetActor(m_Actor);
            projectile.Init();
            projectile.ShowView();
        }

        private ProjectileController CreateProjectile()
        {
            ProjectileView view = m_ProjectilePool.GetProjectile();
            ProjectileController pc = new ProjectileController(m_Data, view);
            view.SetController(pc);
            // pc.SetAudioPool(m_AudioPool);
            return pc;
        }

        private float CalculateProjectileInitialSpeed()
        {
            return Mathf.Lerp(
                m_Data.MinInitialSpeed,
                m_Data.MaxInitialSpeed,
                m_ChargeAmount) + m_Actor.CurrentMoveSpeed;
        }

        private void PlayChargingClip()
        {
            m_FiringAudioSource.clip = m_Data.ChargeClip;
            m_FiringAudioSource.Play();
        }
    }
    */
    public class ChargedFiring : ChargedFiringBase
    {
        private ProjectilePool m_Pool;
        public ChargedFiring(ChargedFiringDataSO data, ProjectilePool projectilePool) : base(data)
        {
            m_Pool = projectilePool;
        }

        public override void Destroy()
        {
            m_Pool.ClearPool();
            base.Destroy();
        }

        protected override ProjectileController CreateProjectile()
        {
            ProjectileView view = m_Pool.GetProjectile();
            ProjectileController pc = new ProjectileController(chargedFiringData, view);
            view.SetController(pc);
            return pc;
        }

        protected override void InvokeShootAudioEvent()
        {
            EventBus<AudioEventData>.Invoke(new AudioEventData
            {
                AudioTag = chargedFiringData.Tag,
                Position = actor.FirePoint.position
            });
        }
    }
}