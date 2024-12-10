using Geodetic.Exercises.Shared;
using System.Globalization;

namespace Geodetic.Exercise.One
{
    public sealed class Projected
    {
        public Projected(int id,double easting, double northing, double height) 
        {
            Id = id;
            Easting = easting;
            Northing = northing;
            Height = height;
        }

        /// <summary>
        /// Gets points' Id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Gets easting in metres.
        /// </summary>
        public double Easting { get; }

        /// <summary>
        /// Gets northing in metres.
        /// </summary>
        public double Northing { get; }

        /// <summary>
        /// Gets longitude in metres.
        /// </summary>
        public double Height { get; }

        /*
        public static int GetObjectCount(List<Projected> list)
        {
            return list?.Count ?? 0;
        }
        */

        /// <summary>
        ///  Loads projected positions from a txt file. data is comma separated. data are in meters format.
        /// for further improvements, there shoudld be a check on the columns' names 
        /// and not to assume that Easting comes before Northing. aslo check on the file existane.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Projected> LoadProjectedPositions(string filePath)
        {
            var ProjectedList = new List<Projected>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;

                    // Skip the header line
                    if (lineNumber == 1) continue; // for further improvements, this could remove the first line only if it was a header.

                    // Split the line and parse values
                    string[] parts = line.Split(',');

                    if (parts.Length == 4 &&
                        int.TryParse(parts[0], NumberStyles.Integer, CultureInfo.InvariantCulture, out int Id) &&
                        double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double Easting) &&
                        double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double Northing) &&
                        double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double Height))
                    {
                        ProjectedList.Add(new Projected(Id, Easting, Northing, Height));
                    }
                    else
                    {
                        Console.WriteLine($"Invalid data format on line {lineNumber}: {line}");
                    }
                }
            }

            return ProjectedList;
        }




        /// <summary>
        /// Converts an angle from degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }



        /// <summary>
        ///  Projects a list of geographic positions to a list of projected positions.
        /// </summary>
        /// <param name="geographicList"></param>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="FE"></param>
        /// <param name="FN"></param>
        /// <returns></returns>
        public static List<Projected> ProjectPositions(List<Geographic> geographicList, int a, double b, int FE, int FN)
        {
            var projectedList = new List<Projected>();

            foreach (var geo in geographicList)
            {
                // Convert latitude and longitude to radians
                double latitudeRadians = DegreesToRadians(geo.Latitude);
                double longitudeRadians = DegreesToRadians(geo.Longitude);

                // Get the point Id;
                int Id = geo.Id;

                // Calculate projected coordinates
                double E = a * (Math.Cos(latitudeRadians) + Math.Sin(longitudeRadians)) + FE;
                double N = b * (Math.Sin(latitudeRadians) + Math.Cos(longitudeRadians)) + FN;
                double H = -geo.Height;


                // Add the projected object to the list
                projectedList.Add(new Projected(Id, E, N, H));
            }

            return projectedList;
        }



        /// <summary>
        /// Saves the list of Projected objects to a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="projectedList"></param>
        public static void SaveProjectedListToFile(string filePath, List<Projected> projectedList)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write the header
                writer.WriteLine("Id,Easting,Northing,Height");

                // Write each projected object
                foreach (var projected in projectedList)
                {
                    writer.WriteLine($"{projected.Id},{projected.Easting:F4},{projected.Northing:F4},{projected.Height:F4}");
                }
            }
        }

    }


}
