using BeatBoardLib;
using System;
using System.Linq;

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
            var agents = Agent.GetAgents();

            foreach (var agent in agents.OrderBy(a => a.lastdate).ThenBy(a => a.name))
            {
                Console.WriteLine($"{agent.lastdate}: {agent.name}");
            }

            int count = agents.Count;

            Console.WriteLine($"Count: {count}");
        }
    }
}
