using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

using NUnit.Framework;

namespace GlobalEnglish.Utility.Security
{
    /// <summary>
    /// Verifies that a security token works as intended.
    /// </summary>
    [TestFixture]
    public class TokenTestFixture
    {
        private readonly string[] TestAccountsName = { "sample.client.username", "svr.client.username" };
        private readonly string[] TestAccountsPwd = { "sample.client.password", "svr.client.password" };
        private readonly string ConfiguredValidAccounts = "Valid.Accounts";
        private readonly string ConfiguredInValidAccounts = "InValid.Accounts";
        /// <summary>
        /// Verifies that it works as intended.
        /// </summary>
        [Test]
        public void SecurityTokenWithTransport()
        {
            // create a token
            SecurityToken token = SecurityToken.With(45);
            byte[] tokenValue = token.ToBinary();

            // mimic transport through a string
            string hexToken = Symmetric.GetHex(tokenValue);
            byte[] tokenBinary = Symmetric.GetBytes(hexToken);

            // decode the token
            SecurityToken result = SecurityToken.From(tokenBinary);
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Account == 45);
            Assert.IsTrue(result.Timestamp <= DateTime.Now);
        }

        /// <summary>
        /// Verifies that a security token works symmetrically.
        /// </summary>
        [Test]
        public void SecurityTokenSymmetry()
        {
            SecurityToken token = SecurityToken.With(45);
            string tokenValue = token.ToString();

            SecurityToken result = SecurityToken.From(tokenValue);
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Account == 45);
            Assert.IsTrue(result.Timestamp <= DateTime.Now);
        }

        /// <summary>
        /// Verifies that a security token works with the configured TTL.
        /// </summary>
        [Test]
        public void SecurityTokenSymmetryWithConfiguredTTL()
        {
            DateTime now = DateTime.Now;
            SecurityToken token = SecurityToken.With(45, now);
            string tokenValue = token.ToString();

            SecurityToken result = SecurityToken.From(tokenValue);
            Assert.IsTrue(result != null);
            Assert.IsTrue(result.Account == 45);
            Assert.IsTrue(result.Timestamp <= DateTime.Now);
            int ttl = Convert.ToInt32(ConfigurationManager.AppSettings.Get("Token.Validity"));
            Assert.IsTrue(token.ExpirationTime.Minute == now.AddMinutes(ttl).Minute);
        }

        /// <summary>
        /// To test if the implementation gets configured valus from config.
        /// </summary>
        [Test]
        public void GetValidAccountsTest()
        {
            IDictionary<string, string> accounts = SecurityToken.GetValidAccounts();
            Assert.IsTrue(accounts.Count == 2);
            Assert.IsTrue(accounts.ContainsKey(TestAccountsName[0]));
            Assert.IsTrue(accounts.ContainsKey(TestAccountsName[1]));
            Assert.IsTrue(accounts[TestAccountsName[0]] == TestAccountsPwd[0]);
             Assert.IsTrue(accounts[TestAccountsName[1]] == TestAccountsPwd[1]);
        }

        /// <summary>
        /// To test account name passed matches configured value Method tested MatchesKnownAccount
        /// </summary>
        [Test]
        public void MatchesKnownAccountTest()
        {
            SecurityToken secToken = SecurityToken.With(TestAccountsName[0]);
            bool result = secToken.MatchesKnownAccount;
            Assert.IsTrue(result == true);
            SecurityToken badToken = SecurityToken.With("unknown Account");
            bool badResult = badToken.MatchesKnownAccount;
            Assert.IsTrue(badResult == false);
           
        }

        /// <summary>
        /// To test account name passed matches any configured value Method tested  MatchesAnyAccount
        /// </summary>
        [Test]
        public void MatchesAnyAccountTest()
        {
            IDictionary<string, string> Validaccounts = SecurityToken.GetAccountPairs(ConfiguredValidAccounts);
            SecurityToken secToken = SecurityToken.With(TestAccountsName[0]);
            bool result = secToken.MatchesAnyAccount(Validaccounts);
            Assert.IsTrue(result == true);
            IDictionary<string, string> Invalidaccounts = SecurityToken.GetAccountPairs(this.ConfiguredInValidAccounts);
            bool badResult = secToken.MatchesAnyAccount(Invalidaccounts);
            Assert.IsTrue(badResult == false);

        }


    } // TokenTestFixture
}
