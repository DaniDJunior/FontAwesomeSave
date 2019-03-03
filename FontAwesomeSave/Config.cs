using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FontAwesomeSave
{
    public class Config
    {
        public string OrigePath { get; set; }
        public string DestiniPath { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }

        public Config()
        {
            OrigePath = string.Empty;
            DestiniPath = string.Empty;
            Size = "32";
            Color = "#FFFFFF";
        }

        public static void Save(string path, Config project)
        {
            StreamWriter File = new StreamWriter(path);
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            serializer.Serialize(File, project);
            File.Close();
        }

        public static Config Open(string path)
        {
            StreamReader Arquivo = new StreamReader(path);
            XmlSerializer serializer = new XmlSerializer(typeof(Config));
            Config project = (Config)serializer.Deserialize(Arquivo);
            Arquivo.Close();
            return project;
        }
    }
}
