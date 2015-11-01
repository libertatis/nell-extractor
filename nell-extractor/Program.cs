using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace nell_extractor
{
    class Entry
    {
        public string Entity;
        public string Category;
        public string OtherCategories;
        public string Name;
    }

    class EntryComparer : IEqualityComparer<Entry>
    {

        public bool Equals(Entry x, Entry y)
        {
           return x.Entity == y.Entity;
        }

        public int GetHashCode(Entry obj)
        {
            return obj.Entity.GetHashCode();
        }
    }

    static class Extractor
    {
        public static string conceptString = "";
        public static string concept = "concept:food";
        static bool generalised = false;
        static void Main(string[] args)
        {
            conceptString = args[0];
            generalised = (args[1] == "general");
            concept = "concept:" + conceptString;
            ConcurrentBag<Entry> entries = new ConcurrentBag<Entry>();

            Parallel.ForEach(File.ReadLines("nell.csv"), (line, _, lineNumber) =>
            {
                string[] split = line.Split('\t').Select(t => t.Trim()).ToArray();
                if (split[1] != "generalizations" || !IsRelatedToConcept(split))
                    return;
                else
                    entries.Add(GenerateEntry(split));
            });
            HashSet<Entry> cleanedEntries = new HashSet<Entry>(entries, new EntryComparer());
            using(StreamWriter writer = new StreamWriter(conceptString+".txt"))
            {
                foreach (Entry e in cleanedEntries)
                    writer.WriteLine(e.Entity + "\t" + e.Category + " " + e.OtherCategories + "\t" + e.Name);
            }

            Console.WriteLine("Found a total of {0} entries, written to {1}.", cleanedEntries.Count, conceptString+".txt");
            Console.ReadLine();
        }

        public static bool IsRelatedToConcept(string[] columns)
        {
            return (columns[2] == concept || columns[10].Contains(concept) || (generalised && columns[11].Contains(concept)));
            //if you want 'indirect' things - e.g. condiments listed as concept:condiment, as condiment is an instance of food - use this as well.
            //  || columns[11].Contains(concept));
        }

        public static Entry GenerateEntry(string[] columns)
        {
            return new Entry()
            {
                Entity = columns[0].Split(':').Last(),
                Category = columns[2].Split(':').Last(),
                OtherCategories = String.Join(" ", (columns[10]).Split(' ').Select(t => t.Split(':').Last()).Where(c => c != columns[2].Split(':').Last())),
                Name = columns[8]
            };
        }
    }
}
