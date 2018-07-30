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

                    return "success :)";
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