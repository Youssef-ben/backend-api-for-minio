using System.ComponentModel;

namespace Backend.Fileupload.API.Helpers.Errors
{
    public enum BackendLayersNames : int
    {
        /// <summary>
        /// Value : [Value:{REPOSITORY}, Description:{Repository.}]
        /// </summary>
        [Description("Repository.")]
        REPOSITORY,

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
