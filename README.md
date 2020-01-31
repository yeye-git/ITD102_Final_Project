# ITD102_Final_Project
face recognition

Usage:
Train the model
dotnet run -mode train -trainFolder <path> -labelWord <string> -numIterations <int> -learningRate <double>
    Parameters:
        -trainFolder <path> : the path of the directory that contains training images
        -labelWord <string> : if an image's file name contains the labelWord, it would be identified as a correct answer
        -numIterations <int> : the number of iterations to train the model (grandient descend)
        -learningRate <double> : the learning rate to train the model

Test the model
dotnet run -mode test -input <path>
    Parameters:
        -input <path> the path to the input image


if you don't have .NET Core 3.0 installed, you can just run Final_Project.exe.
Final_Project.exe is a console app, you would need to use a console to run it.

e.g. 
PS D:\C# projects\ITD102_Final_Project> .\Final_Project.exe -mode train -trainFolder train_folder -labelWord me -numIterations 20 -learningRate 0.03 
