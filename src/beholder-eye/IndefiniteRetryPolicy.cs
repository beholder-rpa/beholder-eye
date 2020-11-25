namespace beholder_eye
{
    using Microsoft.AspNetCore.SignalR.Client;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class IndefiniteRetryPolicy : IRetryPolicy
    {
        private static readonly Random s_random = new Random();

        public TimeSpan? NextRetryDelay(RetryContext retryContext)
        {
            return TimeSpan.FromSeconds(s_random.Next(2, 12) * 5);
        }

        public static async Task<bool> ConnectWithRetryAsync(HubConnection connection, CancellationToken token)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            // Keep trying to until we can start or the token is canceled.
            while (true)
            {
                try
                {
                    await connection.StartAsync(token);
                    return true;
                }
                catch when (token.IsCancellationRequested)
                {
                    return false;
                }
                catch
                {
                    await Task.Delay(s_random.Next(2, 12) * 5, CancellationToken.None);
                }
            }
        }
    }
}
