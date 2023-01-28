using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace ReadTextFile
{
    class Program
    {
        static void Main()
        {
            List<DateStation> dateStationList = new List<DateStation>();
            DateStation dateStationObject = new DateStation();

            XElement root = XElement.Load("C:\\Users\\User\\Desktop\\Example.xml");
            XNamespace cim = "http://iec.ch/TC57/2014/CIM-schema-cim16#";
            XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

            IEnumerable<XElement> idSubstation =
                from element in root.Elements(cim + "Substation")
                select element;
            foreach (XElement element in idSubstation)
                dateStationList.Add(new DateStation(element.Attribute(rdf + "about").Value.ToString(), "","",""));
               // dateStationObject.SetID(element.Attribute(rdf + "about").Value);

               dateStationList.Add(new DateStation("1","2","3","4"));
            Console.WriteLine(dateStationList[0].GetID());
            dateStationList[0].SetID("8");
            Console.WriteLine(dateStationList[0].GetID());




            //запрос на получения станция

            /*      IEnumerable<XElement> address =
                      from el in root.Descendants(cim + "Substation")
                          // where (string)el.Attribute(rdf+"about")=="#_ded01782-8a0b-44b9-9c60-27ebbaac0a08"
                      select el.Element(cim + "IdentifiedObject.name");
                  foreach (XElement el in address)
                  {
                      Console.WriteLine(el.Value);
                  }*/

            //запрос на получение напряжения генератора
            /*   IEnumerable<XElement> add =
                  from el in root.Descendants(cim + "VoltageLevel")  
                  select el.Element(cim + "IdentifiedObject.name");
               foreach (XElement el in add)
               {


                   Console.WriteLine(el.Value);
               }*/

            //запрос получение типа генератора
            /*  IEnumerable<XElement> adds =
                 from el in root.Descendants(cim + "SynchronousMachine")
                 select el.Element(cim + "IdentifiedObject.name");
              foreach (XElement el in adds)
              {

                  Console.WriteLine(el.Value);
              }*/

            //запрос VoltageLevels id
            /*IEnumerable<XElement> add =
               from el in root.Descendants(cim + "Substation")
               select el.Element(cim + "Substation.VoltageLevels");

            foreach (XElement el in add)
            {
               
            // XElement idVoltageLevels = el;
            //      if (idVoltageLevels != null)
            //        Console.WriteLine(idVoltageLevels.LastAttribute.Value);*/

            /* if (el != null)
                 Console.WriteLine(el.Attribute(rdf + "resource").Value);
         }*/

            /*    //получения id станции
                IEnumerable<XElement> address =
                  from el in root.Descendants(cim + "Substation")
                      // where (string)el.Attribute(rdf+"about")=="#_ded01782-8a0b-44b9-9c60-27ebbaac0a08"

                             select el;
                foreach (XElement el in address)
                {
                    Console.WriteLine(el.Attribute(rdf+ "about"));

                }*/

            /*   //получения id станции
               IEnumerable<XElement> address =
                 from el in root.Descendants(cim + "Substation")
                 where (string)el.Attribute(rdf+"about")=="#_ded01782-8a0b-44b9-9c60-27ebbaac0a08"
                 select el.Element(cim+ "Substation.VoltageLevels");
               foreach (XElement el in address)
               {
                   if(el!=null)
                   Console.WriteLine(el.Attribute(rdf+ "resource").Value);
               }*/


            //получения по id имени станции
            /*     IEnumerable<XElement> address =
                   from el in root.Descendants(cim + "Substation")
                   where (string)el.Attribute(rdf + "about") == "#_95a391a0-f448-48e7-abef-24277dadf611"
                   select el.Element(cim + "IdentifiedObject.name");
                 foreach (XElement el in address)
                 {
                    Console.WriteLine(el.Value);
                 }*/



            /*
                        XElement root = XElement.Load("C:\\Users\\User\\Desktop\\Example.xml");
                        XNamespace cim = "http://iec.ch/TC57/2014/CIM-schema-cim16#";
                        XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

                        IEnumerable<XElement> address =
                            from el in root.Elements(cim+ "VoltageLevel.Substation")

                            select el;
                        foreach (XElement el in address)
                        {
                            Console.WriteLine(el);
                        }
            */
            Console.ReadLine();
        }
    
    }

    internal class  DateStation 
    {
        private string substation;      //название станции
        private string voltageLevel;    //название распределительного устройства
        private string name;            //название генератора 
        private string id;              // id генератора

        public string GetID()
        {
            return id;
        }

        public void SetID(string value)
        {
            id = value;
        }

        public string GetSubstation()
        {
            return substation;
        }

        public void SetSubstatio(string value)
        {
            substation = value;
        }

        public string GetVoltageLevel()
        {
            return voltageLevel;
        }

        public void SetVoltageLevel(string value)
        {
            voltageLevel = value;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string value)
        {
            name = value;
        }

        public DateStation() 
        {
            
        }

        public DateStation(string substatio, string voltageLevel, string name, string id)
        {
            this.SetSubstatio(substatio);
            this.SetVoltageLevel(voltageLevel);
            this.SetName(name);
            this.SetID(id);
        }
    }
} 
