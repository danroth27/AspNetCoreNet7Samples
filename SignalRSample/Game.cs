using Microsoft.AspNetCore.SignalR;

namespace SignalRSample;

public class Game
{
    private string _id1;
    private string _id2;
    private int[] _squares = new int[9];

    public void AddPlayer(string id, IHubContext<GameHub> hubContext)
    {
        if (string.IsNullOrEmpty(_id1))
        {
            _id1 = id;
        }
        else
        {
            _id2 = id;
            _ = RunGame(hubContext);
        }
    }

    public async Task RunGame(IHubContext<GameHub> hubContext)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        while (true)
        {
            var square = await hubContext.Clients.Client(_id1).InvokeAsync<int>("Turn", cts.Token);
            _squares[square - 1] = 1;
            await hubContext.Clients.Client(_id2).SendAsync("Disable", square);
            if (await GameOver())
            {
                break;
            }

            ResetCts(ref cts);
            square = await hubContext.Clients.Client(_id2).InvokeAsync<int>("Turn", cts.Token);
            _squares[square - 1] = 2;
            await hubContext.Clients.Client(_id1).SendAsync("Disable", square);
            if (await GameOver())
            {
                break;
            }
            ResetCts(ref cts);
        }
        cts.Dispose();

        #region GameOver check
        async Task<bool> GameOver()
        {
            if (_squares[0] != 0 && _squares[0] == _squares[1] && _squares[1] == _squares[2])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[0]);
                return true;
            }
            if (_squares[0] != 0 && _squares[0] == _squares[3] && _squares[3] == _squares[6])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[0]);
                return true;
            }
            if (_squares[0] != 0 && _squares[0] == _squares[4] && _squares[4] == _squares[8])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[0]);
                return true;
            }
            if (_squares[1] != 0 && _squares[1] == _squares[4] && _squares[4] == _squares[7])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[1]);
                return true;
            }
            if (_squares[2] != 0 && _squares[2] == _squares[4] && _squares[4] == _squares[6])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[2]);
                return true;
            }
            if (_squares[2] != 0 && _squares[2] == _squares[5] && _squares[5] == _squares[8])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[2]);
                return true;
            }
            if (_squares[3] != 0 && _squares[3] == _squares[4] && _squares[4] == _squares[5])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[3]);
                return true;
            }
            if (_squares[6] != 0 && _squares[6] == _squares[7] && _squares[7] == _squares[8])
            {
                await hubContext.Clients.Clients(_id1, _id2).SendAsync("Win", _squares[6]);
                return true;
            }
            return false;
        }
        #endregion
    }

    private void ResetCts(ref CancellationTokenSource cts)
    {
        if (cts.TryReset())
        {
            cts.CancelAfter(TimeSpan.FromSeconds(30));
        }
        else
        {
            cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        }
    }

    public TaskCompletionSource<int> ClientResult { get; private set; }
    
    public string ClientTurn { get; private set; }
}
