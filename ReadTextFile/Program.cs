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
        private static int cout = 0;

        static void Main()
        {
            List<DateStation> dateStationList = new List<DateStation>();
            List<SynchronousMachine> synchronousMachine = new List<SynchronousMachine>();

            XElement root = XElement.Load("C:\\Users\\User\\Desktop\\Example.xml");
            XNamespace cim = "http://iec.ch/TC57/2014/CIM-schema-cim16#";
            XNamespace rdf = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

            IEnumerable<XElement> idSubstation =
               from element in root.Elements(cim + "Substation")
               select element;
            foreach (XElement element in idSubstation)
                dateStationList.Add(new DateStation(element.Attribute(rdf + "about").Value.ToString(), "", "", "", ""));


            IEnumerable<XElement> substation =
               from element in root.Elements(cim + "Substation")
               select element.Element(cim + "IdentifiedObject.name");

            foreach (XElement element in substation)
            {
                dateStationList[cout].SetSubstatioName(element.Value);
                cout++;

            }
            cout = 0;

            for (int i = 0; i < dateStationList.Count; i++)
            {

                IEnumerable<XElement> idVoltageLevels =
                                from element in root.Descendants(cim + "Substation")
                                where (string)element.Attribute(rdf + "about") == dateStationList[i].GetSubstationID()
                                select element.Element(cim + "Substation.VoltageLevels");
                foreach (XElement element in idVoltageLevels)
                {
                    if (element != null)
                    {
                        dateStationList[i].SetGeneratorID(element.Attribute(rdf + "resource").Value);
                    }
                    else
                    {
                        dateStationList[i].SetGeneratorID("Элемент не найден!");
                    }
                }

                //запрос на получение напряжения генератора
                IEnumerable<XElement> voltageLevel =
                from element in root.Descendants(cim + "VoltageLevel")
                where (string)element.Attribute(rdf + "about") == dateStationList[i].GetGeneratorID()
                select element.Element(cim + "IdentifiedObject.name");

                if (!voltageLevel.Any())
                {
                    dateStationList[i].SetVoltageLevel("Нет значения!");
                }

                foreach (XElement element in voltageLevel)
                {
                    if (!element.Value.Equals(""))
                    {
                        dateStationList[i].SetVoltageLevel(element.Value);
                    }
                }
            }


            IEnumerable<XElement> synchronousMachineID =
                    from element in root.Descendants(cim + "SynchronousMachine")
                    select element;
            foreach (XElement el in synchronousMachineID)
            {
                if (el != null)
                    synchronousMachine.Add(new SynchronousMachine(el.Attribute(rdf + "about").Value, "", ""));
            }

            for (int i = 0; i < synchronousMachine.Count; i++)
            {
                //получение в SynchronousMachine у поля Equipment.EquipmentContainer его id
                IEnumerable<XElement> equipmentContainer =
                       from element in root.Descendants(cim + "SynchronousMachine")
                       where (string)element.Attribute(rdf + "about") == synchronousMachine[i].GetSynchronousMachineID()
                       select element.Element(cim + "Equipment.EquipmentContainer");
                foreach (XElement el in equipmentContainer)
                {
                    if (el != null)
                        synchronousMachine[i].SetEquipmentContainer(el.Attribute(rdf + "resource").Value);
                }

                //получение в SynchronousMachine типа генератора

                IEnumerable<XElement> synchronousMachineType =
                       from element in root.Descendants(cim + "SynchronousMachine")
                       where (string)element.Attribute(rdf + "about") == synchronousMachine[i].GetSynchronousMachineID()
                       select element.Element(cim + "IdentifiedObject.name");
                foreach (XElement el in synchronousMachineType)
                {
                    synchronousMachine[i].SetTypeGeneratorName(el.Value);
                }
            }

            for (int i = 0; i < synchronousMachine.Count; i++)
            {
                for (int j = 0; j < dateStationList.Count; j++)
                {
                    if (synchronousMachine[i].GetEquipmentContainer().Equals(dateStationList[j].GetGeneratorID()))
                    {
                        dateStationList[j].SetGeneratorName(synchronousMachine[j].GetTypeGeneratorName());
                    }
                }
            }




            foreach (SynchronousMachine machine in synchronousMachine)
            {
                Console.WriteLine(machine.GetEquipmentContainer() + "   " + machine.GetTypeGeneratorName() + " " + machine.GetSynchronousMachineID());
            }
            Console.WriteLine("___________________________________________________");
            foreach (DateStation station in dateStationList)
            {
                Console.WriteLine(station.GetSubstationID() + " | " + station.GetGeneratorID() + "|" + station.GetVoltageLevel() + " | " + station.GetNameSubstation() + "|" + station.GetGeneratorName());
            }

            #region
            /*    dateStationList.Add(new DateStation("1","2","3","4"));
             Console.WriteLine(dateStationList[0].GetID());
             dateStationList[0].SetID("8");
             Console.WriteLine(dateStationList[0].GetID());
    */


            /*     //запрос получение типа генератора
            for (int i = 0; i < dateStationList.Count; i++)
            {
                IEnumerable<XElement> nameGenerator =
              from element in root.Descendants(cim + "SynchronousMachine")
              select element.Element(cim + "IdentifiedObject.name");

                foreach (XElement element in nameGenerator)
                {

                    Console.WriteLine(element.Value);
                }
            }*/


            /*         //запрос на получения типа генератора
                     IEnumerable<XElement> address =
                         from el in root.Descendants(cim + "SynchronousMachine")
                         select el.Element(cim + "Equipment.EquipmentContainer");
                     foreach (XElement el in address)
                     {
                         if (el != null)
                             for (int i = 0; i < dateStationList.Count; i++) 
                             {
                                 if (el.Attribute(rdf + "resource").Value.Equals(dateStationList[i].GetGeneratorID())) 
                                 {

                                     dateStationList[i].SetGeneratorName("");
                                 }
                             }
                             Console.WriteLine(el.Attribute(rdf + "resource").Value);

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
            #endregion

            /*   string str = string.Empty;

               IEnumerable<XElement> idVoltageLevels =
                                from element in root.Elements(cim + "Substation")
                                where (string)element.Attribute(rdf + "about") == "#_5be426a2-1472-47cb-83ae-1825f4991a9e"
                                select element;
               if (!idVoltageLevels.Any())
               {
                   Console.WriteLine("Элемент не найден!");
               }

               foreach (XElement element in idVoltageLevels)
               {
                   IEnumerable<XElement> elem = element.Elements(cim + "Substation.VoltageLevels");
                   Console.WriteLine(elem.Count());
                   foreach (XElement xElem in elem)
                   {
                       if (xElem != null)
                       {
                           if (elem.Count() > 1)
                           {
                               Console.WriteLine(xElem.Attribute(rdf + "resource").Value);
                               str += xElem.Attribute(rdf + "resource").Value + " ";
                           }
                           else
                           {
                               Console.WriteLine(xElem.Attribute(rdf + "resource").Value);
                           }
                       }
                   }
               }

               Console.WriteLine(str);
               string[] arr = str.Split(new char[] {' '});
               foreach (string s in arr) 
               {
                   Console.WriteLine(s);
               }
               Console.ReadLine();
               }*/
        }
    }
    internal class  DateStation 
    {
        private string substationID;        //id станции
        private string nameSubstation;      //название станции
        private string voltageLevel;        //название распределительного устройства
        private string nameGenerator;       //название генератора 
        private string generatorID;         // id генератора
        
        public DateStation(string substationID, string nameSubstation,string generatorID, string voltageLevel, string nameGenerator)
        {
            this.SetSubstatioName(nameSubstation);
            this.SetVoltageLevel(voltageLevel);
            this.SetGeneratorName(nameGenerator);
            this.SetGeneratorID(generatorID);
            this.SetSubstationID(substationID);
        }

        public string GetSubstationID()
        {
            return substationID;
        }

        public void SetSubstationID(string value)
        {
            substationID = value;
        }

        public string GetGeneratorID()
        {
            return generatorID;
        }

        public void SetGeneratorID(string value)
        {
            generatorID = value;
        }

        public string GetNameSubstation()
        {
            return nameSubstation;
        }

        public void SetSubstatioName(string value)
        {
            nameSubstation = value;
        }

        public string GetVoltageLevel()
        {
            return voltageLevel;
        }

        public void SetVoltageLevel(string value)
        {
            voltageLevel = value;
        }

        public string GetGeneratorName()
        {
            return nameGenerator;
        }

        public void SetGeneratorName(string value)
        {
            nameGenerator = value;
        }
    }

    internal class SynchronousMachine
    {
    private string equipmentContainer;
    private string typeGeneratorName;
    private string synchronousMachineID;

    public SynchronousMachine(string synchronousMachineID, string equipmentContainer, string typeName)
    {
        this.equipmentContainer = equipmentContainer;
        this.typeGeneratorName = typeName;
        this.synchronousMachineID = synchronousMachineID;
    }

    public string GetSynchronousMachineID()
    {
        return synchronousMachineID;
    }

    public void SetSynchronousMachineID(string value)
    {
        synchronousMachineID = value;
    }

    public string GetEquipmentContainer()
        {
            return equipmentContainer;
        }

        public void SetEquipmentContainer(string value)
        {
            equipmentContainer = value;
        }

        public string GetTypeGeneratorName()
        {
            return typeGeneratorName;
        }

        public void SetTypeGeneratorName(string value)
        {
            typeGeneratorName = value;
        }
    }