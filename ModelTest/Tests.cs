using OSDC.DotnetLibraries.Drilling.DrillingProperties;
using OSDC.DotnetLibraries.General.DataManagement;
using OSDC.DotnetLibraries.General.Statistics;
using NORCE.Drilling.DrillString.Model;

namespace NORCE.Drilling.DrillString.ModelTest
{
    public class Tests
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
        }

        [Test]
        public void Test_Calculus()
        {
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DrillStringSection drillStringSection1 = new()
            {
                Name = "My test DrillStringSection name 1",
            };
            DrillStringSection drillStringSection2 = new()
            {
                Name = "My test DrillStringSection name 2",
            };
            Model.DrillString myParentData = new()
            {
                MetaInfo = metaInfo,
                Name = "My test DrillString",
                DrillStringSectionList = [drillStringSection1, drillStringSection2]
            };

            Assert.That(myParentData, Is.Not.Null);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
        }
    }
}