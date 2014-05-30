/* ''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
''''                                                               
''''    Author:         Jaynesh Shah
''''    Copyright:      © Copyright 2014          
''''    Description:    
''''    Filename:       ConfigUtils.vb                     
''''    Name:           ConfigUtils
''''    Namespace:      SP_Gen.Classes
''''
''''                                                                
''''    Version Control                                            
''''    ***************                                             
''''    30-05-2014 :   Initial implementation (v1.0)
''''                                                               
''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
 */
using System;
using System.Xml;
using System.Xml.Serialization;
using System.IO;


namespace SP_Gen.Classes
{
    class ConfigUtils
    {
        static string appDir = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath);

        static string filename = appDir + "\\Config.xml";

        public static ConfigTemplate ReadConfig()
        {
            ConfigTemplate ct;

            if (File.Exists(filename))
            {
                FileStream fs = new FileStream(filename, FileMode.Open);
                
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ConfigTemplate));
                    ct = (ConfigTemplate)serializer.Deserialize(fs);
                }
                catch (Exception exp)
                {
                    ct = new ConfigTemplate();
                }
                finally
                {
                    fs.Close();
                }
            }
            else
            {
                ct = new ConfigTemplate();
            }

            return ct;
        }

        public static void WriteConfig(ConfigTemplate ct)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigTemplate));
                TextWriter writer = new StreamWriter(filename);

                serializer.Serialize(writer, ct);
                writer.Close();
            }
            catch
            {

            }
        }
    }
}
