using StirlingPdf.Api;

namespace StirlingPdf
{
    /// <summary>
    /// Defines the root Stirling PDF API client contract.
    /// </summary>
    public interface IStirlingPdfClient
    {
        /// <summary>
        /// Gets the request builder for endpoints under <c>/api</c>.
        /// </summary>
        /// <remarks>
        /// This includes all <c>/api/v1</c> endpoint groups such as analysis, convert, filter, general, misc, pipeline, and security operations.
        /// </remarks>
        ApiRequestBuilder Api { get; }
    }
}
