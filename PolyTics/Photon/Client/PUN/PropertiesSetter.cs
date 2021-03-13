using Photon.Pun;
using Photon.Realtime;

namespace PolyTics.Photon.Client.PUN
{
    using Realtime;
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    public class PropertiesSetter : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks
    {
        private bool pending;
        private readonly Queue<SetPropertiesCall> setPropertiesQueue = new Queue<SetPropertiesCall>();

        private void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
            PhotonNetwork.NetworkingClient.EventReceived += this.OnEventReceived;
            PhotonNetwork.NetworkingClient.OpResponseReceived += this.OnOpResponseReceived;
        }

        private void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
            PhotonNetwork.NetworkingClient.EventReceived -= this.OnEventReceived;
            PhotonNetwork.NetworkingClient.OpResponseReceived -= this.OnOpResponseReceived;
            this.Clear("script disabled");
        }

        private interface ISetPropertiesCall<T> where T : SetPropertiesRequest
        {
            T Request { get; }
            Action<T> SuccessCallback { get; }
            Action<T, string> FailureCallback { get; }
        }

        private abstract class SetPropertiesCall
        {
            public SetPropertiesRequest Request { get; protected set; }
            public abstract void CallSuccess();
            public abstract void CallFailure(string cause);
        }

        private class SetRoomPropertiesCall : SetPropertiesCall, ISetPropertiesCall<RoomPropertiesRequest>
        {
            public new RoomPropertiesRequest Request
            {
                get => base.Request as RoomPropertiesRequest;
                set => base.Request = value;
            }
            public Action<RoomPropertiesRequest> SuccessCallback { get; set; }
            public Action<RoomPropertiesRequest, string> FailureCallback { get; set; }

            public override void CallSuccess()
            {
                this.SuccessCallback?.Invoke(this.Request);
            }

            public override void CallFailure(string cause)
            {
                this.FailureCallback?.Invoke(this.Request, cause);
            }
        }

        private class SetActorPropertiesCall : SetPropertiesCall, ISetPropertiesCall<ActorPropertiesRequest>
        {
            public new ActorPropertiesRequest Request
            {
                get => base.Request as ActorPropertiesRequest;
                set => base.Request = value;
            }            
            public Action<ActorPropertiesRequest> SuccessCallback { get; set; }
            public Action<ActorPropertiesRequest, string> FailureCallback { get; set; }
            
            public override void CallSuccess()
            {
                this.SuccessCallback?.Invoke(this.Request);
            }

            public override void CallFailure(string cause)
            {
                this.FailureCallback?.Invoke(this.Request, cause);
            }
        }

        private void OnEventReceived(EventData photonEvent)
        {
            if (photonEvent.Code == EventCode.PropertiesChanged &&
                photonEvent.Sender == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                this.setPropertiesQueue.Dequeue().CallSuccess();
                this.pending = false;
            }
        }

        public bool SetRoomProperties(RoomPropertiesRequest request, Action<RoomPropertiesRequest> success,
            Action<RoomPropertiesRequest, string> failure)
        {
            SetRoomPropertiesCall call = new SetRoomPropertiesCall 
            {
                Request = request, 
                SuccessCallback = success, 
                FailureCallback = failure
            };
            this.setPropertiesQueue.Enqueue(call);
            return true;
        }

        public bool SetActorProperties(ActorPropertiesRequest request, Action<ActorPropertiesRequest> success,
            Action<ActorPropertiesRequest, string> failure)
        {
            SetActorPropertiesCall call = new SetActorPropertiesCall 
            {
                Request = request, 
                SuccessCallback = success, 
                FailureCallback = failure
            };
            this.setPropertiesQueue.Enqueue(call);
            return true;
        }

        private void OnOpResponseReceived(OperationResponse opResponse)
        {
            if (opResponse.OperationCode == OperationCode.SetProperties)
            {
                if (opResponse.ReturnCode == ErrorCode.Ok)
                {
                    if (!PhotonNetwork.CurrentRoom.BroadcastPropertiesChangeToAll ||
                        !this.setPropertiesQueue.Peek().Request.SendPropertiesChangedEvent)
                    {
                        this.setPropertiesQueue.Dequeue().CallSuccess();
                        this.pending = false;
                    }
                }
                else
                {
                    this.setPropertiesQueue.Dequeue().CallFailure(opResponse.DebugMessage);
                    this.pending = false;
                }
            }
        }

        private void Clear(string reason)
        {
            while (this.setPropertiesQueue.Count > 0)
            {
                this.setPropertiesQueue.Dequeue().CallFailure(reason);
            }
        }

        public void Update()
        {
            if (!this.pending)
            {
                if (this.setPropertiesQueue.Count > 0)
                {
                    SetPropertiesRequest request = this.setPropertiesQueue.Peek().Request;
                    if (request is RoomPropertiesRequest roomPropertiesRequest)
                    {
                        if (!PhotonNetwork.NetworkingClient.OpSetPropertiesOfRoom(roomPropertiesRequest))
                        {
                            this.setPropertiesQueue.Dequeue().CallFailure("Operation not sent, see error logs for more info");
                            return;
                        }
                    } 
                    else if (request is ActorPropertiesRequest propertiesRequest)
                    {
                        if (!PhotonNetwork.NetworkingClient.OpSetPropertiesOfActor(propertiesRequest))
                        {
                            this.setPropertiesQueue.Dequeue().CallFailure("Operation not sent, see error logs for more info");
                            return;
                        }
                    }
                    this.pending = true;
                }
            }
        }

        #region IConnectionCallbacks

        void IConnectionCallbacks.OnConnected()
        {
        }

        void IConnectionCallbacks.OnConnectedToMaster()
        {
        }

        void IConnectionCallbacks.OnDisconnected(DisconnectCause cause)
        {
            this.Clear(string.Format("client disconnected, cause: {0}", cause));
        }

        void IConnectionCallbacks.OnRegionListReceived(RegionHandler regionHandler)
        {
        }

        void IConnectionCallbacks.OnCustomAuthenticationResponse(Dictionary<string, object> data)
        {
        }

        void IConnectionCallbacks.OnCustomAuthenticationFailed(string debugMessage)
        {
        }

        #endregion

        #region IMatchmakingCallbacks

        void IMatchmakingCallbacks.OnFriendListUpdate(List<FriendInfo> friendList)
        {
        }

        void IMatchmakingCallbacks.OnCreatedRoom()
        {
        }

        void IMatchmakingCallbacks.OnCreateRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinedRoom()
        {
        }

        void IMatchmakingCallbacks.OnJoinRoomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnJoinRandomFailed(short returnCode, string message)
        {
        }

        void IMatchmakingCallbacks.OnLeftRoom()
        {
            this.Clear("Player left room.");
        }

        #endregion
    }
}