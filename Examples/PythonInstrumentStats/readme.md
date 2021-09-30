# Python TE
Example for connection  Trade Evolution C# indicator and Python library.
 
The [Python](https://www.python.org/downloads/windows/) version +3.5 has to be installed on Windows
 
For the example app it is required to use pytho dependencies (pandas, numpy, etc.)
 
```
pip install numpy
pip install pandas
```
There are three path files available for modification: pythonPath.txt (for python executable),scriptPath.txt (for scripts folder) and csvPath.txt (for reading data from a file). 
 
Important! Three txt files should be copied to the root of TE app.
 
Meanwhile scripts folder also should be placed according to path specified in scriptPath.txt.

Same goes for csv folder. The sample is presented in the source of this project.
 
The example app uses [ProcessStartInfo](https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo?view=net-5.0) to start child process under runtime of the TE. 
 
The ProcessStartInfo object uses arguments to pass data from the example indicator. There is a [length symbol limitation](https://docs.microsoft.com/ru-ru/windows/win32/fileio/naming-a-file?redirectedfrom=MSDN) to pass via the argument according to OS.
 
The caller.py script serves as an entrypoint of the so called bridge between c# an python. 
 
The file read is happening in instrumentStats.py, and the filtered result is passed back via stdin.
