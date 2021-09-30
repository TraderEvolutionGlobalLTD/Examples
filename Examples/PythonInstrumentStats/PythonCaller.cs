using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Generic;
using System;

namespace PythonInstrumentStats
{
    class PythonCallerArgs
    {
        private string modelArgs;
        private Dictionary<string, object> metaArgs;
        private JsonSerializerSettings jsonSer;
        private string path;
        public PythonCallerArgs()
        {
            metaArgs = new Dictionary<string, object>();
            jsonSer = new JsonSerializerSettings();
            jsonSer.StringEscapeHandling = StringEscapeHandling.EscapeHtml;
        }
        public void AddMetaArg(string property, object obj)
        {
            //Convert each argument to json to keep all consistently
            metaArgs.Add(JsonConvert.SerializeObject(property, jsonSer), JsonConvert.SerializeObject(obj, jsonSer));
        }
        public void SetArg(InstrumentStats instrumentStats)
        {
            modelArgs = instrumentStats.symbol;
        }
        public void SetPathArg(string _path)
        {
            path = _path;
        }
        public string Serialized()
        {
            DateTime tm = DateTime.Now;
            AddMetaArg("date", tm);
            AddMetaArg("model_input", modelArgs);
            AddMetaArg("csv_path", path);
            return JsonConvert.SerializeObject(metaArgs);
        }
    }

    class ColData 
    {
        public List<string> columns { get; set; }
        public List<List<string>> data { get; set; }
    }

    //Class for script caller.
    class PythonCaller
    {
        private ProcessStartInfo psi;
        private string scriptPath = "";
        private string csvPath = "";
        private string scriptName = "";
        private string errors = "";
        private string results = "";
        //Script-aggregator with the logic for calling the desired method
        private string caller = "";
        private string envPath = "";

        public PythonCaller(string scriptName_)
        {
            //callerPath file contains the absolute path or the relative to the caller.py
            var script_folder = System.IO.File.ReadAllText("scriptPath.txt");
            var csv_path = System.IO.File.ReadAllText("csvPath.txt");
            caller = script_folder + "\\caller.py";

            psi = new ProcessStartInfo();
            //pythonPath file contains the absolute path to the python.exe
            psi.FileName = System.IO.File.ReadAllText("pythonPath.txt");
            string[] separatingStrings = { "\\python.exe" };
            envPath = psi.FileName.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries)[0];
            // Get control over stdin and stderr
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            scriptPath = script_folder;
            scriptName = scriptName_;
            csvPath = csv_path;
        }

        public ColData CallClassMethod(string className, string method, PythonCallerArgs args)
        {
            results = "";
            errors = "";
            args.SetPathArg(csvPath);
            string argString = args.Serialized();

            //Caller produces the command:
            // path/to/python.exe caller.py path/to/python/module python_module_name class_name method_name args_string
            //caller.py is the aggregator script, which finds the desired script (python_module_name.py), imports its contents and calls the class method
            //args_string is JSON-formatted array of the arguments for the desired script (python_module_name.py)

            psi.Arguments = $"\"{caller}\" \"{scriptPath}\" \"{scriptName}\" \"{className}\" \"{method}\" \"{argString}\"";

            var value = System.Environment.GetEnvironmentVariable("PATH");
            var new_value = envPath + "\\:" + envPath + "\\Scripts:" + value;
            System.Environment.SetEnvironmentVariable("PATH", new_value);

            using (var process = Process.Start(psi))
            {
                errors = process.StandardError.ReadToEnd();
                results = process.StandardOutput.ReadToEnd();
            }

            ColData ob = JsonConvert.DeserializeObject<ColData> (results);
            return ob;
        }

        public string GetErrors()
        {
            return errors;
        }
    }
}
