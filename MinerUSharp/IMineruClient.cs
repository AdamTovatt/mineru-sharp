using MinerUSharp.Models;

namespace MinerUSharp
{
    /// <summary>
    /// Interface for the MinerU API client.
    /// </summary>
    public interface IMineruClient
    {
        /// <summary>
        /// Parses files using the MinerU API.
        /// </summary>
        /// <param name="request">The request parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="MineruResponse"/> containing the parsed content.</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="request"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown if the request is invalid.</exception>
        /// <exception cref="MinerUSharp.Exceptions.MineruApiException">Thrown if the API returns an error.</exception>
        Task<MineruResponse> ParseFileAsync(MineruRequest request, CancellationToken cancellationToken = default);
    }
}

