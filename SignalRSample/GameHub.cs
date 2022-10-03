using Microsoft.AspNetCore.SignalR;

namespace SignalRSample;

public class GameHub : Hub
{
    private static int Id;
    private static Game _game = new();

    public Task Join(IHubContext<GameHub> hubContext)
    {
        var id = Interlocked.Increment(ref Id);
        _game.AddPlayer(Context.ConnectionId, hubContext);
        return Clients.Caller.SendAsync("PlayerRegistered", id);
    }

    public void Result(int square)
    {
        if (Context.ConnectionId == _game.ClientTurn)
        {
            _game.ClientResult.SetResult(square);
        }
        else
        {
            //...
        }
    }
}