using BeatBoardLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BeatBoardConsole
{
    class Program
    {
        class BeatAgent : Agent
        {
            public string Beat { get; set; }
        };

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Usage: BeatBoardConsole <baseurls> <username> <password>");
                return;
            }

            try
            {
                ListAgents(args[0].Split(','), args[1], args[2]);
            }
            catch (System.Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        static void ListAgents(string[] baseurls, string username, string password)
        {
            BeatAgent[] agents = baseurls.SelectMany(b =>
                Agent.GetAgents(b, username, password).Select(a => new BeatAgent
                {
                    Beat = b.NthSubstring("/", 3),
                    LastDate = a.LastDate,
                    Name = a.Name,
                    Version = a.Version
                }))
                .ToArray();

            string[] beatnames = agents.Select(a => a.Name).Distinct().OrderBy(b => b).ToArray();
            string[] beats = agents.Select(a => a.Beat).Distinct().OrderBy(b => b).ToArray();

            DataTable table = GetPivotTable(beatnames, beats, agents);

            PrintTable(table);
        }

        static DataTable GetPivotTable(string[] beatnames, string[] beats, IEnumerable<BeatAgent> agents)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Name");
            foreach (string beat in beats)
            {
                table.Columns.Add(beat);
            }

            DateTime old = DateTime.UtcNow.AddHours(-1);

            foreach (string beatname in beatnames)
            {
                DataRow row = table.NewRow();

                row[0] = beatname;

                foreach (string beat in beats)
                {
                    var agent = agents.Where(a => a.Name == beatname && a.Beat == beat).ToArray();
                    if (agent.Length > 1)
                    {
                        row[beat] = $"ERROR: {agent.Length}";
                    }
                    if (agent.Length == 1)
                    {
                        DateTime lastdate = agent[0].LastDate;
                        if (lastdate < old)
                        {
                            row[beat] = $"{agent[0].LastDate}, {agent[0].Version}\nOLD";
                        }
                        else
                        {
                            row[beat] = $"{agent[0].LastDate}, {agent[0].Version}";
                        }
                    }
                }

                table.Rows.Add(row);
            }

            return table;
        }

        static void PrintTable(DataTable table)
        {
            int[] collengths = table.Columns.Cast<DataColumn>().Select(c => c.ColumnName.Length).ToArray();

            foreach (DataRow row in table.Rows)
            {
                for (int column = 0; column < table.Columns.Count; column++)
                {
                    if (!row.IsNull(column) && ((string)row[column]).Length > collengths[column])
                    {
                        collengths[column] = ((string)row[column]).SubstringUntil("\n").Length;
                    }
                }
            }


            for (int column = 0; column < table.Columns.Count; column++)
            {
                string value = table.Columns[column].ColumnName;
                string padding = string.Join(string.Empty, Enumerable.Repeat(' ', collengths[column] - value.Length + 2));

                Log($"{value}{padding}");
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                for (int column = 0; column < table.Columns.Count; column++)
                {
                    if (row.IsNull(column))
                    {
                        Log(string.Join(string.Empty, Enumerable.Repeat(' ', collengths[column] + 2)));
                    }
                    else
                    {
                        string value = (string)row[column];
                        string padding;
                        if (value.Contains("\n"))
                        {
                            value = value.SubstringUntil("\n");
                            padding = string.Join(string.Empty, Enumerable.Repeat(' ', collengths[column] - value.Length + 2));

                            Log($"{value}{padding}", ConsoleColor.Red);
                        }
                        else
                        {
                            padding = string.Join(string.Empty, Enumerable.Repeat(' ', collengths[column] - value.Length + 2));

                            Log($"{value}{padding}");
                        }
                    }
                }

                Console.WriteLine();
            }
        }

        private static void Log(string message)
        {
            Console.Write(message);
        }

        private static void Log(string message, ConsoleColor color)
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

    public static class StringExtensions
    {
        public static string NthSubstring(this string text, string substring, int count)
        {
            int offset = 0;
            for (int i = 0; i < count; i++)
            {
                offset = text.IndexOf(substring, offset);
                if (offset == -1)
                {
                    return string.Empty;
                }
                offset += substring.Length;
            }

            return text.Substring(offset);
        }

        public static string SubstringUntil(this string text, string terminator)
        {
            int offset = text.IndexOf(terminator);
            if (offset == -1)
            {
                return text;
            }

            return text.Substring(0, offset);
        }
    }
}
