using OpenBots.Core.Utilities.CommonUtilities;
using OpenBots.Engine;
using System.Security;
using Xunit;

namespace OpenBots.Commands.Credential.Tests
{
    public class GetCredentialCommandTests
    {
        private AutomationEngineInstance _engine;
        private GetCredentialCommand _getCredential;

        [Fact]
        public void GetsCredential()
        {
            _engine = new AutomationEngineInstance(null);
            _getCredential = new GetCredentialCommand();

            string credentialName = "CommandTestCreds";

            credentialName.StoreInUserVariable(_engine, "{credName}");

            _getCredential.v_CredentialName = "{credName}";
            _getCredential.v_OutputUserVariableName = "{username}";
            _getCredential.v_OutputUserVariableName2 = "{password}";

            _getCredential.RunCommand(_engine);

            SecureString expectedPass = "testPassword".GetSecureString();
            Assert.Equal("testUser", "{username}".ConvertUserVariableToString(_engine));
            Assert.Equal(expectedPass.ToString(),"{password}".ConvertUserVariableToObject(_engine).ToString());
        }
    }
}
