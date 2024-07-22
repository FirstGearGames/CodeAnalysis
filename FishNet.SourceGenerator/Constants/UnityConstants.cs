namespace SourceGenerating.Constants
{
    internal class UnityConstants
    {
        #region Namespaces and assemblies.
        /// <summary>
        /// UnityEngine namespace.
        /// </summary>
        public const string UnityEngine_Namespace = "UnityEngine";
        #endregion

        #region Attributes.
        /// <summary>
        /// [RuntimeInitializeOnLoadMethod] class.
        /// </summary>
        public const string RuntimeInitializeOnLoadMethod_Attribute_FullName = $"{UnityEngine_Namespace}.{RuntimeInitializeOnLoadMethod_FullName}Attribute";
        /// <summary>
        /// RuntimeInitializeOnLoadMethod attribute.
        /// </summary>
        public const string RuntimeInitializeOnLoadMethod_FullName = $"{UnityEngine_Namespace}.RuntimeInitializeOnLoadMethod";
        #endregion


    }
}