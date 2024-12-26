using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using GlobalEnglish.Utility.Parameters;

namespace GlobalEnglish.Utility.Security
{
    /// <summary>
    /// A security token (for account identification and authentication).
    /// </summary>
    /// <remarks>
    /// <h4>Responsibilities:</h4>
    /// <list type="bullet">
    /// <item>knows a client account</item>
    /// <item>knows a token generation timestamp</item>
    /// <item>knows the valid lifetime of a token</item>
    /// <item>knows whether a token is stale (older than a valid lifetime)</item>
    /// </list>
    /// </remarks>
    public class SecurityToken
    {
        private static readonly string ValidAccounts = "Valid.Accounts";

        private static readonly string CryptoKey = "Crypto.Key";
        private static readonly string CryptoVector = "Crypto.Vector";
        private static readonly string TokenValidity = "Token.Validity";

        private static readonly int ValidityMinutes =
                                int.Parse(ConfiguredValue.Named(TokenValidity, "5"));

        /// <summary>
        /// The token validity duration in minutes.
        /// </summary>
        public static TimeSpan ValidDuration = TimeSpan.FromMinutes(ValidityMinutes);

        /// <summary>
        /// A client account.
        /// </summary>
        public int Account { get; set; }

        /// <summary>
        /// A token generation timestamp.
        /// </summary>
        public DateTime Timestamp { get; set; }

        #region creating instances
        /// <summary>
        /// Returns a new SecurityToken.
        /// </summary>
        /// <param name="account">a known account</param>
        /// <returns>a new SecurityToken</returns>
        public static SecurityToken With(string account)
        {
            return With(account.GetHashCode());
        }

        /// <summary>
        /// Returns a new SecurityToken.
        /// </summary>
        /// <param name="accountID">identifies a known account</param>
        /// <returns>a new SecurityToken</returns>
        public static SecurityToken With(int accountID)
        {
            return With(accountID, DateTime.Now);
        }

        /// <summary>
        /// Returns a new SecurityToken.
        /// </summary>
        /// <param name="accountID">identifies a known account</param>
        /// <param name="timestamp">a timestamp</param>
        /// <returns>a new SecurityToken</returns>
        public static SecurityToken With(int accountID, DateTime timestamp)
        {
            SecurityToken result = new SecurityToken();
            result.Timestamp = timestamp;
            result.Account = accountID;
            return result;
        }

        /// <summary>
        /// Constructs a new SecurityToken.
        /// </summary>
        private SecurityToken()
        {
        }
        #endregion

        #region binary tokens
        /// <summary>
        /// Returns a binary token converted to text.
        /// </summary>
        /// <returns>a text token value</returns>
        public override string ToString()
        {
            return Symmetric.GetHex(Symmetric.Package(ToBinary()));
        }

        /// <summary>
        /// Returns the binary value of this token.
        /// </summary>
        /// <returns>the binary value of this token</returns>
        public byte[] ToBinary()
        {
            long time = Timestamp.Ticks;
            long acct = Account;
            byte[] timeBits = BitConverter.GetBytes(time);
            byte[] acctBits = BitConverter.GetBytes(acct);
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(timeBits, 0, timeBits.Length);
                stream.Write(acctBits, 0, acctBits.Length);
                stream.Flush();
                byte[] bits = stream.ToArray();
                return Cryptographer.Encrypt(bits);
            }
        }

        /// <summary>
        /// Returns a new SecurityToken.
        /// </summary>
        /// <param name="tokenValue">a text token value</param>
        /// <returns>a new SecurityToken</returns>
        public static SecurityToken From(string tokenValue)
        {
            return From(Symmetric.Unpack(Symmetric.GetBytes(tokenValue)));
        }

        /// <summary>
        /// Returns a new SecurityToken.
        /// </summary>
        /// <param name="tokenValue">a binary token value</param>
        /// <returns>a new SecurityToken</returns>
        public static SecurityToken From(byte[] tokenValue)
        {
            long sample = 0;
            byte[] sampleBits = BitConverter.GetBytes(sample);
            byte[] bits = Cryptographer.Decrypt(tokenValue);
            using (MemoryStream stream = new MemoryStream(bits))
            {
                byte[] timeBits = new byte[sampleBits.Length];
                byte[] acctBits = new byte[sampleBits.Length];
                stream.Read(timeBits, 0, timeBits.Length);
                stream.Read(acctBits, 0, acctBits.Length);

                long hash = BitConverter.ToInt64(acctBits, 0);
                DateTime timestamp = DateTime.FromBinary(BitConverter.ToInt64(timeBits, 0));
                return With((int)hash, timestamp);
            }
        }

        /// <summary>
        /// A configured cryptographer.
        /// </summary>
        private static Symmetric Cryptographer
        {
            get { return Symmetric.WithConfigured(CryptoVector, CryptoKey); }
        }
        #endregion

        #region testing validity
        /// <summary>
        /// Indicates whether this token matches any known account.
        /// </summary>
        public bool MatchesKnownAccount
        {
            get { return MatchesAnyAccount(GetValidAccounts()); }
        }

        /// <summary>
        /// Indicates whether this token matches any account key.
        /// </summary>
        /// <param name="accounts">the known account pairs</param>
        /// <returns>whether a matching account key was found</returns>
        public bool MatchesAnyAccount(IDictionary<string, string> accounts)
        {
            string knownAccount = FindMatchedAccountName(accounts);
            return knownAccount.Length > 0;
        }

        /// <summary>
        /// Finds the account name that matches this token.
        /// </summary>
        /// <param name="accounts">account name-value pairs</param>
        /// <returns>an account name, or empty</returns>
        public string FindMatchedAccountName(IDictionary<string, string> accounts)
        {
            foreach (string account in accounts.Keys)
            {
                if (account.GetHashCode() == Account) return account;
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the known (configured) accounts.
        /// </summary>
        /// <returns>the known accounts</returns>
        public static IDictionary<string, string> GetValidAccounts()
        {
            return GetAccountPairs(ValidAccounts);
        }

        /// <summary>
        /// Returns the configured account value pairs.
        /// </summary>
        /// <param name="valueName">a configured value name</param>
        /// <returns>the configured account value pairs</returns>
        public static IDictionary<string, string> GetAccountPairs(string valueName)
        {
            string accounts = ConfiguredValue.Named(valueName);
            Argument.Check("AppSettings[valueName]", accounts);

            string[] pairs = accounts.Split(';');
            Dictionary<string, string> result = new Dictionary<string, string>();
            foreach (string credentialPair in pairs)
            {
                string[] parts = credentialPair.Split('=');
                result.Add(parts[0].Trim(), parts[1].Trim());
            }
            return result;
        }

        /// <summary>
        /// Indicates whether the valid usage period for this token has expired.
        /// </summary>
        public bool IsExpired
        {
            get { return (DateTime.Now > ExpirationTime); }
        }

        /// <summary>
        /// The token expiration time.
        /// </summary>
        public DateTime ExpirationTime
        {
            get { return Timestamp + ValidDuration; }
        }
        #endregion

    } // SecurityToken
}
