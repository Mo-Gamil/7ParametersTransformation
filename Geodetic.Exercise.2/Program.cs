
// please create Geographic positions from the input files
using Geodetic.Exercise.Shared;
using Geodetic.Exercises.Shared;
using System;
using System.Globalization;
using System.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Geodetic.Exercise.Two;



Console.WriteLine("This program is to take 2 text files of 8 common geographic coordinates " +
    "in a source (ED50) and a target (WGS84) and it uses least squares to solve for the 7 " +
    "transformation parameters..  ");
Console.WriteLine("------------------------------------------------");


// Take user input
Console.WriteLine("please type the path to the source coordinates(dms) text file: ");

string source_input_file_path =Console.ReadLine();
// Define a default path and name for the source_positions_degrees
string source_output_file_path_degrees = "source_positions_degrees.txt";


Console.WriteLine("please type the path to the target coordinates(dms) text file: ");
string target_input_file_path = Console.ReadLine();
// Define a default path and name for the target_positions_degrees
string target_outputFilePath_degrees = "target_positions_degrees.txt";


Console.WriteLine("please type the path to save the transformation Parameters file: ");
string transformation_parameters_path = Console.ReadLine();

// convert the dms source and targetcoordinates into decimal degrees.
Geographic.ConvertDMSToDecimalDegrees(source_input_file_path, source_output_file_path_degrees);
Geographic.ConvertDMSToDecimalDegrees(target_input_file_path, target_outputFilePath_degrees);


// Load source geographic positions in degrees from the source input file
var sourceGeographicList = Geographic.LoadGeographicPositions(source_output_file_path_degrees);

// Load target geographic positions in degrees from the target input file
var targetGeographicList = Geographic.LoadGeographicPositions(target_outputFilePath_degrees);


// convert sourceGeographicList into sourceCartesianCoordinates
var sourceCartesianCoordinates = Geographic.ConvertToCartesian(sourceGeographicList, 6378388,1.0 / 297 );

Console.WriteLine("__Source__");
Cartesian.displayCartesianCoordinates(sourceCartesianCoordinates);


// convert targetCartesianCoordinates into targetGeographicList
var targetCartesianCoordinates = Geographic.ConvertToCartesian(targetGeographicList, 6378137, 1.0/298.257223563);

Console.WriteLine("__Target__");
Cartesian.displayCartesianCoordinates(targetCartesianCoordinates);



// calculate the transformation parameters
var transformationParameters = Parameters.calculateHelmertTransformationParams (targetCartesianCoordinates, sourceCartesianCoordinates);
Parameters.printResiduals(targetCartesianCoordinates, sourceCartesianCoordinates);
Parameters.printAandbMatrices(targetCartesianCoordinates, sourceCartesianCoordinates);

Parameters.validateTransformationParams(targetCartesianCoordinates, sourceCartesianCoordinates);

Parameters.SaveParametersToFile(transformationParameters, transformation_parameters_path);
















