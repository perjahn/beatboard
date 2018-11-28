using BeatBoardLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BeatBoardConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: BeatBoardConsole <beaturls> <username> <password> <duration>");
                return;
            }
            if (!TimeSpan.TryParse(args[3], out TimeSpan duration))
            {
                Console.WriteLine($"Couldn't parse duration '{args[3]}'");
                return;
            }

            try
            {
                await ListBeatsAsync(args[0].Split(','), args[1], args[2], duration);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        static async Task ListBeatsAsync(string[] beaturls, string username, string password, TimeSpan duration)
        {
            DateTime now = DateTime.UtcNow;
            DateTime old = now.Add(-duration);
            Console.WriteLine($"Now: {now}");
            Console.WriteLine($"Old: {old}");

            var hosts = await Host.GetHostsAsync(beaturls, username, password);

            PrintTable(hosts, old);
        }

        static void PrintTable(List<Host> hosts, DateTime old)
        {
            var agents = hosts.SelectMany(m => m.Agents).ToArray();

            var name = new[] { "Name" };
            var colheaders = name.Concat(agents.Select(a => a.BeatType).Distinct().OrderBy(b => b)).ToArray();
            var colwidths = colheaders.Select(c => c.Length).ToArray();

            for (int i = 0; i < colheaders.Length; i++)
            {
                if (i == 0)
                {
                    foreach (var host in hosts)
                    {
                        string value = host.Agents[0].Name;
                        if (value.Length > colwidths[i])
                        {
                            colwidths[i] = value.Length;
                        }
                    }
                }
                else
                {
                    foreach (var agent in agents.Where(a => a.BeatType == colheaders[i]))
                    {
                        string value = $"{agent.LastDate:yyyy-MM-dd HH:mm:ss}, {agent.Version}";
                        if (value.Length > colwidths[i])
                        {
                            colwidths[i] = value.Length;
                        }
                    }
                }
            }

            int currentOffset = 0;

            for (int column = 0; column < colheaders.Length; column++)
            {
                int offset = colwidths.Take(column).Sum() + 2 * column;
                string prepadding = new string(' ', offset - currentOffset);
                Console.Write(prepadding);

                string value = colheaders[column];
                Console.Write(value);

                currentOffset = offset + value.Length;
            }
            Console.WriteLine();

            foreach (var host in hosts.OrderBy(h => h.Agents[0].Name))
            {
                var value = host.Agents[0].Name;
                Console.Write(value);
                currentOffset = value.Length;

                for (int column = 1; column < colheaders.Length; column++)
                {
                    if (host.Agents.Any(a => a.BeatType == colheaders[column]))
                    {
                        int offset = colwidths.Take(column).Sum() + 2 * column;
                        string prepadding = new string(' ', offset - currentOffset);
                        Console.Write(prepadding);

                        var agent = host.Agents.Single(a => a.BeatType == colheaders[column]);
                        value = $"{agent.LastDate:yyyy-MM-dd HH:mm:ss}, {agent.Version}";

                        if (agent.LastDate < old)
                        {
                            WriteColor(value, ConsoleColor.Red);
                        }
                        else
                        {
                            Console.Write(value);
                        }

                        currentOffset = offset + value.Length;
                    }
                }

                Console.WriteLine();
            }
        }

        private static void WriteColor(string message, ConsoleColor color)
        {
            ConsoleColor oldcolor = Console.ForegroundColor;
            try
            {
                Console.ForegroundColor = color;
                Console.Write(message);
            }
            finally
            {
                Console.ForegroundColor = oldcolor;
            }
        }
    }
}
