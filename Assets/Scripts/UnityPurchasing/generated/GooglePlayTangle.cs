// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("Ljl6FbgHXaskfw6ZiBDTEFWkprsbqSoJGyYtIgGtY63cJioqKi4rKIFyZyN+a+3elUw4QansuQVNAScc1fdekdoFv3Qt7X4WViumZNBZr/jrRa4cxZ/lwn1wP74vzGSMZ3H3MrmdDz8A3+J7Zr0n+uBTYAV2AiBtPDMTmCTO1PW8LhJRM8P8yKo5PqZLnfGwbyyU+qreYwul6HM/ZU1Eg4YJV2monPnKn6V6IbbWWp613crW9IK50U479pPcvhOapRH0fgeNlp+hQPC9oKsFQtP52UHZ93eYdReV7RXKz6a6i2ASc1JeD2m3Cfy2PPiQqSokKxupKiEpqSoqK70JQlmR82FEZmZdFLtCmxYZARBMBs0KlhI55lhSxcHqjE/FgikoKisq");
        private static int[] order = new int[] { 2,2,11,12,5,10,7,12,12,12,12,12,12,13,14 };
        private static int key = 43;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
