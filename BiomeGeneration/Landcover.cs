﻿/********************************************************************************************
 * 
 * Class:       Focus
 * Project:     Brave New World
 * Author:      Keith Emery
 * Date:        4/11/14
 * Description: The Focus class comprises an arraylist containing a focus (a point on the map)
 *              and all of the attendant contours (and the physical attributes) that make up 
 *              a feature on the map.
 *              
 * ******************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
//using System.Convert;

namespace BiomeGeneration
{
    public class Biome
    {
        // Constants
        const int SCREEN_HEIGHT = 486;
        int equator = SCREEN_HEIGHT / 2;
        const int CONTOUR_INTERVAL = 320;  // 520
        const int MAX_CONTOUR = 17;

        // Physical attributes received from mesh generator
        private int m_roundsOfGeneration;
        Point[] m_locations;
        private string[] m_key = { "class level" };
        private string[] m_climates;
        string m_customKey = "";



        // build key from: latitude, altitude, slope, substrate, aspect
        // use key to get climate from table
        // constructor
        public Biome(Point[] locations, int rounds)
        {
            string altitude = "";
            string aspect = "";
            string latitude = "";
            string slope = "";
            string substrate = "";

            // Substrate(int latitude, int altitude, int slope)
            m_locations = locations;
            m_roundsOfGeneration = rounds;
            string x;
 //           string[] keys = new string[m_locations.Length];
                m_key = new string[m_locations.Length];
  //              m_climates = new string[m_locations.Length];

            for(int i = 0; i < m_locations.Length; i++)
            {
                altitude = Altitude(m_locations[i]);
                aspect = Aspect(i);
                latitude = Latitude(m_locations[i]);
                slope = Slope(m_locations[i]);
                substrate = Substrate(Convert.ToInt16(latitude), Convert.ToInt16(altitude), Convert.ToInt16(slope));


                SetCustomKey(latitude, altitude, slope, substrate, aspect);
                x = GetCustomKey();
                m_key[i] = x; // keys[i];

   //              BiomeLookup pointClimate = new BiomeLookup();
  //              m_climates[i] = pointClimate.ClimateLookup(m_key[i]);
   //                            m_climates[i] = pointClimate.ClimateLookup("04010304");
              
   //             m_customKey = GetCustomKey();
   //             keys[i] = m_customKey;

            }
            m_climates = Climates(m_key);
        }

        public string[] Climates(string[] keys)
        {
            string[] climates = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                BiomeLookup pointClimate = new BiomeLookup();
                climates[i] = pointClimate.ClimateLookup(keys[i]);
            }
                return climates;
        }



   /*     public string FetchBiome(string key)
        {
            string testString = "0104030101";
            if (key.Equals(testString, StringComparison.Ordinal));

            return "green";
        } */



        private void SetCustomKey(string latitude, string altitude, string slope, string substrate, string aspect)
        {
            m_customKey = latitude + altitude + slope + substrate + aspect;
        }
        public string GetCustomKey()
        {
            return m_customKey;
        }

        public string[] GetClimate()
        {
            return m_climates;
        }

        public string[] GetKeys()
        {
            return m_key;
        }

        // member functions
 /*       public int RoundsOfGeneration
        {
            get { return roundsOfGeneration; }
            set
            {
                roundsOfGeneration = value;
            }
        } */



        // Altitude is the Maximum number of rounds of generation, minus the round in which 
        // the countour is generated. The altitude is the number of rounds remaining.
        public string Altitude(Point p)
        {
            return  (MAX_CONTOUR - m_roundsOfGeneration).ToString("D2");
        }



        // Assign the 16 directions of the compass rose to a an integer representation of aspect.
        public string Aspect(int locationIndex)
        {
            string aspect;
            switch(locationIndex)
            {
                case 0:
                case 1:
                case 15:
                    aspect = "01"; // East
                    break;
                case 2:
                    aspect = "02"; // NE
                    break;
                case 3:
                case 4:
                case 5:
                    aspect = "03"; // N
                    break;
                case 6:
                    aspect = "04"; // NW
                    break;
                case 7:
                case 8:
                case 9:
                    aspect = "05"; // W
                    break;
                case 10:
                    aspect = "06"; // SW
                    break;
                case 11:
                case 12:
                case 13:
                    aspect = "07"; // S
                    break;
                case 14:
                    aspect = "08"; // SE
                    break;
                default:
                    aspect = "09"; // Central
                    break;
            }
            return aspect;
        }


        // Reversing non-zero latitude values (zero didn't change) for demonstration. Need to re-figure logic.
        public string Latitude(Point p)
        {
            double y = p.Y;
            double calculatedLatitude;
            int latitude = 0;
            if (y < equator) // if our location is north of the equator
            {
                calculatedLatitude = (90 - (y * 180) / SCREEN_HEIGHT);
            }
            else            // if our location is south of the equator
            {
                calculatedLatitude = (((y * 180) / SCREEN_HEIGHT) - 90);
            }
            if(Math.Abs(calculatedLatitude) >= 67)
                latitude = 1;
            else if((Math.Abs(calculatedLatitude) >= 37) && (Math.Abs(calculatedLatitude) <= 66))
                latitude = 2;
            else if((Math.Abs(calculatedLatitude) >=23) && (Math.Abs(calculatedLatitude) <= 36))
                latitude = 3;
            else
                latitude = 4;
            return latitude.ToString("D2");
        }


        
        public string Slope(Point p)
        {
            string slope;
            double x = p.X;
            double y = p.Y;
            if (x == 0)
                throw new System.DivideByZeroException();
            else
            {
                if (y / x >= 10)
                    slope = "03";
                else if ((y / x < 10) && (y / x >= 5))
                    slope = "02";
                else if (y / x < 5)
                    slope = "01";
                else
                    slope = "00";
            return (slope); // This should work because each point in our point array represents the change
                                // from the prior point (in other words, the difference between point 1 and point 2.
            }

        }

        public string Substrate(int latitude, int altitude, int slope)
        {
            int substrate = 0;
            // the random numbers are a punt as I could not easily pass an array of ints
            // they will require that I prepend a '0' before each returned int.
            Random r = new Random();
            
            // conditionals for tropical substrates
            if((latitude == 1 || (latitude == 2) || (latitude == 3)) && (altitude == 1 || altitude == 2) && (slope == 1 || slope ==2))
            {
                int randomSubstrate = r.Next(2, 4);
                substrate = randomSubstrate;
            }
            // conditionals for arid substrates
            else if((latitude == 0) && (altitude == 0) && (slope == 1 || (slope == 2)))
            {
                int randomSubstrate = r.Next(1, 2);
                substrate = randomSubstrate;
            }
            // conditionals for dry temperate substrates
            else if((latitude == 3) && (altitude == 1) && (slope == 1))
            {
                int randomSubstrate = r.Next(1,2);
                substrate = randomSubstrate;
            }
            // conditionals for humid temperate substrates
            else if((latitude == 3) && (altitude == 1) && (slope == 2))
            {
                int randomSubstrate = r.Next(1, 4);
                substrate = randomSubstrate;
            }
            // conditionals for cool temperate substrates
            else if((latitude == 3) && (altitude == 2 || altitude == 3) && (slope == 2))
            {
                int randomSubstrate = r.Next(1, 4);
                substrate = randomSubstrate;
            }
            // conditionals for alpine substrates
            else if((latitude == 0) && (altitude == 3) && (slope == 3))
            {
                int randomSubstrate = r.Next(1, 3);
                if (randomSubstrate == 2)
                    randomSubstrate = 4;
                substrate = randomSubstrate;
            }
            // conditional for polar substrates
            else if((latitude == 4) && (altitude == 1) && (slope == 0))
            {
                substrate = 0;
            }
            // default substrate is rock if all other conditions fail.
            else
            {
                substrate = 1;
            }
            return substrate.ToString("D2");
        }
    }
}
