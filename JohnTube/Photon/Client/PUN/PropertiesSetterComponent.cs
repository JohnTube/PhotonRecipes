﻿using Photon.Pun;

namespace JohnTube.Photon.Client.PUN
{
    using System;
    using Realtime;
    using UnityEngine;

    public class PropertiesSetterComponent : MonoBehaviour
    {
        private PropertiesSetter propertiesSetter;
        #pragma warning disable 649
        [SerializeField] private bool clearOnLeave, clearOnDisconnect, queueUntilJoined;
        [SerializeField] private int maxFailure;
        #pragma warning restore 649

        private void Awake()
        {
            this.propertiesSetter = new PropertiesSetter(PhotonNetwork.NetworkingClient, this.clearOnLeave, this.clearOnDisconnect, this.queueUntilJoined, this.maxFailure);
        }

        private void OnDestroy()
        {
            this.propertiesSetter.Dispose();
        }

        public bool SetRoomProperties(RoomPropertiesRequest request, Action<RoomPropertiesRequest> success,
            Action<RoomPropertiesRequest, string> failure, int retries = 0)
        {
            return this.propertiesSetter.SetRoomProperties(request, success, failure, retries);
        }

        public bool SetActorProperties(ActorPropertiesRequest request, Action<ActorPropertiesRequest> success,
            Action<ActorPropertiesRequest, string> failure, int retries = 0)
        {
            return this.propertiesSetter.SetActorProperties(request, success, failure, retries);
        }
    }

}