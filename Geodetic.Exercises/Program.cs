using Geodetic.Exercise.One;
using Geodetic.Exercises.Shared;
using System.Globalization;


// Constants for the GRS80 ellipsoid
const int a = 6378137;        // Semi-major axis 
const double b = 6356752.314140; // Semi-minor axis

// projection params
const int FE = 1000000;       // False Easting 
const int FN = 2000;          // False Northing 

Console.WriteLine("The aim of this app is to take a text file of geographic coordinates and a text" +
    " file of expected projected coordinates and output the calculated projected coordinates " +
    "and their differeances to the expected ones");
Console.WriteLine();

// Taking the input file path with the Geographic coordinates from the user in the command line
Console.WriteLine("Please type the path to the input file with the Geographic coordinates:");
string geographic_coords_file_path =Console.ReadLine();

// taking the input file path with the expected projected coordinates from the user in the command line
Console.WriteLine("Please type the path to the file with the expected projected coordinates:");
string expected_projected_coords_file_path = Console.ReadLine();

// Taking the path to the output projected coordinates
Console.WriteLine("Please type the path to save the calculated projected coordinates and consider naming the output file at the end of the path:");
string output_projected_coords_path = Console.ReadLine();

// Taking the path to the calculated differences.
Console.WriteLine("Please type the path to save the calculated differences and consider naming the output file at the end of the path:");
string output_coords_differences_path = Console.ReadLine();


    // Load geographic positions from input file
    var geographicList = Geographic.LoadGeographicPositions(geographic_coords_file_path);

    // Perform the projection
    var projectedList = Projected.ProjectPositions(geographicList, a, b, FE, FN);

    // Save the projected positions to the output file
    Projected.SaveProjectedListToFile(output_projected_coords_path, projectedList);

    // Load projected positions from input file
    var expectedProjectedCoordsList = Projected.LoadProjectedPositions(expected_projected_coords_file_path);

    var coordsDifferences = CoordinatesDifferences.CoordsDifferencesCalculation(expectedProjectedCoordsList, projectedList);

    Console.WriteLine($"Projection completed. Results are saved to {output_projected_coords_path} and {output_coords_differences_path}");

    CoordinatesDifferences.SavecoordsDifferencesListToFile(output_coords_differences_path, coordsDifferences);

    CoordinatesDifferences.displayDifferences(coordsDifferences);

