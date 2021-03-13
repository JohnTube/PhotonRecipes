using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using ExitGames.Client.Photon;

    public abstract class SetPropertiesRequest
    {
        public WebFlags WebFlags;
        public bool SendPropertiesChangedEvent = true;
        public SendOptions SendOptions = SendOptions.SendReliable;
        protected Hashtable expectedProperties;

        public Hashtable ExpectedProperties
        {
            get { return this.GetExpectedProperties(); }
            set { this.expectedProperties = this.SetExpectedProperties(value); }
        }

        public Hashtable CustomProperties;

        public void SetCustomProperty(object propKey, object propValue)
        {
            if (this.CustomProperties == null)
            {
                this.CustomProperties = new Hashtable();
            }

            if (propKey == null)
            {
                return;
            }

            this.CustomProperties[propKey] = propValue;
        }

        public void SetExpectedProperty(object propKey, object propValue)
        {
            if (this.ExpectedProperties == null)
            {
                this.ExpectedProperties = new Hashtable();
            }

            if (propKey == null)
            {
                return;
            }

            this.ExpectedProperties[propKey] = propValue;
        }

        public Hashtable ToHashtable()
        {
            Hashtable hash = this.CustomProperties;
            if (hash == null)
            {
                hash = new Hashtable();
            }

            this.AddToHashtable(hash);
            return hash;
        }

        protected virtual void AddToHashtable(Hashtable hash)
        {

        }

        protected virtual Hashtable SetExpectedProperties(Hashtable hash)
        {
            return hash;
        }

        protected virtual Hashtable GetExpectedProperties()
        {
            return this.expectedProperties;
        }
    }
}