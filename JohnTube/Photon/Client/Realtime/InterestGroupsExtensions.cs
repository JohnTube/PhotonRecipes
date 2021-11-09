using Photon.Realtime;

namespace JohnTube.Photon.Client.Realtime
{
    using System;
    using System.Linq;

    public static partial class ExtensionMethods
    {
        internal static readonly byte[] emptyByteArray = new byte[0];

        internal static readonly byte[] allByteValues = Enumerable.Range(1, 255).SelectMany(BitConverter.GetBytes).ToArray();

        public static bool AddInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.OpChangeGroups(null, new[] { group });
        }

        public static bool AddInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.OpChangeGroups(null, groups);
        }

        public static bool RemoveInterestGroup(this LoadBalancingClient client, byte group)
        {
            return client.OpChangeGroups(new[] { group }, null);
        }

        public static bool RemoveInterestGroups(this LoadBalancingClient client, byte[] groups)
        {
            return client.OpChangeGroups(groups, null);
        }

        public static bool AddAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(null, emptyByteArray);
        }

        public static bool RemoveAllExistingInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(emptyByteArray, null);
        }

        public static bool AddAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(null, allByteValues);
        }

        public static bool RemoveAllPossibleInterestGroups(this LoadBalancingClient client)
        {
            return client.OpChangeGroups(allByteValues, null);
        }
    }
}
