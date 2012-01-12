// -----------------------------------------------------------------------
// <copyright file="ThirdPartyManualTests.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace BDSA_Project_ThirdParty
{
    using System.Threading;

    using NUnit.Framework;

    /// <summary>
    /// Manual tests for basic third party database operations based on scenario_chart THIRD_PARTY_MANUAL_TESTS
    /// <author>Janus Varmarken</author>
    /// </summary>
    [TestFixture]
    public class ThirdPartyManualTests
    {
        private ThirdParty database;
        
        [SetUp]
        public void Init()
        {
            this.database = new ThirdParty();
        }

        [Test]
        public void TestAddUser()
        {
            string dwd = "darkwingduck";
            string email = "e@dwd.dk";
            Assert.False(this.database.ContainsUsername(dwd));
            Assert.True(this.database.AddUserAccount(dwd, email));
            Assert.True(this.database.ContainsUsername(dwd));
            // Not possible to add user with equal username:
            Assert.False(this.database.AddUserAccount(dwd, email));
        }
        
        [Test]
        public void TestCompareTokensOnInvalidUser()
        {
            Assert.False(this.database.CompareTokens("1234", "donaldduck"));
        }
        
        [Test]
        public void TestAuthUpdateTokenForInvalidUser()
        {
            Assert.False(this.database.SetAuthTokenForAccount("pixeline", "2430")); // Todo change method to bool return type for testing purposes
        }

        [Test]
        public void TestSuccessfulAuthTokenUpdate()
        {
            Assert.True(this.database.AddUserAccount("yoda", "yoda@yoda.dk"));
            Assert.True(this.database.SetAuthTokenForAccount("yoda", "5691"));
        }

        [Test]
        public void TestSuccessfulTokenCompare()
        {
            string usr = "chucknorris";
            string tkn = "1558";
            Assert.True(this.database.AddUserAccount(usr, "usr@ussr.ru"));
            Assert.True(this.database.SetAuthTokenForAccount(usr, tkn));
            Assert.True(this.database.CompareTokens(tkn, usr));
        }

        [Test]
        public void TestTokenInitialTimeout()
        {
            string usr = "bond";
            string tkn = "0000"; // initial value of the auth token field in ThirdPartyUserAccount
            Assert.True(this.database.AddUserAccount(usr, "email@em.dk"));
            Assert.False(this.database.CompareTokens(tkn,usr)); // the server running the database is assumed to have a somewhat correct value of DateTime.Now (at least 1 minute greater than DateTime.MinValue)
        }

        [Test]
        public void TestTokenTimeout()
        {
            string usr = "bond";
            string tkn = "4558";
            Assert.True(this.database.AddUserAccount(usr, "bond@007.dk"));
            Assert.True(this.database.SetAuthTokenForAccount(usr, tkn));
            Thread.Sleep(1001); // token is invalid after 1 minute
            Assert.False(this.database.CompareTokens(usr, tkn));
        }

        [Test]
        public void TestCorrectTokenAfterIncorrectToken()
        {
            string usr = "ratata";
            string validTkn = "1598";
            string invalidTkn = "1599";
            Assert.True(this.database.AddUserAccount(usr, "lucky@luke.dk"));
            Assert.True(this.database.SetAuthTokenForAccount(usr, validTkn));
            Assert.False(this.database.CompareTokens(usr, invalidTkn)); // wrong token
            Assert.False(this.database.CompareTokens(usr, validTkn)); // token is correct, but access not allowed since an earlier attempt was made
        }

        [Test]
        public void TestThirdPartyUserAccountUsernameProperty()
        {
            ThirdPartyUserAccount tpua = new ThirdPartyUserAccount("kingkong", "king@kong.dk");
            Assert.True(tpua.Username.Equals("kingkong"));
        }
    }    
}