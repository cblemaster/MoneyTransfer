namespace MoneyTransfer.Security
{
    /// <summary>
    /// Represents a hashed password.
    /// </summary>
    /// <remarks>
    /// Creates a new hashed password.
    /// </remarks>
    /// <param name="password">The hashed password</param>
    /// <param name="salt">The salt used to get the hashed password.</param>
    public class PasswordHash(string password, string salt)
    {

        /// <summary>
        /// The hashed password
        /// </summary>
        public string Password { get; } = password;

        /// <summary>
        /// The salt used to get the hashed password.
        /// </summary>
        public string Salt { get; } = salt;

    }
}
