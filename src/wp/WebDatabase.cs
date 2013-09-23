using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;
using SQLiteClient;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace Cordova.Extension.Commands
{
    public class WebDatabase : BaseCommand
    {
        SQLiteConnection db;

        public void openDatabase(string options)
        {
            string[] optVal = getOptVal(options);

            if (optVal == null)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }
            else
            {
                try
                {
                    string name = optVal[0];
                    string version = optVal[1];
                    string display_name = optVal[2];
                    string size = optVal[3];
                    db = new SQLiteConnection(name);
                    db.Open();
                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK));
                }
                catch (Exception e)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, e.StackTrace));
                }
            }
        }

        public void executeSql(string options)
        {
            string[] optVal = getOptVal(options);

            if (optVal == null)
            {
                DispatchCommandResult(new PluginResult(PluginResult.Status.JSON_EXCEPTION));
            }
            else
            {
                try
                {
                    string query = optVal[0];
                    string[] parameters = JsonHelper.Deserialize<string[]>(optVal[1]);
                    string queryId = optVal[2];

                    SQLiteCommand cmd = db.CreateCommand(query, parameters);
                    var data = cmd.ExecuteSql();

                    DispatchCommandResult(new PluginResult(PluginResult.Status.OK, new SqlResult(queryId, data)));
                }
                catch (Exception e)
                {
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, e.StackTrace));
                }
            }
        }

        public class SqlResult
        {
            public SqlResult()
            {
            }
            public SqlResult(string id, object data)
            {
                this.id = id;
                // JsonHelper not used (do not accept recursivity ? and do not respect object <=> Json binding ?)
                // So we use JsonConvert, but the result string is then re-serialized by the PluginResult constructor...
                this.data = JsonConvert.SerializeObject(data);
            }
            string _id;
            public string id
            {
                get { return _id; }
                set { _id = value; }
            }
            string _data;
            public string data
            {
                get { return _data; }
                set { _data = value; }
            }
        }

        private string[] getOptVal(string options)
        {
            string[] optVal = null;

            try
            {
                optVal = JsonHelper.Deserialize<string[]>(options);
            }
            catch (Exception)
            {
                // simply catch the exception, we will handle null values and exceptions together
            }

            return optVal;
        }
    }
}