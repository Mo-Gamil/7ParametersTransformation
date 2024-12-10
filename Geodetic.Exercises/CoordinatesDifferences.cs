using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geodetic.Exercise.One
{
    public sealed class CoordinatesDifferences
    {
        public CoordinatesDifferences(int id, double dx, double dy, double dz)
        {
            Id = id;
            Dx = dx;
            Dy = dy;
            Dz = dz;
        }
        public int Id { get; }

        public double Dx { get; }

        public double Dy { get; }

        public double Dz { get; }


        /// <summary>
        /// Calculate the differences between the calculated and the expected projected coordinates.
        /// </summary>
        /// <param name="expectedProjectedCoordsList"></param>
        /// <param name="calculatedProjectedCoordsList"></param>
        /// <returns></returns>
        public static List<CoordinatesDifferences> CoordsDifferencesCalculation(List<Projected> expectedProjectedCoordsList, List<Projected> calculatedProjectedCoordsList)
        {
            // for further improvements, we need to check on the points ids if they match. 
            

            var coordsDifferences = new List<CoordinatesDifferences>();

            if (expectedProjectedCoordsList.Count == calculatedProjectedCoordsList.Count)
            {
                foreach (var expectedObject in expectedProjectedCoordsList)
                {
                    // get the exact coordinate object based on the same Id. 
                    var calculatedObject = calculatedProjectedCoordsList.FirstOrDefault(x => x.Id == expectedObject.Id);
                    if (calculatedObject != null)
                    {
                        double northingDifference = Math.Round(calculatedObject.Northing - expectedObject.Northing, 4);
                        double eastingDifference = Math.Round(calculatedObject.Easting - expectedObject.Easting, 4);
                        double heightDifference = Math.Round(calculatedObject.Height - expectedObject.Height, 4);

                        coordsDifferences.Add(new CoordinatesDifferences(
                                expectedObject.Id,
                                northingDifference,
                                eastingDifference,
                                heightDifference));

                    }
                }

            }
            else
            {
                Console.WriteLine("the no of the expected coordinates does not match the no of the calculated points!");
            }


            return coordsDifferences;
        }



        /// <summary>
        /// Saves the list of coordinates differences objects to a file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="coordsDifferences"></param>
        public static void SavecoordsDifferencesListToFile(string filePath, List<CoordinatesDifferences> coordsDifferences)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Write the header
                writer.WriteLine("Id,Dx,Dy,Dz");

                // Write each difference object
                foreach (var coordDifference in coordsDifferences)
                {
                    writer.WriteLine($"{coordDifference.Id},{coordDifference.Dx},{coordDifference.Dy},{coordDifference.Dz}");
                }
            }
        }




        public static void displayDifferences(List<CoordinatesDifferences> differnces)
        {
            Console.WriteLine("---------Coordinates' differences ---------");
            foreach (var coordDifference in differnces)
            {
                
                Console.WriteLine($"{coordDifference.Id} => Dx:{coordDifference.Dx} ,Dy: {coordDifference.Dy}, Dz: {coordDifference.Dz} ");
               
            }


        }

        }
}
