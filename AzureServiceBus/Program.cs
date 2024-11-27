
using Azure.Messaging.ServiceBus;
using System.Transactions;

Console.WriteLine("Hello, World!");
//connection string of the service bus
string conn = "Endpoint=sb://myservicebusstud.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=e8SlpTlyy2VNvWCutTZmzvPu4Gq/NNzR/+ASbOXdTdU=";
string q1 = "queue1";
string q2 = "queue2";
string q3 = "queue3";

//create client and set cross entity transaction = true
var servicebusClient = new ServiceBusClient(conn, new ServiceBusClientOptions() { EnableCrossEntityTransactions = true });

//receive from queue1
var serviceBusReceiver1 = servicebusClient.CreateReceiver(q1);

var serviceBusSender2 = servicebusClient.CreateSender(q2);

var serviceBusSender3 = servicebusClient.CreateSender(q3);

var messageReceived = await serviceBusReceiver1.ReceiveMessageAsync();

using var trans = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
{
    await serviceBusSender2.SendMessageAsync(new ServiceBusMessage(messageReceived.Body.ToString()));
    await serviceBusSender3.SendMessageAsync(new ServiceBusMessage(messageReceived.Body.ToString()));
    await serviceBusReceiver1.CompleteMessageAsync(messageReceived);
    trans.Complete();
}
Console.WriteLine("final");

