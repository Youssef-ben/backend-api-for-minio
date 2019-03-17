using System.ComponentModel;

namespace Backend.Manager.Helpers
{
    public enum BackendLayersNames : int
    {
        /// <summary>
        /// Value : [Value:{TEST}, Description:{Test.}]
        /// </summary>
        [Description("Test.")]
        TEST,

        /// <summary>
        /// Value : [Value:{MANAGER}, Description:{Manager.}]
        /// </summary>
        [Description("Manager.")]
        MANAGER,

        /// <summary>
        /// Value : [Value:{API}, Description:{Api.}]
        /// </summary>
        [Description("API.")]
        API,
    }
}
