using BTG.ConnectionManagement;
using BTG.Utilities;
using Unity.Multiplayer.Samples.BossRoom;
using Unity.Netcode;
using UnityEngine;


namespace BTG.Gameplay.GameplayObjects
{
    /// <summary>
    /// NetworkBehaviour that representes a player connection and is the "Default player prefab"
    /// inside NGO's NetworkManager.
    /// This NetworkBehaviour will contain several other NetworkBehaviours that should persist
    /// throughout the duration of this connection, meaning it will persist between scenes.
    /// </summary>
    /// <remarks>
    /// It is not necessary to explicitely mark this as Netcode will handle 
    /// migrating this player object between scene loads.
    /// </remarks>
    [RequireComponent(typeof(NetworkObject))]
    [RequireComponent(typeof(NetworkNameState))]
    [RequireComponent(typeof(NetworkEntityGuidState))]
    public class PersistentPlayer : NetworkBehaviour
    {
        [SerializeField]
        private PersistentPlayersRuntimeCollectionSO _persistentPlayerRuntimeCollection;

        private NetworkNameState _networkNameState;

        NetworkEntityGuidState _NetworkAvatarGuidState;

        public NetworkNameState NetworkNameState => _networkNameState;

        public NetworkEntityGuidState NetworkEntityGuidState => _NetworkAvatarGuidState;

        private void Awake()
        {
            _networkNameState = GetComponent<NetworkNameState>();
            _NetworkAvatarGuidState = GetComponent<NetworkEntityGuidState>();
        }

        public override void OnNetworkSpawn()
        {
            gameObject.name = "PersistentPlayer_" + OwnerClientId;

            // Note that this is done here on NetworkSpawn in case this NetworkBehaviour's properties are accessed
            // when this element is added to the runtime collection. If this was done in OnEnable() there is a chance
            // that OwnerClientID could be its default value (0).
            _persistentPlayerRuntimeCollection.Add(this);

            if (IsServer)
            {
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    var playerData = sessionPlayerData.Value;
                    _networkNameState.Name.Value = playerData.PlayerName;
                    if (playerData.HasCharacterSpawned)
                    {
                        _NetworkAvatarGuidState.n_NetworkEntityGuid.Value = playerData.AvatarNetworkGuid;
                    }
                    else
                    {
                        _NetworkAvatarGuidState.SetRandomEntity();
                        playerData.AvatarNetworkGuid = _NetworkAvatarGuidState.n_NetworkEntityGuid.Value;
                        SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                    }
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            RemovePersistentPlayer();
        }

        public override void OnNetworkDespawn()
        {
            RemovePersistentPlayer();
        }

        private void RemovePersistentPlayer()
        {
            _persistentPlayerRuntimeCollection.Remove(this);
            if (IsServer)
            {
                var sessionPlayerData = SessionManager<SessionPlayerData>.Instance.GetPlayerData(OwnerClientId);
                if (sessionPlayerData.HasValue)
                {
                    var playerData = sessionPlayerData.Value;
                    playerData.PlayerName = _networkNameState.Name.Value;
                    playerData.AvatarNetworkGuid = _NetworkAvatarGuidState.n_NetworkEntityGuid.Value;
                    SessionManager<SessionPlayerData>.Instance.SetPlayerData(OwnerClientId, playerData);
                }
            }
        }
    }
}

