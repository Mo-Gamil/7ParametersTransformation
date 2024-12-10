using Geodetic.Exercise.Shared;
using System.Globalization;

namespace Geodetic.Exercises.Shared
{
    public sealed class Geographic
    {
        public Geographic(int id, double latitude, double longitude, double height) 
        {
            Id = id;
            Latitude = latitude;
            Longitude = longitude;
            Height = height;
        }

        /// <summary>
        /// Gets points' Id.
        /// </summary>
        public int Id { get; }
        /// <summary>
        /// Gets latitude in degrees.
        /// </summary>
        public double Latitude { get; }

        /// <summary>
        /// Gets longitude in degrees.
        /// </summary>
        public double Longitude { get; }

        /// <summary>
        /// Gets longitude in metres.
        /// </summary>
        public double Height { get; }

        /// <summary>
        /// Loads geographic positions from a txr file. data is comma separated. data are in 
        /// decimal degrees format.
        /// for further improvements, there shoudld be a check on the columns' names 
        /// and not to assume that lat comes before long. aslo check on the file existane.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Geographic> LoadGeographicPositions(string filePath)
        {

            var geographicList = new List<Geographic>();

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
                        double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double latitude) &&
                        double.TryParse(parts[2], NumberStyles.Float, CultureInfo.InvariantCulture, out double longitude) &&
                        double.TryParse(parts[3], NumberStyles.Float, CultureInfo.InvariantCulture, out double height))
                    {
                        geographicList.Add(new Geographic(Id, latitude, longitude, height));
                    }
                    else
                    {
                        Console.WriteLine($"Invalid data format on line {lineNumber}: {line}");
                    }
                }
            }

            return geographicList;
        }



        /// <summary>
        /// Convert dms formal To Decimal Degrees
        /// </summary>
        /// <param name="inputFilePath"></param>
        /// <param name="outputFilePath"></param>
        public static void ConvertDMSToDecimalDegrees(string inputFilePath, string outputFilePath)
        {

            string[] lines = File.ReadAllLines(inputFilePath);
            using (StreamWriter writer = new StreamWriter(outputFilePath))
            {
                writer.WriteLine("Id,Latitude,Longitude,Height(m)");

                int lineNumber = 0;
                foreach (string line in lines)
                {
                    lineNumber++;
                    // Skip the header row
                    //if (line.StartsWith("Target_latitude") || line.StartsWith("Source_latitude"))
                    // continue; 
                    if (lineNumber == 1) continue; // for further improvements, this could remove the first line only if it was a header.


                    string[] parts = line.Split(',');

                    if (parts.Length == 4)
                    {
                        int Id = Convert.ToInt32(parts[0]);
                        string latitude = parts[1].Trim();
                        string longitude = parts[2].Trim();
                        string height = parts[3].Trim().Replace("m", "");

                        double latDegrees = ParseDmsToDecimal(latitude);
                        double lonDegrees = ParseDmsToDecimal(longitude);

                        writer.WriteLine($"{Id},{latDegrees:F6},{lonDegrees:F6},{height}");
                    }
                }
            }
        }

        /// <summary>
        /// ParseDmsToDecimal.
        /// </summary>
        /// <param name="dms"></param>
        /// <returns></returns>
        static double ParseDmsToDecimal(string dms)
        {
            bool isNegative = dms.Contains("W") || dms.Contains("S");
            dms = dms.Replace("N", "").Replace("S", "").Replace("E", "").Replace("W", "").Replace("°", " ").Replace("’", " ").Replace("”", "").Trim();

            string[] dmsParts = dms.Split(' ');
            double degrees = double.Parse(dmsParts[0], CultureInfo.InvariantCulture);
            double minutes = double.Parse(dmsParts[1], CultureInfo.InvariantCulture);
            double seconds = double.Parse(dmsParts[2], CultureInfo.InvariantCulture);

            double decimalDegrees = degrees + ((minutes * 60) + seconds) / 3600;
            if (isNegative) decimalDegrees *= -1;

            return decimalDegrees;
        }

        /// <summary>
        /// convert geographic lat long h into  X Y Z
        /// </summary>
        /// <param name="geodeticCoordinates"></param>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        public static List<Cartesian> ConvertToCartesian(List<Geographic> geodeticCoordinates, double a, double f)
        {
            //const double a = 6378388; // semi-major axis in meters
            //const double f = 1.0 / 297; // flattening
            //double b = a * (1 - f); // semi-minor axis
            //double e2 = (a * a - b * b) / (a * a); // eccentricity squared

            var (b, e2) = EllipsoidParams(a, f);

            var cartesianCoordinates = new List<Cartesian>();

            foreach (var coord in geodeticCoordinates)
            {
                // Convert latitude and longitude from degrees to radians
                double phi = coord.Latitude * Math.PI / 180; // latitude in radians
                double lambda = coord.Longitude * Math.PI / 180; // longitude in radians
                double h = coord.Height; // height in meters

                // Calculate prime vertical radius of curvature (ν)
                double sinPhi = Math.Sin(phi);
                double v = a / Math.Sqrt(1 - e2 * sinPhi * sinPhi);

                // Calculate X, Y, Z
                double X = (v + h) * Math.Cos(phi) * Math.Cos(lambda);
                double Y = (v + h) * Math.Cos(phi) * Math.Sin(lambda);
                double Z = ((1 - e2) * v + h) * Math.Sin(phi);

                int Id = coord.Id;
                cartesianCoordinates.Add(new Cartesian(Id, X, Y, Z));
            }

            return cartesianCoordinates;
        }

        /// <summary>
        ///calculate ellipsoids b and e2 from a and f.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="f"></param>
        /// <returns></returns>
        static (double b, double e2) EllipsoidParams(double a, double f)
        {
            double b = a * (1 - f); // semi-minor axis
            double e2 = (a * a - b * b) / (a * a); // eccentricity squared
            return (b, e2);
        }

    }
}
