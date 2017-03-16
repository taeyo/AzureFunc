#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, ICollector<Challenge3Input> tableWriterBinding, TraceWriter log)
{
    try
    {
        // Get request body
        log.Info($"Received message to trigger");

        var jsonContent = await req.Content.ReadAsAsync<Challenge3Message>();

        // extract from json
        string key = jsonContent.Key;
        var values = jsonContent.ArrayOfValues;
        // sort int array 
        int[] sortedCopy = (from element in values orderby element ascending select element)
                   .ToArray();

        // and make sorted array to string
        string sortedValue = string.Join(",", sortedCopy);

        // add data to table binding 
        tableWriterBinding.Add(
            new Challenge3Input()
            {
                PartitionKey = "test-dev",   // for test
                RowKey = key,                // for test
                ChallengeKey = key,
                ArrayOfValues = sortedValue
            }
        );

        return req.CreateResponse(HttpStatusCode.OK, "{}");
    }
    catch (Exception ex)
    {
        log.Info($"challenge3TestFunction exception{ex}");
        return req.CreateResponse(HttpStatusCode.InternalServerError, "An error occured. Please notify the Code Challenge Team. functionschallenge@microsoft.com");
    }
}

// represents request msg going to  users challenge3 postback function
class Challenge3Message
{
    public string Key { get; set; }
    public int[] ArrayOfValues { get; set; }
}
public class Challenge3Input : TableEntity
{
    public string ChallengeKey { get; set; }
    public string ArrayOfValues { get; set; }
}