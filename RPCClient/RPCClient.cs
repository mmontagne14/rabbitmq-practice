private static async Task InvokeAsync(string n)
{
    var rpcClient = new RpcClient();
    await rpcClient.StartAsync();

    Console.WriteLine(" [x] Requesting fib({0})", n);
    var response = await rpcClient.CallAsync(n);
    Console.WriteLine(" [.] Got '{0}'", response);
}