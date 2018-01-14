using BeatBoardLib;
using System;
using System.Linq;

namespace BeatBoardConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: BeatBoardConsole <baseurl> <username> <password>");
                return;
            }

            try
            {
                ListAgents(args[0], args[1], args[2]);
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        static void ListAgents(string baseurl, string username, string password)
        {
            var agents = Agent.GetAgents(baseurl, username, password);

            foreach (var agent in agents.OrderBy(a => a.lastdate).ThenBy(a => a.name))
            {
                Console.WriteLine($"{agent.lastdate}: {agent.name}");
            }

            int count = agents.Count;

            Console.WriteLine($"Count: {count}");
        }
    }
}
