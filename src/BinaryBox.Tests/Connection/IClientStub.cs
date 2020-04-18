using System.Threading.Tasks;

namespace BinaryBox.Connection.Test
{
    /// <summary>
    /// A stub client to verify the connection abstraction is called correctly.
    /// </summary>
    public interface IClientStub
    {
        /// <summary>
        /// Returns a result.
        /// </summary>
        /// <returns>The result.</returns>
        Task Result();

        /// <summary>
        /// Returns a result.
        /// </summary>
        /// <typeparam name="T">The result type.</typeparam>
        /// <returns>The result.</returns>
        Task<T> Result<T>();
    }
}