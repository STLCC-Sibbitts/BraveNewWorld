﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BiomeGeneration
{
    class Climate
    {
        public string key;
        public string climate;


        public static void ClimateLookup()
        {
            List<Climate> climates = new List<Climate>

            Lookup<string, string> lookup = (Lookup<string, string>)climates.ToLookup(p => p.key, p => p.climate);

            // Iterate through each IGrouping in the Lookup and output the contents.
            foreach (IGrouping<string, string> climateGroup in lookup)
            {
                // Print the key value of the IGrouping.
                Console.WriteLine(climateGroup.Key);
                // Iterate through each value in the IGrouping and print its value.
                foreach (string str in climateGroup)
                    Console.WriteLine("    {0}", str);
            }

            // Get the number of key-collection pairs in the Lookup.
            int count = lookup.Count;

            // Select a collection of Packages by indexing directly into the Lookup.
            IEnumerable<string> cgroup = lookup["c"];



            // Output the results.
            Console.WriteLine("\nPackages that have a key of 'C':");
            foreach (string str in cgroup)
                Console.WriteLine(str);

            // This code produces the following output:
            //
            // Packages that have a key of 'C'
            // Coho Vineyard 89453312
            // Contoso Pharmaceuticals 670053128

            // Determine if there is a key with the value 'G' in the Lookup.
            bool hasG = lookup.Contains("c");
        }

    }

}