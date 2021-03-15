using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using System;
    using System.Collections.Generic;
    using ExitGames.Client.Photon;

    public class PropertiesSetter : IDisposable
    {
        private bool pending;
        private readonly Queue<SetPropertiesCall> setPropertiesQueue = new Queue<SetPropertiesCall>();
        private readonly LoadBalancingClient loadBalancingClient;
        private readonly bool clearOnLeave;
        private readonly bool clearOnDisconnect;
        private readonly bool queueUntilJoined;
        private readonly int maxFailure;
        private int failureCount;

        public bool Disposed { get; private set; }

        public PropertiesSetter(LoadBalancingClient client, bool clearOnLeave, bool clearOnDisconnect, bool queueUntilJoined, int maxFailure)
        {
            this.loadBalancingClient = client;
            this.loadBalancingClient.EventReceived += this.OnEventReceived;
            this.loadBalancingClient.OpResponseReceived += this.OnOpResponseReceived;
            this.loadBalancingClient.StateChanged += this.OnStateChanged;
            this.clearOnLeave = clearOnLeave;
            this.clearOnDisconnect = clearOnDisconnect;
            this.queueUntilJoined = queueUntilJoined;
            this.maxFailure = maxFailure;
        }

        ~PropertiesSetter()
        {
            this.Dispose();
        }

        private void OnStateChanged(ClientState previousState, ClientState currentState)
        {
            if (previousState == ClientState.Joined)
            {
                switch (currentState)
                {
                    case ClientState.Disconnected:
                    case ClientState.Disconnecting:
                        break;
                    default:
                        if (this.clearOnLeave)
                        {
                            this.Clear("Actor left room.");
                        }
                        return;
                }
            }
            if (currentState == ClientState.Disconnected && this.clearOnDisconnect)
            {
                this.Clear(string.Format("Client disconnected, cause: {0}.", this.loadBalancingClient.DisconnectedCause));
            }
            if (currentState == ClientState.Joined)
            {
                this.Set();
            }
        }
        
        private interface ISetPropertiesCall<T> where T : SetPropertiesRequest
        {
            T Request { get; }
            Action<T> SuccessCallback { get; }
            Action<T, string> FailureCallback { get; }
            int Retries { get; }
        }

        private abstract class SetPropertiesCall
        {
            public SetPropertiesRequest Request { get; protected set; }
            public abstract void CallSuccess();
            public abstract void CallFailure(string cause);
            public int Retries { get; set; }
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
                photonEvent.Sender == this.loadBalancingClient.LocalPlayer.ActorNumber)
            {
                this.OnSuccess();
            }
        }

        public bool SetRoomProperties(RoomPropertiesRequest request, Action<RoomPropertiesRequest> success,
            Action<RoomPropertiesRequest, string> failure, int retries = 0)
        {
            if (!this.loadBalancingClient.InRoom && !this.queueUntilJoined)
            {
                failure(request, "Operation not queued, client is not joined to a room.");
                return false;
            }
            SetRoomPropertiesCall call = new SetRoomPropertiesCall
            {
                Request = request,
                SuccessCallback = success,
                FailureCallback = failure,
                Retries = retries
            };
            this.setPropertiesQueue.Enqueue(call);
            this.Set();
            return true;
        }

        public bool SetActorProperties(ActorPropertiesRequest request, Action<ActorPropertiesRequest> success,
            Action<ActorPropertiesRequest, string> failure, int retries = 0)
        {
            if (!this.loadBalancingClient.InRoom && !this.queueUntilJoined)
            {
                failure(request, "Operation not queued, client is not joined to a room.");
                return false;
            }
            SetActorPropertiesCall call = new SetActorPropertiesCall
            {
                Request = request,
                SuccessCallback = success,
                FailureCallback = failure,
                Retries = retries
            };
            this.setPropertiesQueue.Enqueue(call);
            this.Set();
            return true;
        }

        private void OnOpResponseReceived(OperationResponse opResponse)
        {
            if (opResponse.OperationCode == OperationCode.SetProperties)
            {
                if (opResponse.ReturnCode == ErrorCode.Ok)
                {
                    SetPropertiesRequest request = this.setPropertiesQueue.Peek().Request;
                    if (!this.loadBalancingClient.CurrentRoom.BroadcastPropertiesChangeToAll && request.HasExpectedProperties ||
                        !request.SendPropertiesChangedEvent)
                    {
                        this.OnSuccess();
                    }
                }
                else
                {
                    this.OnFailure(opResponse.DebugMessage);
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

        private void Set()
        {
            if ((!this.queueUntilJoined || this.loadBalancingClient.InRoom) && !this.Disposed && !this.pending && this.setPropertiesQueue.Count > 0)
            {
                SetPropertiesCall call = this.setPropertiesQueue.Peek();
                if (call.Retries > 0)
                {
                    call.Retries--;
                }
                SetPropertiesRequest request = call.Request;
                bool failed = false;
                if (request is RoomPropertiesRequest roomPropertiesRequest)
                {
                    failed = !this.loadBalancingClient.OpSetPropertiesOfRoom(roomPropertiesRequest);
                }
                else if (request is ActorPropertiesRequest actorPropertiesRequest)
                {
                    failed = !this.loadBalancingClient.OpSetPropertiesOfActor(actorPropertiesRequest);
                }
                if (failed)
                {
                    this.OnFailure("Operation not sent, see error logs for more info.");
                    return;
                }
                this.pending = true;
            }
        }

        private void OnFailure(string cause)
        {
            this.pending = false;
            if (this.setPropertiesQueue.Peek().Retries == 0)
            {
                this.setPropertiesQueue.Dequeue().CallFailure(cause);
            }
            if (this.maxFailure > 0)
            {
                this.failureCount++;
                if (this.failureCount == this.maxFailure)
                {
                    this.Clear(string.Format("Max failures {0} reached.", this.failureCount));
                    return;
                }
            }
            this.Set();
        }

        private void OnSuccess()
        {
            this.pending = false;
            this.setPropertiesQueue.Dequeue().CallSuccess();
            this.failureCount = 0;
            this.Set();
        }

        public void Dispose()
        {
            if (!this.Disposed)
            {
                this.Clear("PropertiesSetter disposed.");
                this.loadBalancingClient.EventReceived -= this.OnEventReceived;
                this.loadBalancingClient.OpResponseReceived -= this.OnOpResponseReceived;
                this.loadBalancingClient.StateChanged -= this.OnStateChanged;
                this.Disposed = true;
            }
        }
    }
}