#r "Newtonsoft.Json"
#r "Microsoft.WindowsAzure.Storage"

using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

public static async Task<HttpResponseMessage> Run(HttpRequestMessage req, IQueryable<Challenge3Data> tableReaderBinding, TraceWriter log)
{
    try
    {
        // Get request body
        log.Info($"Received message to trigger");

        var jsonContent = await req.Content.ReadAsAsync<Challenge32Message>();

        // extract from json
        string key = jsonContent.key;

        string partitionKey = "test-dev";
        string rowKey = key;
        var data = tableReaderBinding.Where(o => o.PartitionKey == partitionKey && o.RowKey == rowKey).FirstOrDefault();
        if (data == null)
        {
            log.Error($"ERROR: Couldn't find data with RowKey {rowKey} and ParititionKey {partitionKey}");
            return req.CreateResponse(HttpStatusCode.InternalServerError, $"ERROR: Couldn't find data with RowKey {rowKey} and ParititionKey {partitionKey}");
        }

        string values = data.ArrayOfValues;
        var vals = values.Split(',').Select(x => int.Parse(x));

        return req.CreateResponse(HttpStatusCode.OK, new
            {
                key = jsonContent.key,
                ArrayOfValues = vals
        });
    }
    catch (Exception ex)
    {
        log.Info($"challenge3TestFunction exception{ex}");
        return req.CreateResponse(HttpStatusCode.InternalServerError, "An error occured. Please notify the Code Challenge Team. functionschallenge@microsoft.com");
    }
}

// represents request msg going to  users challenge3 postback function
class Challenge32Message
{
    public string key { get; set; }
}
public class Challenge3Data : TableEntity
{
    public string ChallengeKey { get; set; }
    public string ArrayOfValues { get; set; }
}