using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using IronPython;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;


namespace rules 
{
    public class rule
    {
        public string createEngine()
        {
            try{

                ScriptEngine engine = Python.CreateEngine();
                if (engine != null)
                {
                    ScriptScope scope = engine.CreateScope();
                    scope.SetVariable("total", 0);
                    string pyScript = string.Format("total = 5 * 4");
                    ScriptSource src = engine.CreateScriptSourceFromString(pyScript);
                    var ret_val = src.Execute(scope);
                    var variable = scope.GetVariable("total");

                    return string.Format("success :)  value is: {0}", variable);
                }    

                return "failed :(";

            }
            catch(Exception ex)
            {
                return "error";
            }
        }



    }






}