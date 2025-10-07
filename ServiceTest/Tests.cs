using System.Net.Http.Headers;
using NORCE.Drilling.DrillString.ModelShared;

namespace ServiceTest
{
    public class Tests
    {
        // testing outside Visual Studio requires using http port (https faces authentication issues both in console and on github)
        private static string host = "http://localhost:8080/";
        //private static string host = "https://localhost:5001/";
        //private static string host = "https://localhost:44368/";
        //private static string host = "http://localhost:54949/";
        private static HttpClient httpClient;
        private static Client nSwagClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }; // temporary workaround for testing purposes: bypass certificate validation (not recommended for production environments due to security risks)
            httpClient = new HttpClient(handler);
            httpClient.BaseAddress = new Uri(host + "DrillString/api/");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            nSwagClient = new Client(httpClient.BaseAddress.ToString(), httpClient);
        }

        [Test]
        public async Task Test_DrillString_GET()
        {
            #region post a DrillString
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DrillStringComponentPart drillStringComponentPart = new()
            {
                InnerDiameter = 1.0,
                OuterDiameter = 2.0
            };
            DrillStringComponent drillStringComponent = new()
            {
                MetaInfo = metaInfo,
                Length = 1.0,
                Type = DrillStringComponentTypes.Mwd,
                Name = "MyMWD",
                PartList = [drillStringComponentPart]
            };
            DrillStringSection drillStringSection1 = new()
            {
                Name = "My test DrillStringSection name 1",
                SectionComponentList = [drillStringComponent]
            };
            DrillStringSection drillStringSection2 = new()
            {
                Name = "My test DrillStringSection name 2",
                SectionComponentList = [drillStringComponent]
            };
            DrillString drillString = new()
            {
                MetaInfo = metaInfo,
                Name = "My test DrillString",
                DrillStringSectionList = [drillStringSection1, drillStringSection2]
            };

            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST given DrillString\n" + ex.Message);
            }
            #endregion

            #region GetAllDrillStringId
            List<Guid> idList = [];
            try
            {
                idList = (List<Guid>)await nSwagClient.GetAllDrillStringIdAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all DrillString ids\n" + ex.Message);
            }
            Assert.That(idList, Is.Not.Null);
            Assert.That(idList, Does.Contain(guid));
            #endregion

            #region GetAllDrillStringMetaInfo
            List<MetaInfo> metaInfoList = [];
            try
            {
                metaInfoList = (List<MetaInfo>)await nSwagClient.GetAllDrillStringMetaInfoAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET all DrillString metainfos\n" + ex.Message);
            }
            Assert.That(metaInfoList, Is.Not.Null);
            IEnumerable<MetaInfo> metaInfoList2 =
                from elt in metaInfoList
                where elt.ID == guid
                select elt;
            Assert.That(metaInfoList2, Is.Not.Null);
            Assert.That(metaInfoList2, Is.Not.Empty);
            #endregion

            #region GetAllDrillStringById
            DrillString? drillString2 = null;
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Not.Null);
            Assert.That(drillString2.Name, Is.EqualTo(drillString.Name));
            #endregion

            #region GetAllDrillStringLight
            List<DrillStringLight> drillStringLightList = [];
            try
            {
                drillStringLightList = (List<DrillStringLight>)await nSwagClient.GetAllDrillStringLightAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the list of DrillStringLight\n" + ex.Message);
            }
            Assert.That(drillStringLightList, Is.Not.Null);
            Assert.That(drillStringLightList, Is.Not.Empty);
            IEnumerable<DrillStringLight> drillStringLightList2 =
                from elt in drillStringLightList
                where elt.Name == drillString.Name
                select elt;
            Assert.That(drillStringLightList2, Is.Not.Null);
            Assert.That(drillStringLightList2, Is.Not.Empty);
            #endregion

            #region GetAllDrillString
            List<DrillString> drillStringList = new();
            try
            {
                drillStringList = (List<DrillString>)await nSwagClient.GetAllDrillStringAsync();
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the list of DrillString\n" + ex.Message);
            }
            Assert.That(drillStringList, Is.Not.Null);
            IEnumerable<DrillString> drillStringList2 =
                from elt in drillStringList
                where elt.Name == drillString.Name
                select elt;
            Assert.That(drillStringList2, Is.Not.Null);
            Assert.That(drillStringList2, Is.Not.Empty);
            #endregion

            #region finally delete the new ID
            drillString2 = null;
            try
            {
                await nSwagClient.DeleteDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE DrillString of given Id\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_DrillString_POST()
        {
            #region trying to post an empty guid
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DrillStringComponentPart drillStringComponentPart = new()
            {
                InnerDiameter = 1.0,
                OuterDiameter = 2.0
            };
            DrillStringComponent drillStringComponent = new()
            {
                MetaInfo = metaInfo,
                Length = 1.0,
                Type = DrillStringComponentTypes.Mwd,
                Name = "MyMWD",
                PartList = [drillStringComponentPart]
            };
            DrillStringSection drillStringSection1 = new()
            {
                Name = "My test DrillStringSection name 1",
                SectionComponentList = [drillStringComponent]
            };
            DrillStringSection drillStringSection2 = new()
            {
                Name = "My test DrillStringSection name 2",
                SectionComponentList = [drillStringComponent]
            };
            DrillString drillString = new()
            {
                MetaInfo = metaInfo,
                Name = "My test DrillString",
                DrillStringSectionList = [drillStringSection1, drillStringSection2]
            };
            DrillString? drillString2 = null;
            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to POST DrillString with empty Guid\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(Guid.Empty);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(400));
                TestContext.WriteLine("Impossible to GET DrillString identified by an empty Guid\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Null);
            #endregion

            #region post some corrupted data
            // post data with missing input that fails the calculation process
            #endregion

            #region posting a new ID in a valid state
            guid = Guid.NewGuid();
            metaInfo = new() { ID = guid };
            drillString.MetaInfo = metaInfo;
            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST DrillString although it is in a valid state\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Not.Null);
            Assert.That(drillString2.MetaInfo, Is.Not.Null);
            Assert.That(drillString2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(drillString2.Name, Is.EqualTo(drillString.Name));
            #endregion

            #region trying to repost the same ID
            bool conflict = false;
            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                conflict = true;
                Assert.That(ex.StatusCode, Is.EqualTo(409));
                TestContext.WriteLine("Impossible to POST existing DrillString\n" + ex.Message);
            }
            Assert.That(conflict, Is.True);
            #endregion

            #region finally delete the new ID
            drillString2 = null;
            try
            {
                await nSwagClient.DeleteDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE DrillString of given Id\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_DrillString_PUT()
        {
            #region posting a new ID
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DrillStringComponentPart drillStringComponentPart = new()
            {
                InnerDiameter = 1.0,
                OuterDiameter = 2.0
            };
            DrillStringComponent drillStringComponent = new()
            {
                MetaInfo = metaInfo,
                Length = 1.0,
                Type = DrillStringComponentTypes.Mwd,
                Name = "MyMWD",
                PartList = [drillStringComponentPart]
            };
            DrillStringSection drillStringSection1 = new()
            {
                Name = "My test DrillStringSection name 1",
                SectionComponentList = [drillStringComponent]
            };
            DrillStringSection drillStringSection2 = new()
            {
                Name = "My test DrillStringSection name 2",
                SectionComponentList = [drillStringComponent]
            };
            DrillString drillString = new()
            {
                MetaInfo = metaInfo,
                Name = "My test DrillString",
                DrillStringSectionList = [drillStringSection1, drillStringSection2]
            };
            DrillString? drillString2 = null;
            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST DrillString\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Not.Null);
            Assert.That(drillString2.MetaInfo, Is.Not.Null);
            Assert.That(drillString2.MetaInfo.ID, Is.EqualTo(guid));
            Assert.That(drillString2.Name, Is.EqualTo(drillString.Name));
            #endregion

            #region updating the new Id
            drillString.Name = "My test DrillString with modified name";
            drillString.LastModificationDate = DateTimeOffset.UtcNow;
            try
            {
                await nSwagClient.PutDrillStringByIdAsync(drillString.MetaInfo.ID, drillString);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to PUT DrillString of given Id\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the updated DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Not.Null);
            Assert.That(drillString2.MetaInfo, Is.Not.Null);
            Assert.That(drillString2.MetaInfo.ID, Is.EqualTo(drillString.MetaInfo.ID));
            Assert.That(drillString2.Name, Is.EqualTo(drillString.Name));
            #endregion

            #region finally delete the new ID
            drillString2 = null;
            try
            {
                await nSwagClient.DeleteDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE DrillString of given Id\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Null);
            #endregion
        }

        [Test]
        public async Task Test_DrillString_DELETE()
        {
            #region posting a new ID
            Guid guid = Guid.NewGuid();
            MetaInfo metaInfo = new() { ID = guid };
            DrillStringComponentPart drillStringComponentPart = new()
            {
                InnerDiameter = 1.0,
                OuterDiameter = 2.0
            };
            DrillStringComponent drillStringComponent = new()
            {
                MetaInfo = metaInfo,
                Length = 1.0,
                Type = DrillStringComponentTypes.Mwd,
                Name = "MyMWD",
                PartList = [drillStringComponentPart]
            };
            DrillStringSection drillStringSection1 = new()
            {
                Name = "My test DrillStringSection name 1",
                SectionComponentList = [drillStringComponent]
            };
            DrillStringSection drillStringSection2 = new()
            {
                Name = "My test DrillStringSection name 2",
                SectionComponentList = [drillStringComponent]
            };
            DrillString drillString = new()
            {
                MetaInfo = metaInfo,
                Name = "My test DrillString",
                DrillStringSectionList = [drillStringSection1, drillStringSection2]
            };
            DrillString? drillString2 = null;
            try
            {
                await nSwagClient.PostDrillStringAsync(drillString);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to POST DrillString\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to GET the DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Not.Null);
            Assert.That(drillString2.MetaInfo, Is.Not.Null);
            Assert.That(drillString2.MetaInfo.ID, Is.EqualTo(drillString.MetaInfo.ID));
            Assert.That(drillString2.Name, Is.EqualTo(drillString.Name));
            #endregion

            #region finally delete the new ID
            drillString2 = null;
            try
            {
                await nSwagClient.DeleteDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                TestContext.WriteLine("Impossible to DELETE DrillString of given Id\n" + ex.Message);
            }
            try
            {
                drillString2 = await nSwagClient.GetDrillStringByIdAsync(guid);
            }
            catch (ApiException ex)
            {
                Assert.That(ex.StatusCode, Is.EqualTo(404));
                TestContext.WriteLine("Impossible to GET deleted DrillString of given Id\n" + ex.Message);
            }
            Assert.That(drillString2, Is.Null);
            #endregion
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            httpClient?.Dispose();
        }
    }
}