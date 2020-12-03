using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using OpenBots.Engine;
using OpenBots.Core.Utilities.CommonUtilities;

namespace OpenBots.Commands.Asset.Tests
{
    public class GetAssetCommandTests
    {
        private AutomationEngineInstance _engine;
        private GetAssetCommand _getAsset;

        [Fact]
        public void GetsAsset()
        {
            _engine = new AutomationEngineInstance(null);
            _getAsset = new GetAssetCommand();

            _getAsset.v_AssetName = "testTextAsset";
            _getAsset.v_AssetType = "Text";
            _getAsset.v_OutputDirectoryPath = "";
            _getAsset.v_OutputUserVariableName = "{output}";

            _getAsset.RunCommand(_engine);

            Assert.Equal("testText", "{output}".ConvertUserVariableToString(_engine));
        }
    }
}
