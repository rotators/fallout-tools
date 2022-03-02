using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace ScriptEditor.SyntaxRules
{
    public class SyntaxFile
    {
        private readonly string syntaxfolder = "SyntaxRules";

        private static readonly string userRules = "User_SyntaxRules.xml";

        private static readonly string msgRules = "msg_SyntaxRules.xshd";
        private static readonly string msgDRules = "msgd_SyntaxRules.xshd";
        private static readonly string ssl0Rules = "ssl_SyntaxRules.xshd";
        private static readonly string ssl1Rules = "ssl+_SyntaxRules.xshd";
        private static readonly string ssl2Rules = "ssld_SyntaxRules.xshd";
        
        private static readonly string msgRulesPath = Path.Combine(Settings.ResourcesFolder, msgRules);
        private static readonly string msgDRulesPath = Path.Combine(Settings.ResourcesFolder, msgDRules);
        private static readonly string ssl0RulesPath = Path.Combine(Settings.ResourcesFolder, ssl0Rules);
        private static readonly string ssl1RulesPath = Path.Combine(Settings.ResourcesFolder, ssl1Rules);
        private static readonly string ssl2RulesPath = Path.Combine(Settings.ResourcesFolder, ssl2Rules);
       
        public static string SyntaxFolder
        {
            get {
                new SyntaxFile();
                return Settings.ResourcesFolder;
            }
        }

        private SyntaxFile()
        {
            LoadRules();
        }

        private void LoadRules()
        {
            if (!File.Exists(msgRulesPath)) {
                File.Copy(Path.Combine(syntaxfolder, msgRules), msgRulesPath);
                File.Copy(Path.Combine(syntaxfolder, msgDRules), msgDRulesPath);
            }
            if (!File.Exists(userRules))
                File.WriteAllText(userRules, Properties.Resources.User_SyntaxRules);

            try {
                XmlDocument user = new XmlDocument();
                user.Load(userRules);
                XmlNode node = user.LastChild;

                CreateRules(node, ssl0Rules);
                CreateRules(node, ssl1Rules);
                CreateRules(node, ssl2Rules);
            } catch { 
                File.Copy(Path.Combine(syntaxfolder, ssl0Rules), ssl0RulesPath);
                File.Copy(Path.Combine(syntaxfolder, ssl1Rules), ssl1RulesPath);
                File.Copy(Path.Combine(syntaxfolder, ssl2Rules), ssl2RulesPath);
            }
        }

        private void CreateRules(XmlNode node, string name)
        {
            XmlDocument ssl = new XmlDocument();
            ssl.Load(Path.Combine(syntaxfolder, name));
            
            XPathNavigator nodes = ssl.CreateNavigator(); 
            nodes.MoveToChild("SyntaxDefinition", "");
            nodes.MoveToChild("RuleSets", "");
            nodes.MoveToChild("RuleSet", "");
            nodes.MoveToChild("KeyWords", "");
            nodes.InsertBefore(node.InnerXml);

            ssl.Save(Path.Combine(Settings.ResourcesFolder, name));
        }

        public static void DeleteSyntaxFile()
        {
            File.Delete(msgRulesPath);
            File.Delete(msgDRulesPath);
            File.Delete(ssl0RulesPath);
            File.Delete(ssl1RulesPath);
            File.Delete(ssl2RulesPath);
        }

        public static void AddKeyWord(string keyWord)
        {
            XmlDocument user = new XmlDocument();
            user.Load(userRules);

            XmlElement node = user.SelectSingleNode("//KeyWords[@name = \"UserMacros\"]") as XmlElement;
            
            XmlElement key = user.CreateElement("Key");
            key.SetAttribute("word", keyWord);
            node.AppendChild(key);

            user.Save(userRules);
        }

        public static void RemoveKeyWord(string keyWord)
        {
            XmlDocument user = new XmlDocument();
            user.Load(userRules);

            XmlElement node = user.SelectSingleNode("//KeyWords[@name = \"UserMacros\"]/Key[@word = \"" + keyWord + "\"]") as XmlElement;
            node.ParentNode.RemoveChild(node);

            user.Save(userRules);
        }
    }
}
