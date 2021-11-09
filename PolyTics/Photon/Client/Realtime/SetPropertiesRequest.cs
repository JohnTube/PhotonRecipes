JohnTubeusing Photon.Realtime;

namespace PolyTics.Photon.Client.Realtime
{
    using ExitGames.Client.Photon;

    public abstract class SetPropertiesRequest
    {
        /// <summary>
        /// Flags to define PathGameProperties WebHook behaviour.
        /// </summary>
        public WebFlags WebFlags;
        /// <summary>
        /// Whether or not this operation should result in PropertiesChanged event sent to joined actors to sync their cached properties.
        /// </summary>
        public bool SendPropertiesChangedEvent = true;
        /// <summary>
        /// Generic options affecting the operation request.
        /// </summary>
        public SendOptions SendOptions = new SendOptions { Reliability = true };
        /// <summary>
        /// Returns whether or not this request includes Expected Properties.
        /// </summary>
        public bool HasExpectedProperties => this.expectedProperties == null || this.expectedProperties.Count == 0;

        protected Hashtable expectedProperties;
        protected Hashtable properties;

        internal Hashtable Properties => this.properties;
        internal Hashtable ExpectedProperties => this.expectedProperties;
        /// <summary>
        /// Adds a new key/value pair of expected properties or updates value of existing one. 
        /// </summary>
        /// <typeparam name="TK">Type of expected property key.</typeparam>
        /// <typeparam name="TV">Type of expected property value.</typeparam>
        /// <param name="propKey">Expected property key.</param>
        /// <param name="propValue">Expected property value.</param>
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
        /// <summary>
        /// Adds key/value pairs of properties or updates values of exiting ones.
        /// </summary>
        /// <param name="hashtable">Properties to set.</param>
        public void SetProperties(Hashtable hashtable)
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
        /// <summary>
        /// Adds key/value pairs of expected properties or updates values of exiting ones.
        /// </summary>
        /// <param name="hashtable">Expected Properties to set.</param>
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
        /// <summary>
        /// Try to get the property value if it exists in this request.
        /// </summary>
        /// <typeparam name="TK">Type of property key.</typeparam>
        /// <typeparam name="TV">Type of property value.</typeparam>
        /// <param name="propertyKey">Property key.</param>
        /// <param name="propertyValue">Property value.</param>
        /// <returns>If the property exists.</returns>
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
        /// <summary>
        /// Try to get the expected property value if it exists in this request.
        /// </summary>
        /// <typeparam name="TK">Type of expected property key.</typeparam>
        /// <typeparam name="TV">Type of expected property value.</typeparam>
        /// <param name="propertyKey">Expected property key.</param>
        /// <param name="propertyValue">Expected property value.</param>
        /// <returns>If the property exists.</returns>
        public bool TryGetExpectedProperty<TK, TV>(TK propertyKey, out TV propertyValue)
        {
            if (this.expectedProperties != null && this.expectedProperties.TryGetValue(propertyKey, out object temp))
            {
                propertyValue = (TV) temp;
                return true;
            }
            propertyValue = default;
            return false;
        }
        /// <summary>
        /// Adds a new key/value pair of properties or updates value of existing one. 
        /// </summary>
        /// <typeparam name="TK">Type of property key.</typeparam>
        /// <typeparam name="TV">Type of property value.</typeparam>
        /// <param name="propertyKey">Property key.</param>
        /// <param name="propertyValue">Property value.</param>
        public void SetProperty<TK, TV>(TK propertyKey, TV propertyValue)
        {
            if (this.properties == null)
            {
                this.properties = new Hashtable();
            }
            this.properties[propertyKey] = propertyValue;
        }
    }
}