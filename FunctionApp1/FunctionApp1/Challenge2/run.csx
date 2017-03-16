#r "Newtonsoft.Json"
using System;using System.Collections.Generic;using System.Text;using System.Net;using Newtonsoft.Json;
public static async Task<object> Run(HttpRequestMessage req, TraceWriter log){
    log.Info($"Webhook was triggered!");
    Dictionary<int, string> cipherKey = new Dictionary<int, string>();
    StringBuilder result = new StringBuilder();

    var jsonContent = await req.Content.ReadAsAsync<ChallengeJson>();

    string messageText = jsonContent.msg;
    var cipher = jsonContent.cipher;

    foreach (KeyValuePair<string, int> entry in cipher)
    {
        cipherKey.Add(entry.Value, entry.Key);
    }

    string decryptedStr = string.Empty;

    // Decode the Message by using the Cipher
    for (int i = 0; i < messageText.Length; i = i + 2)
    {
        int parsedNum = Convert.ToInt32(messageText.Substring(i, 2));
        result.Append(cipherKey[parsedNum]);
    }

    return req.CreateResponse(HttpStatusCode.OK, new
    {
        key = jsonContent.key,
        result = result.ToString()
    });}
class ChallengeJson{
    public string key { get; set; }
    public string msg { get; set; }
    public Dictionary<string, int> cipher { get; set; }}
