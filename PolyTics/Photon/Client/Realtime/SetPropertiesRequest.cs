using Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using System;
    using ExitGames.Client.Photon;

    public abstract class SetPropertiesRequest
    {
        public WebFlags WebFlags;
        public bool SendPropertiesChangedEvent = true;
        public SendOptions SendOptions = new SendOptions { Reliability = true };
        public bool HasExpectedProperties => this.expectedProperties == null || this.expectedProperties.Count == 0;

        protected Hashtable expectedProperties;
        protected Hashtable properties;

        internal Hashtable Properties => this.properties;
        internal Hashtable ExpectedProperties => this.expectedProperties;

        public void SetExpectedProperty<TK, TV>(TK propKey, TV propValue)
        {
            if (this.expectedProperties == null)
            {
                this.expectedProperties = new Hashtable();
            }
            if (propKey == null)
            {
                return;
            }
            this.expectedProperties[propKey] = propValue;
        }

        public void SetCustomProperties(Hashtable hashtable)
        {
            if (hashtable != null)
            {
                if (this.properties == null)
                {
                    this.properties = new Hashtable();
                }
                foreach (var pair in hashtable)
                {
                    this.properties[pair.Key] = pair.Value;
                }
            }
        }

        public void SetExpectedProperties(Hashtable hashtable)
        {
            if (hashtable != null)
            {
                if (this.expectedProperties == null)
                {
                    this.expectedProperties = new Hashtable();
                }
                foreach (var pair in hashtable)
                {
                    this.expectedProperties[pair.Key] = pair.Value;
                }
            }
        }

        public bool TryGetProperty<TK, TV>(TK propertyKey, out TV propertyValue)
        {
            if (this.properties != null && this.properties.TryGetValue(propertyKey, out object temp))
            {
                propertyValue = (TV) temp;
                return true;
            }
            propertyValue = default;
            return false;
        }

        public void SetProperty<TV>(byte propertyKey, TV propertyValue)
        {
            if (this.properties == null)
            {
                this.properties = new Hashtable();
            }
            this.properties[propertyKey] = propertyValue;
        }

        //public void Clear()
        //{
        //    this.properties?.Clear();
        //    this.expectedProperties?.Clear();
        //}
    }
}