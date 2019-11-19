using System;
using RestSharp;
using Newtonsoft.Json.Linq;

namespace OrchestratorAPIusingCSharp
{
    class Program
    {
        const string ORCHESTRATOR_URL = "https://lptp-blr-tuhsam.corp.uipath.com";

        static void Main(string[] args)
        {
            string bearerToken = GetBearerToken("default", "admin", "December#2018");
            Console.WriteLine(bearerToken);

            string robotName = "MadRobo";
            string robotID = GetRobotID(bearerToken, robotName);
            Console.WriteLine(robotID);

            string processName = "TestPublish";
            string processID = GetProcessID(bearerToken, processName);
            Console.WriteLine(processID);
            Console.ReadLine();

            RunJob(bearerToken, processID, robotID);
        }

        private static void RunJob(string bearerToken, string processID, string robotID)
        {
            string startJobURL = ORCHESTRATOR_URL + "/odata/Jobs/UiPath.Server.Configuration.OData.StartJobs";
            JObject reqbody = new JObject(new JProperty("startInfo",new JObject(
                                              new JProperty("ReleaseKey", processID),
                                              new JProperty("Strategy", "Specific"),
                                              new JProperty("RobotIds", new JArray(Convert.ToUInt32(robotID))),
                                              new JProperty("Source", "Manual"))
                                          ));

            var client = new RestClient(startJobURL);
            var request = new RestRequest(Method.POST);
            
            request.AddParameter("startJobParameters", reqbody.ToString(), ParameterType.RequestBody);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", bearerToken);

            IRestResponse response = client.Execute(request);
            string responseContent = response.Content;
        }

        
        private static string GetRobotID(string bearerToken, string robotName)
        {
            string getRobotIdURL = ORCHESTRATOR_URL +"/odata/Robots?$filter=Name eq '"+ robotName + "'";
            var client = new RestClient(getRobotIdURL);
            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", bearerToken);

            IRestResponse response = client.Execute(request);
            string responseContent = response.Content;
            dynamic data = JObject.Parse(responseContent);
            string robotID = Convert.ToString((data.value[0]).Id.Value);

            return robotID;
        }

        private static string GetProcessID(string bearerToken, string processName)
        {
            string getProcessIdURL = ORCHESTRATOR_URL + "/odata/Releases?$filter=ProcessKey eq '" + processName + "'";

            var client = new RestClient(getProcessIdURL);
            var request = new RestRequest(Method.GET);

            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Authorization", bearerToken);
            IRestResponse response = client.Execute(request);

            string responseContent = response.Content;
            dynamic data = JObject.Parse(responseContent);
            string processID = Convert.ToString((data.value[0]).Key.Value);

            return processID;
        }

        public static string GetBearerToken(string tenant, string user, string password)
        {
            var client = new RestClient(ORCHESTRATOR_URL + "/api/account/authenticate");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");

            JObject reqbody = new JObject(
                            new JProperty("tenancyName", tenant),
                            new JProperty("usernameOrEmailAddress", user),
                            new JProperty("password", password));

            request.AddParameter("loginModel", reqbody.ToString(), ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            string responseContent = response.Content;
            dynamic data = JObject.Parse(responseContent);
            return "Bearer " + data.result;
        }
    }
}
