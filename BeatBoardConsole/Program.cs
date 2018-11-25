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
            if (args.Length != 4)
            {
                Console.WriteLine("Usage: BeatBoardConsole <baseurls> <username> <password> <duration>");
                return;
            }
            if (!TimeSpan.TryParse(args[3], out TimeSpan duration))
            {
                Console.WriteLine($"Couldn't parse duration '{args[3]}'");
                return;
            }

            try
            {
                ListAgents(args[0].Split(','), args[1], args[2], duration);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }

        static void ListAgents(string[] baseurls, string username, string password, TimeSpan duration)
        {
            DateTime now = DateTime.UtcNow;
            DateTime old = now.Add(-duration);
            Console.WriteLine($"Now: {now}");
            Console.WriteLine($"Old: {old}");

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

            DataTable table = GetPivotTable(beatnames, beats, agents, old);

            PrintTable(table);
        }

        static DataTable GetPivotTable(string[] beatnames, string[] beats, IEnumerable<BeatAgent> agents, DateTime old)
        {
            DataTable table = new DataTable();

            table.Columns.Add("Name");
            foreach (string beat in beats)
            {
                table.Columns.Add(beat);
            }

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
                            row[beat] = $"{agent[0].LastDate.ToString("yyyy-MM-dd HH:mm:ss")}, {agent[0].Version}\nOLD";
                        }
                        else
                        {
                            row[beat] = $"{agent[0].LastDate.ToString("yyyy-MM-dd HH:mm:ss")}, {agent[0].Version}";
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
                    if (!row.IsNull(column) && ((string)row[column]).SubstringUntil("\n").Length > collengths[column])
                    {
                        collengths[column] = ((string)row[column]).SubstringUntil("\n").Length;
                    }
                }
            }

            int currentOffset = 0;

            for (int column = 0; column < table.Columns.Count; column++)
            {
                int offset = collengths.Take(column).Sum() + 2 * column;
                string prepadding = new string(' ', offset - currentOffset);
                Console.Write(prepadding);

                string value = table.Columns[column].ColumnName;
                Console.Write(value);

                currentOffset = offset + value.Length;
            }
            Console.WriteLine();

            foreach (DataRow row in table.Rows)
            {
                currentOffset = 0;

                for (int column = 0; column < table.Columns.Count; column++)
                {
                    if (!row.IsNull(column))
                    {
                        int offset = collengths.Take(column).Sum() + 2 * column;
                        string prepadding = new string(' ', offset - currentOffset);
                        Console.Write(prepadding);

                        string value = (string)row[column];

                        if (value.Contains("\n"))
                        {
                            value = value.SubstringUntil("\n");
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
