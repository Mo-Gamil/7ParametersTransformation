using Geodetic.Exercise.Shared;
using MathNet.Numerics.LinearAlgebra;

namespace Geodetic.Exercise.Two
{
    internal sealed class Parameters
    {
        public Parameters(double translationX, double translationY, double translationZ, double rotationX, double rotationY, double rotationZ, double scale) 
        {
            TranslationX = translationX;
            TranslationY = translationY;
            TranslationZ = translationZ;
            RotationX = rotationX;
            RotationY = rotationY;
            RotationZ = rotationZ;
            Scale = scale;
        }

        /// <summary>
        /// Translation X in metres.
        /// </summary>
        public double TranslationX { get; }

        /// <summary>
        /// Translation Y in metres. 
        /// </summary>
        public double TranslationY { get; }

        /// <summary>
        /// Translation Z in metres. 
        /// </summary>
        public double TranslationZ { get; }

        /// <summary>
        /// Rotation X in arc seconds.
        /// </summary>
        public double RotationX { get; }

        /// <summary>
        /// Rotation Y in arc seconds.
        /// </summary>
        public double RotationY { get; }

        /// <summary>
        /// Rotation Z in arc seconds.
        /// </summary>
        public double RotationZ { get; }

        /// <summary>
        /// Scale, no unit.
        /// </summary>
        public double Scale { get; }


        /// <summary>
        /// populate the matrices of A and b with the data from the input points
        /// </summary>
        /// <param name="targetCartesianCoordinates"></param>
        /// <param name="sourceCartesianCoordinates"></param>
        /// <param name="A"></param>
        /// <param name="b"></param>
        private static void PopulateMatrices(List<Cartesian> targetCartesianCoordinates,List<Cartesian> sourceCartesianCoordinates,
        out Matrix<double> A,
        out MathNet.Numerics.LinearAlgebra.Vector<double> b)
        {
            // Ensure both lists have the same number of points
            if (sourceCartesianCoordinates.Count != targetCartesianCoordinates.Count)
            {
                Console.WriteLine("Error: Source and target coordinate lists must have the same number of points.");
                
            }

            int numPoints = sourceCartesianCoordinates.Count;
            int numObservations = numPoints * 3; // 3 rows per point
            int numParameters = 7; // tX, tY, tZ, rX, rY, rZ, dS

            // Initialize design matrix (A) and observation vector (b)
            A = Matrix<double>.Build.Dense(numObservations, numParameters);
            b = MathNet.Numerics.LinearAlgebra.Vector<double>.Build.Dense(numObservations);

            for (int i = 0; i < numPoints; i++)
            {
                var source = sourceCartesianCoordinates[i];
                var target = targetCartesianCoordinates[i];
                // Get the cartesian coords of both source and target points 
                double Xs = source.X, Ys = source.Y, Zs = source.Z;
                double Xt = target.X, Yt = target.Y, Zt = target.Z;

                // Row indices in A and b
                int rowX = 3 * i;
                int rowY = 3 * i + 1;
                int rowZ = 3 * i + 2;

                // Populate A
                A[rowX, 0] = 1; A[rowX, 1] = 0; A[rowX, 2] = 0; A[rowX, 3] = 0; A[rowX, 4] = -Zs; A[rowX, 5] = Ys; A[rowX, 6] = Xs;
                A[rowY, 0] = 0; A[rowY, 1] = 1; A[rowY, 2] = 0; A[rowY, 3] = Zs; A[rowY, 4] = 0; A[rowY, 5] = -Xs; A[rowY, 6] = Ys;
                A[rowZ, 0] = 0; A[rowZ, 1] = 0; A[rowZ, 2] = 1; A[rowZ, 3] = -Ys; A[rowZ, 4] = Xs; A[rowZ, 5] = 0; A[rowZ, 6] = Zs;

                // Populate b
                b[rowX] = Xt - Xs;
                b[rowY] = Yt - Ys;
                b[rowZ] = Zt - Zs;
            }
        }




        /// <summary>
        /// use least squares to solve for the 7-transformation parameters 
        /// </summary>
        /// <param name="targetCartesianCoordinates"></param>
        /// <param name="sourceCartesianCoordinates"></param>
        /// <returns></returns>
        public static Parameters calculateHelmertTransformationParams(List<Cartesian> targetCartesianCoordinates, List<Cartesian> sourceCartesianCoordinates)
        {
            Matrix<double> A;
            MathNet.Numerics.LinearAlgebra.Vector<double> b;
            // call the PopulateMatrices function to populate A and b
            PopulateMatrices( targetCartesianCoordinates, sourceCartesianCoordinates, out A,out b);

            // Solve normal equations
            var C = A.Transpose() * A;
            var D = A.Transpose() * b;
            var X = C.Solve(D);

            //var X = ((A.Transpose() * A)
                             //.Inverse() * A.Transpose() * b).ToArray();

           
            /*
             * tX: 1566.917682, tY: 73.052551, tZ: 2131.797471
            rX: 0.000095, rY: -0.000028, rZ: 0.000167, dS: -0.000438
            */
            // Output the 7 parameters
            Console.WriteLine("Transformation Parameters:");
            Console.WriteLine($"tX: {X[0]:F6}, tY: {X[1]:F6}, tZ: {X[2]:F6}");
            Console.WriteLine($"rX: {X[3]:F6}, rY: {X[4]:F6}, rZ: {X[5]:F6}, dS: {X[6]:F6}");

            return new Parameters(X[0], X[1], X[2], X[3], X[4], X[5], X[6]);
        }


        /// <summary>
        /// disply the residuals
        /// </summary>
        /// <param name="targetCartesianCoordinates"></param>
        /// <param name="sourceCartesianCoordinates"></param>
        public static void printResiduals(List<Cartesian> targetCartesianCoordinates, List<Cartesian> sourceCartesianCoordinates)
        {
            Matrix<double> A;
            MathNet.Numerics.LinearAlgebra.Vector<double> b;
            // call the PopulateMatrices function to populate A and b
            PopulateMatrices(targetCartesianCoordinates,sourceCartesianCoordinates, out A, out b);
            // Solve normal equations
            var C = A.Transpose() * A;
            var D = A.Transpose() * b;
            var X = C.Solve(D);

            //var X = ((A.Transpose() * A)
            //               .Inverse() * A.Transpose() * b).ToArray();
            var residuals = A * X - b;

            Console.WriteLine("Residuals:");
            for (int i = 0; i < residuals.Count; i++)
            {
                Console.WriteLine($"{residuals[i]:F6}");
            }
        }


        /// <summary>
        /// Display the matrix of A and b
        /// </summary>
        /// <param name="targetCartesianCoordinates"></param>
        /// <param name="sourceCartesianCoordinates"></param>
        public static void printAandbMatrices(List<Cartesian> targetCartesianCoordinates, List<Cartesian> sourceCartesianCoordinates)
        {
            Matrix<double> A;
            MathNet.Numerics.LinearAlgebra.Vector<double> b;
            // call the PopulateMatrices function to populate A and b
            PopulateMatrices( targetCartesianCoordinates, sourceCartesianCoordinates, out A, out b);

            // Print matrix A
            Console.WriteLine("Design Matrix A:");
            for (int i = 0; i < A.RowCount; i++)
            {
                for (int j = 0; j < A.ColumnCount; j++)
                {
                    Console.Write($"{A[i, j]:F6}\t");
                }
                Console.WriteLine();
            }

            // Print vector b
            Console.WriteLine("\nObservation Vector b:");
            for (int i = 0; i < b.Count; i++)
            {
                Console.WriteLine($"{b[i]:F6}");
            }

        }



        /// <summary>
        /// Compares the target coordinated with the calculated cartesian coorinates form the transformation 
        /// parameters
        /// </summary>
        /// <param name="targetCartesianCoordinates"></param>
        /// <param name="sourceCartesianCoordinates"></param>
        public static void validateTransformationParams(List<Cartesian> targetCartesianCoordinates, List<Cartesian> sourceCartesianCoordinates)
        {
            Matrix<double> A;
            MathNet.Numerics.LinearAlgebra.Vector<double> b;
            // call the PopulateMatrices function to populate A and b
            PopulateMatrices( targetCartesianCoordinates, sourceCartesianCoordinates, out A, out b);

            // Solve normal equations
            var C = A.Transpose() * A;
            var D = A.Transpose() * b;
            var X = C.Solve(D);


            for (int i = 0; i < targetCartesianCoordinates.Count; i++)
            {
                var source = sourceCartesianCoordinates[i];

                // use the same equations that were used to derive the parameters
                var transformedX = X[0] - X[5]*source.Y + X[4]*source.Z + X[6]*source.X + source.X ;

                var transformedY = X[3]*source.Z - X[5]* source.X + X[6]* source.Y + X[1] +source.Y;

                var transformedZ = X[4] * source.X + -X[3]*source.Y +  X[6] * source.Z + X[2] +source.Z;
                Console.WriteLine($"Transformed Pt({i + 1}): X: {transformedX:F6}, Y: {transformedY:F6}, Z: {transformedZ:F6}");
                Console.WriteLine($"Target Pt({i + 1}): X: {targetCartesianCoordinates[i].X:F6}, Y: {targetCartesianCoordinates[i].Y:F6}, Z: {targetCartesianCoordinates[i].Z:F6}");
            }



        }
        /// <summary>
        /// saves the 7 parameters into a text file
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filePath"></param>
        public static void SaveParametersToFile(Parameters parameters, string filePath)
        {
            // Write the object's properties to a text file
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("TranslationX: " + parameters.TranslationX);
                writer.WriteLine("TranslationY: " + parameters.TranslationY);
                writer.WriteLine("TranslationZ: " + parameters.TranslationZ);
                writer.WriteLine("RotationX: " + parameters.RotationX);
                writer.WriteLine("RotationY: " + parameters.RotationY);
                writer.WriteLine("RotationZ: " + parameters.RotationZ);
                writer.WriteLine("Scale: " + parameters.Scale);
            }
        }






    }

}
