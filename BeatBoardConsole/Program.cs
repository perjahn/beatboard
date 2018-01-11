using BeatBoardLib;
using System;

namespace BeatBoardConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                ListAgents();
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        static void ListAgents()
        {
            Agent agent = new Agent();

            var agents = agent.GetAgentsAsync();

            foreach (var agentname in agents)
            {
                Console.WriteLine($"'{agentname}'");
            }

            int count = agents.Count;

            Console.WriteLine($"Count: {count}");
        }
    }
}
