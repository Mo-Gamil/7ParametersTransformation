using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geodetic.Exercise.Shared
{

    public sealed class Cartesian
    {
        public Cartesian(int id, double x, double y, double z)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
        }
        /// <summary>
        /// Gets points' Id.
        /// </summary>
        public int Id {  get; }
        /// <summary>
        /// Gets points' X.
        /// </summary>
        public double X { get; }
        /// <summary>
        /// Gets points' Y.
        /// </summary>
        public double Y { get;  }
        /// <summary>
        /// Gets points' Z.
        /// </summary>
        public double Z { get; }

        /// <summary>
        /// display the cartesian coordinates for visualization
        /// </summary>
        /// <param name="CartesianCoordinates"></param>
        public static void displayCartesianCoordinates (List<Cartesian> CartesianCoordinates)
        {
            foreach (var coord in CartesianCoordinates)
            {

                Console.WriteLine($"Pt({coord.Id}), X: {coord.X}, Y: {coord.Y}, Z: {coord.Z}");
            }
        }
    }
}
