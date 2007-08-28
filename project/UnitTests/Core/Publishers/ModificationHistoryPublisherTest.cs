using System;
using System.IO;
using System.Xml;
using Exortech.NetReflector;
using NUnit.Framework;
using ThoughtWorks.CruiseControl.Core;
using ThoughtWorks.CruiseControl.Core.Publishers;
using ThoughtWorks.CruiseControl.Core.Util;
using ThoughtWorks.CruiseControl.Remote;

namespace ThoughtWorks.CruiseControl.UnitTests.Core.Publishers
{
    [TestFixture]
    public class ModificationHistoryPublisherTest
    {

        public static readonly string FULL_CONFIGURED_LOG_DIR = "FullConfiguredLogDir";
        public static readonly string FULL_CONFIGURED_LOG_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(FULL_CONFIGURED_LOG_DIR));

        public static readonly string ARTIFACTS_DIR = "Artifacts";
        public static readonly string ARTIFACTS_DIR_PATH = Path.GetFullPath(TempFileUtil.GetTempPath(ARTIFACTS_DIR));

        private ModificationHistoryPublisher publisher;

        [SetUp]
        public void SetUp()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
            TempFileUtil.CreateTempDir(ARTIFACTS_DIR);

            publisher = new ModificationHistoryPublisher();
        }

        [TearDown]
        public void TearDown()
        {
            TempFileUtil.DeleteTempDir(FULL_CONFIGURED_LOG_DIR);
            TempFileUtil.DeleteTempDir(ARTIFACTS_DIR);
        }

        [Test]
        public void BuildWithoutModificationsShouldPublishNoModifications()
        {
            // Setup
            IntegrationResult result = CreateIntegrationResult(IntegrationStatus.Success, false);
            result.ArtifactDirectory = ARTIFACTS_DIR_PATH;
            string PublishedModifications;
            string ExpectedLoggedModifications = string.Format("<History><Build BuildDate=\"{0}\" Success=\"True\" Label=\"{1}\" />\r\n</History>",
                                                    DateUtil.FormatDate(result.StartTime), result.Label);

            // Execute
            publisher.Run(result);            

            //Verify
            PublishedModifications = ModificationHistoryPublisher.LoadHistory(ARTIFACTS_DIR_PATH);

            Assert.AreEqual(ExpectedLoggedModifications, PublishedModifications, "Differences in log Detected");
        }

  
        private IntegrationResult CreateIntegrationResult(IntegrationStatus status, bool addModifications)
        {
            IntegrationResult result = IntegrationResultMother.Create(status, new DateTime(1980, 1, 1));
            result.ProjectName = "proj";
            result.StartTime = new DateTime(1980, 1, 1);
            result.Label = "1";
            result.Status = status;
            if (addModifications)
            {
                Modification[] modifications = new Modification[1];
                modifications[0] = new Modification();
                modifications[0].ModifiedTime = new DateTime(2002, 2, 3);
                result.Modifications = modifications;
            }
            return result;
        }

    }
}
