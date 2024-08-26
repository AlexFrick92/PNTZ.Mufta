
using System.Xml.Linq;

namespace PNTZ.Mufta.App
{
    public class ConfigCreater
    {
        public ConfigCreater()
        {
        
        }

        public string Create(string block)
        {
            string DocName = $"TPA_DpDef_{block}.xml";
            int num = 499;

            XElement root = new XElement("DpConfiguration",
                new XElement("DataPointDefinition"));

            var dpdef = root.Element("DataPointDefinition");

            for (int i = 0; i <= num; i++)
            {
                var dp = new XElement("DpValue",
                               new XAttribute("Name", "ArrayVal1"),

                               new XElement("Provider",
                                   new XAttribute("Name", "Stend TPA"),

                                   new XElement("NodeId", $"ns=3;s=\"{block}\".\"var\"[{i}]")),

                               new XElement("Processor",
                                   new XAttribute("Name", "ArrayReader1"),
                                   new XAttribute("TargetProperty", "ArrayMember1"))
                               );
                dpdef.Add(dp);
            }       

            // Создаем XDocument с корневым элементом
            XDocument xdoc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);

            // Сохраняем XML документ в файл
            xdoc.Save(DocName);

            Console.WriteLine("XML файл успешно создан!");

            return DocName;
        }
    }
}
